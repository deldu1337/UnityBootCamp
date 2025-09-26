using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static DamageTextManager;

// ���� �������̽�
public interface IPlayerStates
{
    void Enter(PlayerAttacks player);
    void Exit(PlayerAttacks player);
    void Update(PlayerAttacks player);
}

// Idle ���� (���)
public class IdleStates : IPlayerStates
{
    public void Enter(PlayerAttacks player)
    {
        if (player.animationComponent != null)
            player.animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.2f);
    }

    public void Exit(PlayerAttacks player) { }

    public void Update(PlayerAttacks player)
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (player.TryPickEnemyUnderMouse(out var clickedEnemy))
            {
                player.SetTarget(clickedEnemy);
                player.ChangeState(new AttackingStates());
            }
        }
    }
}

// Attacking ����
public class AttackingStates : IPlayerStates
{
    public void Enter(PlayerAttacks player)
    {
        player.lastAttackTime = Mathf.Max(player.lastAttackTime, Time.time);
    }

    public void Exit(PlayerAttacks player) { }

    public void Update(PlayerAttacks player)
    {
        if (player.isCastingSkill) return; // ��ų ���� ���̸� ���� ���� �ߴ�

        bool targetDead = player.targetEnemy == null || player.targetEnemy.CurrentHP <= 0;

        if (!targetDead)
        {
            player.RotateTowardsTarget(player.targetEnemy.transform.position);

            if (Time.time >= player.lastAttackTime)
            {
                Collider enemyCollider = player.targetEnemy.GetComponent<Collider>();
                Vector3 playerOrigin = player.transform.position + Vector3.up * player.raycastYOffset;
                Vector3 closest = enemyCollider.ClosestPoint(playerOrigin);
                float distance = Vector3.Distance(playerOrigin, closest);

                if (distance <= player.GetAttackRange())
                {
                    player.PerformAttack();
                    player.lastAttackTime = Time.time + player.GetAttackCooldown();
                }
            }
        }

        // ��Ŭ��: �ٸ� ������ ��ȯ �Ǵ� ����
        if (Input.GetMouseButtonDown(1))
        {
            if (player.TryPickEnemyUnderMouse(out var clickedEnemy))
            {
                // �ٸ� ���� �������� Ÿ�� ��ü
                if (clickedEnemy != player.targetEnemy)
                    player.SetTarget(clickedEnemy);
            }
            else
            {
                // ���� �ƴ� ���� �����ٸ� Ÿ�� ����
                player.ClearTarget();
                player.ChangeState(new IdleStates());
            }
        }

        if (targetDead && !player.isAttacking)
        {
            player.ClearTarget();
            player.ChangeState(new IdleStates());
        }
    }


}

// PlayerAttack Ŭ����
public class PlayerAttacks : MonoBehaviour
{
    [Header("���� ����")]
    public float raycastYOffset = 1f;
    public LayerMask enemyLayer;

    [Header("��Ÿ��")]
    [HideInInspector] public float lastAttackTime;

    [HideInInspector] public EnemyStatsManager targetEnemy;
    [HideInInspector] public HealthBarUI targetHealthBar;
    [HideInInspector] public Animation animationComponent;

    private IPlayerStates currentState;
    private PlayerStatsManager stats;

    [HideInInspector] public bool isAttacking = false; // ���� �� ����
    [HideInInspector] public bool isCastingSkill = false;

    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        //stats = GetComponent<PlayerStatsManager>();
        stats = PlayerStatsManager.Instance; // �� �̱���

        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� Player ������ �Ǵ� �ڽĿ� �����ϴ�!");

        if (stats == null)
            Debug.LogError("PlayerCombatStats ������Ʈ�� Player�� �����ϴ�!");
    }

    void Start()
    {
        ChangeState(new IdleStates());
    }

    void Update()
    {
        currentState?.Update(this);
    }

    public void ChangeState(IPlayerStates newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public void SetTarget(EnemyStatsManager enemy)
    {
        targetEnemy = enemy;
        targetHealthBar = enemy?.GetComponentInChildren<HealthBarUI>();
    }

    public void ClearTarget()
    {
        targetEnemy = null;
        targetHealthBar = null;
    }

    public void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public float DistanceTo(EnemyStatsManager enemy)
    {
        if (enemy == null) return float.MaxValue;
        var col = enemy.GetComponent<Collider>();
        Vector3 origin = transform.position + Vector3.up * raycastYOffset;
        Vector3 closest = col != null ? col.ClosestPoint(origin) : enemy.transform.position;
        return Vector3.Distance(origin, closest);
    }

    public bool IsInAttackRange(EnemyStatsManager enemy)
    {
        return DistanceTo(enemy) <= GetAttackRange();
    }

    // ���콺 �Ʒ� �� ���� (���� ���� ����)
    public bool TryPickEnemyUnderMouse(out EnemyStatsManager enemy)
    {
        enemy = null;

        // UI ���� ����
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;

        if (Camera.main == null) return false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = enemyLayer; // Enemy ���̾

        // 1) RaycastAll: ���� ����� Enemy ��Ʈ ����
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f, mask, QueryTriggerInteraction.Collide);
        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var h in hits)
            {
                var esm = h.collider.GetComponentInParent<EnemyStatsManager>();
                if (esm != null && esm.CurrentHP > 0)
                {
                    enemy = esm;
                    return true;
                }
            }
        }

        // 2) ���� ����: SphereCast
        RaycastHit sh;
        if (Physics.SphereCast(ray, 0.3f, out sh, 100f, mask, QueryTriggerInteraction.Collide))
        {
            var esm = sh.collider.GetComponentInParent<EnemyStatsManager>();
            if (esm != null && esm.CurrentHP > 0)
            {
                enemy = esm;
                return true;
            }
        }

        // 3) ���� ����: �÷��̾� �ֺ����� ���� ����� Enemy
        //Collider[] near = Physics.OverlapSphere(transform.position, 1.5f, mask, QueryTriggerInteraction.Collide);
        //float best = float.MaxValue;
        //foreach (var c in near)
        //{
        //    var esm = c.GetComponentInParent<EnemyStatsManager>();
        //    if (esm == null || esm.CurrentHP <= 0) continue;

        //    // �ݶ��̴������� �ִܰŸ� ����(��ħ/�ʱ��� ����)
        //    Vector3 origin = transform.position + Vector3.up * 1f;
        //    float d = Vector3.Distance(origin, c.ClosestPoint(origin));
        //    if (d < best)
        //    {
        //        best = d;
        //        enemy = esm;
        //    }
        //}
        return enemy != null;
    }

    // PlayerAttacks.cs ���� ��ü
    private static readonly Collider[] _overlapCache = new Collider[16];

    //public bool TryPickEnemyUnderMouse(out EnemyStatsManager enemy)
    //{
    //    enemy = null;

    //    // 0) UI �� Ŭ���̸� ����
    //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
    //        return false;

    //    var cam = Camera.main;
    //    if (!cam) return false;

    //    int mask = enemyLayer;
    //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);

    //    // === 1) 1��: �Ϲ� Raycast (���� ������ ��Ȯ)
    //    if (Physics.Raycast(ray, out RaycastHit hit, 200f, mask, QueryTriggerInteraction.Collide))
    //    {
    //        var esm = hit.collider.GetComponentInParent<EnemyStatsManager>();
    //        if (esm != null && esm.CurrentHP > 0) { enemy = esm; return true; }
    //    }

    //    // === 2) 2��: ������ SphereCast (�����ϼ��� �ݰ� ũ��)
    //    float distCamToPlayer = Vector3.Distance(cam.transform.position, transform.position);
    //    float nearFactor = Mathf.Clamp01(1f - (distCamToPlayer - 2f) / 6f); // ī�޶�-�÷��̾� 2~8m ���� ����
    //    float sphereRadius = Mathf.Lerp(0.25f, 0.6f, nearFactor);           // 0.25~0.6 ����

    //    if (Physics.SphereCast(ray, sphereRadius, out hit, 200f, mask, QueryTriggerInteraction.Collide))
    //    {
    //        var esm = hit.collider.GetComponentInParent<EnemyStatsManager>();
    //        if (esm != null && esm.CurrentHP > 0) { enemy = esm; return true; }
    //    }

    //    // === 3) 3��: �÷��̾� ������ ���� ���� ĳ��Ʈ (�ʱ��� ��ħ ���)
    //    Vector3 playerOrigin = transform.position + Vector3.up * raycastYOffset;
    //    Vector3 toRay = ray.origin - playerOrigin;
    //    Vector3 dirFromPlayerToRay = Vector3.Project(toRay, ray.direction).normalized;
    //    if (dirFromPlayerToRay.sqrMagnitude < 0.01f) dirFromPlayerToRay = (ray.direction + transform.forward).normalized;

    //    if (Physics.SphereCast(playerOrigin, sphereRadius, dirFromPlayerToRay,
    //                           out hit, 2.5f, mask, QueryTriggerInteraction.Collide))
    //    {
    //        var esm = hit.collider.GetComponentInParent<EnemyStatsManager>();
    //        if (esm != null && esm.CurrentHP > 0) { enemy = esm; return true; }
    //    }

    //    // === 4) 4��: ȭ��� Ŀ�� �α��� ������ �а� �ܱ� (���콺 �����Ͱ� �� ��ó�ε� �ݶ��̴� �𼭸��� ��)
    //    // ���̸� ���� 1.5m ���� �ٹ��� �߽����� OverlapSphere
    //    Vector3 probe = ray.GetPoint(1.5f);
    //    int count = Physics.OverlapSphereNonAlloc(probe, 0.9f, _overlapCache, mask, QueryTriggerInteraction.Collide);
    //    if (count > 0)
    //    {
    //        float best = float.MaxValue;
    //        EnemyStatsManager bestEsm = null;
    //        for (int i = 0; i < count; i++)
    //        {
    //            var col = _overlapCache[i];
    //            if (!col) continue;
    //            var esm = col.GetComponentInParent<EnemyStatsManager>();
    //            if (esm == null || esm.CurrentHP <= 0) continue;

    //            // ȭ�鿡�� Ŀ������ �Ÿ��� ���� "������ ��������" Ÿ�� ����
    //            Vector3 screen = cam.WorldToScreenPoint(esm.transform.position);
    //            float screenDist = (new Vector2(screen.x, screen.y) - (Vector2)Input.mousePosition).sqrMagnitude;
    //            if (screenDist < best) { best = screenDist; bestEsm = esm; }
    //        }
    //        if (bestEsm != null) { enemy = bestEsm; return true; }
    //    }

    //    // === 5) 5��: �÷��̾� �ֺ� �ֱ��� (�ʱ��� ���� ��ħ ���� ����)
    //    count = Physics.OverlapSphereNonAlloc(transform.position, 1.8f, _overlapCache, mask, QueryTriggerInteraction.Collide);
    //    if (count > 0)
    //    {
    //        float best = float.MaxValue;
    //        EnemyStatsManager bestEsm = null;
    //        Vector3 origin = transform.position + Vector3.up * 1f;
    //        for (int i = 0; i < count; i++)
    //        {
    //            var col = _overlapCache[i];
    //            if (!col) continue;
    //            var esm = col.GetComponentInParent<EnemyStatsManager>();
    //            if (esm == null || esm.CurrentHP <= 0) continue;

    //            float d = Vector3.Distance(origin, col.ClosestPoint(origin));
    //            if (d < best) { best = d; bestEsm = esm; }
    //        }
    //        if (bestEsm != null) { enemy = bestEsm; return true; }
    //    }

    //    return false;
    //}


    public void PerformAttack()
    {
        if (targetEnemy == null) return;

        string animName = "Attack1H (ID 17 variation 0)";
        if (animationComponent.GetClip(animName) != null)
        {
            // ���� ����
            isAttacking = true;

            // �ִϸ��̼� �ӵ� ����
            animationComponent[animName].speed = stats.Data.AttackSpeed;
            animationComponent.Play(animName);

            // ���� ��Ÿ��
            lastAttackTime = Time.time + GetAttackCooldown();

            // �ִϸ��̼� ����Ʈ ������ ������ ����
            float impactTime = 0.2f;
            StartCoroutine(DelayedDamage(impactTime));

            // �ִϸ��̼� ���� �� Idle�� ��ȯ
            float animDuration = animationComponent[animName].length / animationComponent[animName].speed;
            StartCoroutine(AttackAnimationEnd(animDuration));
        }
    }

    // �ִϸ��̼� ���� �� ó��
    private IEnumerator AttackAnimationEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    // ������ �� ������ ����
    private IEnumerator DelayedDamage(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (targetEnemy == null) yield break;

        bool isCrit;
        float damage = stats.CalculateDamage(out isCrit);

        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.CurrentHP}");
        targetEnemy.TakeDamage(damage);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.CurrentHP}");

        // ġ��Ÿ�� ������, ��Ÿ�� ���
        var color = isCrit ? Color.red : Color.white;

        DamageTextManager.Instance.ShowDamage(
            targetEnemy.transform,
            Mathf.RoundToInt(damage),
            color,
            DamageTextManager.DamageTextTarget.Enemy
        );

        targetHealthBar?.CheckHp();
    }

    // PlayerAttacks.cs
    public void ForceStopAttack()
    {
        StopAllCoroutines();        // ���� ���� �ڷ�ƾ �ߴ�
        isAttacking = false;
        if (animationComponent != null)
            animationComponent.Stop(); // ���� ���� ��� �ߴ�
    }


    // === PlayerCombatStats���� �������� ���� ===
    public float GetAttackRange()
    {
        return 1f;
    }

    public float GetAttackCooldown()
    {
        return 1f / stats.Data.AttackSpeed; // ���� �ӵ��� ������ ��Ÿ�� ª����
    }
}
