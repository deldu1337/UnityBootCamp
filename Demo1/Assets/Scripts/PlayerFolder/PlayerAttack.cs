using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackSpeed = 2.5f;       // 공격 애니메이션 속도
    public float attackPower = 20f;        // 공격력
    public float attackRange = 1f;         // 근접 공격 범위
    public float raycastYOffset = 1f;      // 공격 기준 높이 (캡슐 콜라이더 중심 기준)
    public LayerMask enemyLayer;           // 공격 가능한 레이어 (Enemy)

    [Header("쿨타임")]
    public float attackCooldown = 0.5f;    // 공격 간 최소 시간
    private float lastAttackTime = 0f;     // 마지막 공격 시점 기록

    private bool isAutoAttacking = false;  // 자동 공격 상태
    private EnemyStats targetEnemy;        // 현재 공격 대상
    private HealthBarUI targetHealthBar;   // 대상 체력바 UI
    private Animation animationComponent;  // Animator 대신 Unity Legacy Animation 사용

    // 현재 타겟 반환
    public EnemyStats GetCurrentTarget() => targetEnemy;

    // 타겟 설정 및 자동 공격 시작
    public void SetTarget(EnemyStats enemy)
    {
        targetEnemy = enemy;
        targetHealthBar = enemy?.GetComponentInChildren<HealthBarUI>();
        if (enemy != null)
        {
            isAutoAttacking = true;
            lastAttackTime = 0f; // 즉시 공격 가능
        }
    }

    // 초기화
    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 Player 프리팹 또는 자식에 없습니다!");
    }

    // 매 프레임 업데이트
    void Update()
    {
        // 마우스 오른쪽 클릭: 타겟 지정
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                EnemyStats clickedEnemy = hit.collider.GetComponent<EnemyStats>();

                // 클릭한 대상이 Enemy이고 enemyLayer에 속하면 타겟 지정
                if (clickedEnemy != null && ((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
                {
                    SetTarget(clickedEnemy); // 자동 공격 시작
                }
                else
                {
                    // 빈 공간 클릭 시 타겟 해제
                    isAutoAttacking = false;
                    targetEnemy = null;
                    targetHealthBar = null;

                    // 공격 중이면 기본 대기 애니메이션 재생
                    if (animationComponent != null)
                        animationComponent.Play("Stand (ID 0 variation 0)");
                }
            }
        }

        // 자동 공격 로직
        if (isAutoAttacking && targetEnemy != null && targetEnemy.currentHP > 0)
        {
            // 공격 쿨타임 확인
            if (Time.time >= lastAttackTime)
            {
                // 공격 범위 체크 (Collider 기반 거리 계산)
                Collider enemyCollider = targetEnemy.GetComponent<Collider>();
                Vector3 playerOrigin = transform.position + Vector3.up * raycastYOffset;
                Vector3 closest = enemyCollider.ClosestPoint(playerOrigin);
                float distance = Vector3.Distance(playerOrigin, closest);

                if (distance <= attackRange)
                {
                    PerformAttack();                     // 공격 수행
                    lastAttackTime = Time.time + attackCooldown; // 쿨타임 적용
                }
            }

            // 타겟 방향으로 회전
            RotateTowardsTarget(targetEnemy.transform.position);
        }
        else if (targetEnemy == null || targetEnemy.currentHP <= 0)
        {
            // 타겟이 없거나 죽으면 자동 공격 종료
            isAutoAttacking = false;
            targetEnemy = null;
            targetHealthBar = null;

            // 공격 애니메이션이 재생 중이면 기본 대기 애니메이션으로 변경
            if (animationComponent != null && animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // 타겟 방향으로 부드럽게 회전
    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // 수직 회전 제거
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    // 공격 수행
    void PerformAttack()
    {
        if (targetEnemy == null)
        {
            Debug.LogWarning("TargetEnemy is null!");
            return;
        }

        string animName = "Attack1H (ID 17 variation 0)";

        // 공격 애니메이션 재생
        if (animationComponent.GetClip(animName) != null)
        {
            animationComponent[animName].speed = attackSpeed;
            animationComponent.Play(animName); // 애니메이션 재생
        }
        else
        {
            Debug.LogError($"애니메이션 {animName}을 찾을 수 없습니다!");
        }

        // 공격 전/후 HP 로그 출력
        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(attackPower); // 실제 데미지 적용
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        // 체력바 갱신
        if (targetHealthBar != null)
            targetHealthBar.CheckHp();
    }
}