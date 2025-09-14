using UnityEngine;

[RequireComponent(typeof(EnemyMove))]
public class EnemyAttack : MonoBehaviour
{
    private EnemyStatsManager stats;
    private EnemyMove enemyMove;
    private PlayerStatsManager targetPlayer;

    private float lastAttackTime;

    [Header("���� ���� ����")]
    public float attackRange = 2f;

    void Awake()
    {
        stats = GetComponent<EnemyStatsManager>();
        enemyMove = GetComponent<EnemyMove>();
    }

    void Update()
    {
        if (enemyMove.TargetPlayer != null)
        {
            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStatsManager>();
            if (targetPlayer != null && targetPlayer.Data.CurrentHP > 0)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
                if (distance <= attackRange && Time.time >= lastAttackTime)
                {
                    PerformAttack();
                    lastAttackTime = Time.time + 1f / stats.Data.As; // ���ݼӵ� �ݿ�
                }
            }
        }
    }

    private void PerformAttack()
    {
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(stats.Data.atk);
            Debug.Log($"{stats.Data.name}�� {targetPlayer.name}�� ���� ({stats.Data.atk} ������)");
        }
    }
}
