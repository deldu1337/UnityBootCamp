using UnityEngine;

[RequireComponent(typeof(EnemyMove))]
public class EnemyAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackPower = 5f;
    public float attackRange = 2f;       // 근접 공격 범위
    public float attackCooldown = 1.5f;  // 공격 간격

    private float lastAttackTime = 0f;
    private EnemyMove enemyMove;
    private PlayerStats targetPlayer;

    void Awake()
    {
        enemyMove = GetComponent<EnemyMove>();
        if (enemyMove == null)
            Debug.LogError("EnemyMove 컴포넌트가 필요합니다.");
    }

    void Update()
    {
        // EnemyMove가 추적하는 플레이어가 있는지 확인
        if (enemyMove.TargetPlayer != null)
        {
            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStats>();

            if (targetPlayer != null && targetPlayer.currentHP > 0)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

                // 근접 범위 내일 때 공격
                if (distance <= attackRange && Time.time >= lastAttackTime)
                {
                    PerformAttack();
                    lastAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }

    private void PerformAttack()
    {
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(attackPower);
            Debug.Log($"{name} attacked {targetPlayer.name} for {attackPower} damage");
        }
    }
}
