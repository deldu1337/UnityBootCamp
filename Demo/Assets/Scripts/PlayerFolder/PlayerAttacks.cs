using System.Collections;
using UnityEngine;

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
        // ���콺 Ŭ�� �� Ÿ�� ����
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                EnemyStatsManager clickedEnemy = hit.collider.GetComponent<EnemyStatsManager>();
                if (clickedEnemy != null && ((1 << hit.collider.gameObject.layer) & player.enemyLayer) != 0)
                {
                    player.SetTarget(clickedEnemy);
                    player.ChangeState(new AttackingStates());
                }
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
        bool targetDead = player.targetEnemy == null || player.targetEnemy.CurrentHP <= 0;

        if (!targetDead)
        {
            // Ÿ�� ȸ��
            player.RotateTowardsTarget(player.targetEnemy.transform.position);

            // ���� ��Ÿ�� üũ
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

        // ���콺 Ŭ�� �� Ÿ�� ����
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                EnemyStats clickedEnemy = hit.collider.GetComponent<EnemyStats>();
                if (clickedEnemy == null || ((1 << hit.collider.gameObject.layer) & player.enemyLayer) == 0)
                {
                    player.ClearTarget();
                    player.ChangeState(new IdleStates());
                }
            }
        }

        // ���� �׾��� ���� �ִϸ��̼ǵ� �����ٸ� Idle ��ȯ
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

    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        stats = GetComponent<PlayerStatsManager>();

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
