using UnityEngine;

// 상태 인터페이스
public interface IPlayerStates
{
    void Enter(PlayerAttacks player);
    void Exit(PlayerAttacks player);
    void Update(PlayerAttacks player);
}

// Idle 상태 (대기)
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
        // 마우스 클릭 시 타겟 지정
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

// Attacking 상태
public class AttackingStates : IPlayerStates
{
    public void Enter(PlayerAttacks player)
    {
        player.lastAttackTime = 0f; // 즉시 공격 가능
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

        // 타겟 방향 회전
        player.RotateTowardsTarget(player.targetEnemy.transform.position);

        // 공격 쿨타임 체크
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

        // 마우스 클릭 시 타겟 해제
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

// PlayerAttack 클래스
public class PlayerAttacks : MonoBehaviour
{
    [Header("공격 설정")]
    public float raycastYOffset = 1f;
    public LayerMask enemyLayer;

    [Header("쿨타임")]
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
            Debug.LogError("Animation 컴포넌트가 Player 프리팹 또는 자식에 없습니다!");

        if (stats == null)
            Debug.LogError("PlayerCombatStats 컴포넌트가 Player에 없습니다!");
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
            // 애니메이션 길이
            float animLength = animationComponent.GetClip(animName).length;

            // 공격 속도 적용
            float speed = stats.AttackSpeed;
            animationComponent[animName].speed = speed * 1.2f;
            animationComponent.Play(animName);

            // 애니메이션 길이에 따라 쿨타임 조정
            lastAttackTime = Time.time + (animLength / speed);
        }

        float damage = stats.CalculateDamage();

        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(damage);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        targetHealthBar?.CheckHp();
    }


    // === PlayerCombatStats에서 가져오는 헬퍼 ===
    public float GetAttackRange()
    {
        // 민첩(Dex)에 따라 공격 사거리를 보정할 수도 있음
        //return 1f + (stats.Dex * 0.1f);
        return 1f;
    }

    public float GetAttackCooldown()
    {
        return 1f / stats.AttackSpeed; // 공격 속도가 높으면 쿨타임 짧아짐
    }
}
