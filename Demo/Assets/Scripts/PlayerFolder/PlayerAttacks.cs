using System.Collections;
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
            player.animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.2f);
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

// Attacking 상태
public class AttackingStates : IPlayerStates
{
    public void Enter(PlayerAttacks player)
    {
        //player.lastAttackTime = 0f; // 즉시 공격 가능
        player.lastAttackTime = player.GetAttackCooldown(); // 즉시 공격 가능
    }

    public void Exit(PlayerAttacks player) { }

    public void Update(PlayerAttacks player)
    {
        bool targetDead = player.targetEnemy == null || player.targetEnemy.CurrentHP <= 0;

        if (!targetDead)
        {
            // 타겟 회전
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

        // 적이 죽었고 공격 애니메이션도 끝났다면 Idle 전환
        if (targetDead && !player.isAttacking)
        {
            player.ClearTarget();
            player.ChangeState(new IdleStates());
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

    [HideInInspector] public EnemyStatsManager targetEnemy;
    [HideInInspector] public HealthBarUI targetHealthBar;
    [HideInInspector] public Animation animationComponent;

    private IPlayerStates currentState;
    private PlayerStatsManager stats;

    [HideInInspector] public bool isAttacking = false; // 공격 중 여부

    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        stats = GetComponent<PlayerStatsManager>();

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
            // 공격 시작
            isAttacking = true;

            // 애니메이션 속도 적용
            animationComponent[animName].speed = stats.Data.AttackSpeed;
            animationComponent.Play(animName);

            // 공격 쿨타임
            lastAttackTime = Time.time + GetAttackCooldown();

            // 애니메이션 임팩트 시점에 데미지 적용
            float impactTime = 0.2f;
            StartCoroutine(DelayedDamage(impactTime));

            // 애니메이션 끝날 때 Idle로 전환
            float animDuration = animationComponent[animName].length / animationComponent[animName].speed;
            StartCoroutine(AttackAnimationEnd(animDuration));
        }
    }

    // 애니메이션 종료 후 처리
    private IEnumerator AttackAnimationEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    // 딜레이 후 데미지 적용
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


    // === PlayerCombatStats에서 가져오는 헬퍼 ===
    public float GetAttackRange()
    {
        return 1f;
    }

    public float GetAttackCooldown()
    {
        return 1f / stats.Data.AttackSpeed; // 공격 속도가 높으면 쿨타임 짧아짐
    }
}
