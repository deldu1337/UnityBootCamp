using UnityEngine;

// ���� �������̽�
public interface IPlayerState
{
    void Enter(PlayerAttack player);
    void Exit(PlayerAttack player);
    void Update(PlayerAttack player);
}

// Idle ���� (���)
public class IdleState : IPlayerState
{
    public void Enter(PlayerAttack player)
    {
        if (player.animationComponent != null)
            player.animationComponent.Play("Stand (ID 0 variation 0)");
    }

    public void Exit(PlayerAttack player) { }

    public void Update(PlayerAttack player)
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
                    player.ChangeState(new AttackingState());
                }
            }
        }
    }
}

// Attacking ����
public class AttackingState : IPlayerState
{
    public void Enter(PlayerAttack player)
    {
        player.lastAttackTime = 0f; // ��� ���� ����
    }

    public void Exit(PlayerAttack player) { }

    public void Update(PlayerAttack player)
    {
        if (player.targetEnemy == null || player.targetEnemy.currentHP <= 0)
        {
            player.ClearTarget();
            player.ChangeState(new IdleState());
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

            if (distance <= player.attackRange)
            {
                player.PerformAttack();
                player.lastAttackTime = Time.time + player.attackCooldown;
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
                    player.ChangeState(new IdleState());
                }
            }
        }
    }
}

// PlayerAttack Ŭ����
public class PlayerAttack : MonoBehaviour
{
    [Header("���� ����")]
    public float attackSpeed = 2.5f;
    public float attackPower = 20f;
    public float attackRange = 1f;
    public float raycastYOffset = 1f;
    public LayerMask enemyLayer;

    [Header("��Ÿ��")]
    public float attackCooldown = 0.5f;
    [HideInInspector] public float lastAttackTime;

    [HideInInspector] public EnemyStats targetEnemy;
    [HideInInspector] public HealthBarUI targetHealthBar;
    [HideInInspector] public Animation animationComponent;

    private IPlayerState currentState;

    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� Player ������ �Ǵ� �ڽĿ� �����ϴ�!");
    }

    void Start()
    {
        ChangeState(new IdleState());
    }

    void Update()
    {
        currentState?.Update(this);
    }

    public void ChangeState(IPlayerState newState)
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
            animationComponent[animName].speed = attackSpeed;
            animationComponent.Play(animName);
        }

        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(attackPower);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        targetHealthBar?.CheckHp();
    }
}
