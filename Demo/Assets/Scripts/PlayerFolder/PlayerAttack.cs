using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackSpeed = 2.5f;
    public float attackPower = 20f;
    public float attackRange = 1f;        // 근접 공격 범위
    public float raycastYOffset = 1f; // 기존 A보다 캡슐 콜라이더가 1 높으므로 1만큼 올림
    public LayerMask enemyLayer;           // Enemy 레이어만 공격

    [Header("쿨타임")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    private bool isAutoAttacking = false;  // 자동 공격 상태
    private EnemyStats targetEnemy;        // 현재 공격 대상
    private HealthBarUI targetHealthBar;
    private Animation animationComponent; // Animator 대신

    public EnemyStats GetCurrentTarget() => targetEnemy;

    public void SetTarget(EnemyStats enemy)
    {
        targetEnemy = enemy;
        targetHealthBar = enemy?.GetComponentInChildren<HealthBarUI>();
        if (enemy != null)
        {
            isAutoAttacking = true;
            lastAttackTime = 0f;
        }
    }


    void Awake()
    {
        animationComponent = GetComponent<Animation>();

        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 Player 프리팹 또는 자식에 없습니다!");
    }

    void Update()
    {
        // 마우스 오른쪽 클릭 시 대상 지정
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                EnemyStats clickedEnemy = hit.collider.GetComponent<EnemyStats>();
                if (clickedEnemy != null && ((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
                {
                    SetTarget(clickedEnemy); // 자동 공격 시작
                }
                else
                {
                    // 빈 공간 클릭 → 타겟 해제
                    isAutoAttacking = false;
                    targetEnemy = null;
                    targetHealthBar = null;

                    if (animationComponent != null)
                        animationComponent.Play("Stand (ID 0 variation 0)");
                }
            }
        }


        // 자동 공격
        if (isAutoAttacking && targetEnemy != null && targetEnemy.currentHP > 0)
        {
            // 공격 쿨타임 확인
            if (Time.time >= lastAttackTime)
            {
                // 공격 범위 체크
                Collider enemyCollider = targetEnemy.GetComponent<Collider>();
                Vector3 playerOrigin = transform.position + Vector3.up * raycastYOffset;
                Vector3 closest = enemyCollider.ClosestPoint(playerOrigin);
                float distance = Vector3.Distance(playerOrigin, closest);

                if (distance <= attackRange)
                {
                    PerformAttack();
                    lastAttackTime = Time.time + attackCooldown; // 쿨타임 적용
                }
            }

            // 타겟 방향 회전
            RotateTowardsTarget(targetEnemy.transform.position);
        }
        else if (targetEnemy == null || targetEnemy.currentHP <= 0)
        {
            // 타겟이 없거나 죽으면 자동 공격 종료
            isAutoAttacking = false;
            targetEnemy = null;
            targetHealthBar = null;

            if (animationComponent != null && animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }


    // 타겟 위치로 부드럽게 회전
    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // y축만 회전
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }


    void PerformAttack()
    {
        if (targetEnemy == null)
        {
            Debug.LogWarning("TargetEnemy is null!");
            return;
        }
        string animName = "Attack1H (ID 17 variation 0)";
        if (animationComponent.GetClip(animName) != null)
        {
            animationComponent[animName].speed = attackSpeed;
            animationComponent.Play(animName); // 부드럽게 전환
        }
        else
        {
            Debug.LogError($"애니메이션 {animName}을 찾을 수 없습니다!");
        }


        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(attackPower);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        if (targetHealthBar != null)
            targetHealthBar.CheckHp();
    }
}