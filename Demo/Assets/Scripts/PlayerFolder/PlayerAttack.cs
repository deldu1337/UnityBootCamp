using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackPower = 20f;
    public float attackRange = 1f;        // 근접 공격 범위
    public LayerMask enemyLayer;           // Enemy 레이어만 공격

    [Header("쿨타임")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    private bool isAutoAttacking = false;  // 자동 공격 상태
    private EnemyStats targetEnemy;        // 현재 공격 대상
    private HealthBarUI targetHealthBar;

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
                    Debug.Log("Auto Attack Started on " + hit.collider.name);
                }
            }
        }

        // 자동 공격 실행
        if (isAutoAttacking && targetEnemy != null && Time.time >= lastAttackTime)
        {
            // 플레이어와 적 사이 거리 체크
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distance <= attackRange && targetEnemy.currentHP > 0)
            {
                PerformAttack();
                lastAttackTime = Time.time + attackCooldown;
            }
            else if (targetEnemy.currentHP <= 0)
            {
                // 대상 사망 시 자동 공격 종료
                isAutoAttacking = false;
                targetEnemy = null;
                targetHealthBar = null;
                Debug.Log("Target is dead. Auto Attack stopped.");
            }
        }
    }

    void PerformAttack()
    {
        targetEnemy.TakeDamage(attackPower);
        Debug.Log($"Attacked {targetEnemy.name} for {attackPower} damage");

        if (targetHealthBar != null)
            targetHealthBar.CheckHp();
    }
}
