using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackSpeed = 2.3f;
    public float attackPower = 20f;
    public float attackRange = 2f;        // 근접 공격 범위
    public float raycastYOffset = 1f; // 기존 A보다 캡슐 콜라이더가 1 높으므로 1만큼 올림
    public LayerMask enemyLayer;           // Enemy 레이어만 공격

    [Header("쿨타임")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    private bool isAutoAttacking = false;  // 자동 공격 상태
    private EnemyStats targetEnemy;        // 현재 공격 대상
    private HealthBarUI targetHealthBar;
    private Animation animationComponent; // Animator 대신

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
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, enemyLayer))
            {
                targetEnemy = hit.collider.GetComponent<EnemyStats>();
                targetHealthBar = hit.collider.GetComponentInChildren<HealthBarUI>();
                if (targetEnemy != null)
                {
                    isAutoAttacking = true;
                    lastAttackTime = 0f; // 첫 타 바로 가능하게
                }
            }
        }

        if (isAutoAttacking)
        {
            // 타겟이 사망하면 자동 공격 종료 + 공격 모션 멈춤
            if (targetEnemy == null || targetEnemy.currentHP <= 0)
            {
                isAutoAttacking = false;
                targetEnemy = null;
                targetHealthBar = null;

                // 공격 중이면 스탠드 모션으로 전환
                if (animationComponent != null && animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                {
                    animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.1f);
                }

                return;
            }

            // 플레이어와 적 사이 거리 체크
            Vector3 playerAttackOrigin = transform.position + Vector3.up * raycastYOffset;
            float distance = Vector3.Distance(playerAttackOrigin, targetEnemy.transform.position);
            if (distance <= attackRange && Time.time >= lastAttackTime)
            {
                PerformAttack();
                lastAttackTime = Time.time + attackCooldown;
            }
        }
    }


    void PerformAttack()
    {
        if (targetEnemy == null)
        {
            Debug.LogWarning("TargetEnemy is null!");
            return;
        }
        if (animationComponent != null)
        {
            string animName = "Attack1H (ID 17 variation 0)";
            if (animationComponent.GetClip(animName) != null)
            {
                animationComponent[animName].speed = attackSpeed;
                animationComponent.CrossFade(animName, 0.1f); // 부드럽게 전환
            }
            else
            {
                Debug.LogError($"애니메이션 {animName}을 찾을 수 없습니다!");
            }
        }


        Debug.Log($"Before Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");
        targetEnemy.TakeDamage(attackPower);
        Debug.Log($"After Attack: {targetEnemy.name} HP={targetEnemy.currentHP}");

        if (targetHealthBar != null)
            targetHealthBar.CheckHp();
    }
}