using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

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
        //player.lastAttackTime = 0f; // ��� ���� ����
        player.lastAttackTime = player.GetAttackCooldown(); // ��� ���� ����
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
        Collider[] near = Physics.OverlapSphere(transform.position, 1.5f, mask, QueryTriggerInteraction.Collide);
        float best = float.MaxValue;
        foreach (var c in near)
        {
            var esm = c.GetComponentInParent<EnemyStatsManager>();
            if (esm == null || esm.CurrentHP <= 0) continue;

            // �ݶ��̴������� �ִܰŸ� ����(��ħ/�ʱ��� ����)
            Vector3 origin = transform.position + Vector3.up * 1f;
            float d = Vector3.Distance(origin, c.ClosestPoint(origin));
            if (d < best)
            {
                best = d;
                enemy = esm;
            }
        }
        return enemy != null;
    }

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

        float damage = stats.CalculateDamage();
        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.CurrentHP}");
        targetEnemy.TakeDamage(damage);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.CurrentHP}");

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
