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
            player.animationComponent.Play("Stand (ID 0 variation 0)");
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
                EnemyStats clickedEnemy = hit.collider.GetComponent<EnemyStats>();
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
        player.lastAttackTime = 0f; // ��� ���� ����
    }

    public void Exit(PlayerAttacks player) { }

    public void Update(PlayerAttacks player)
    {
        if (player.targetEnemy == null || player.targetEnemy.currentHP <= 0)
        {
            player.ClearTarget();
            player.ChangeState(new IdleStates());
            return;
        }

        // Ÿ�� ���� ȸ��
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

    [HideInInspector] public EnemyStats targetEnemy;
    [HideInInspector] public HealthBarUI targetHealthBar;
    [HideInInspector] public Animation animationComponent;

    private IPlayerStates currentState;
    private PlayerCombatStats stats;

    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        stats = GetComponent<PlayerCombatStats>();

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

    public void SetTarget(EnemyStats enemy)
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
            // �ִϸ��̼� ����
            float animLength = animationComponent.GetClip(animName).length;

            // ���� �ӵ� ����
            float speed = stats.AttackSpeed;
            animationComponent[animName].speed = speed * 1.2f;
            animationComponent.Play(animName);

            // �ִϸ��̼� ���̿� ���� ��Ÿ�� ����
            lastAttackTime = Time.time + (animLength / speed);
        }

        float damage = stats.CalculateDamage();

        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(damage);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        targetHealthBar?.CheckHp();
    }


    // === PlayerCombatStats���� �������� ���� ===
    public float GetAttackRange()
    {
        // ��ø(Dex)�� ���� ���� ��Ÿ��� ������ ���� ����
        //return 1f + (stats.Dex * 0.1f);
        return 1f;
    }

    public float GetAttackCooldown()
    {
        return 1f / stats.AttackSpeed; // ���� �ӵ��� ������ ��Ÿ�� ª����
    }
}
