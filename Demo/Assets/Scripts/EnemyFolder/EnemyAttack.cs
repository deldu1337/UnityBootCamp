using UnityEngine;

[RequireComponent(typeof(EnemyMove))]
public class EnemyAttack : MonoBehaviour
{
    [Header("���� ����")]
    public float attackPower = 5f;
    public float attackRange = 2f;       // ���� ���� ����
    public float attackCooldown = 1.5f;  // ���� ����

    private float lastAttackTime = 0f;
    private EnemyMove enemyMove;
    private PlayerStats targetPlayer;

    void Awake()
    {
        enemyMove = GetComponent<EnemyMove>();
        if (enemyMove == null)
            Debug.LogError("EnemyMove ������Ʈ�� �ʿ��մϴ�.");
    }

    void Update()
    {
        // EnemyMove�� �����ϴ� �÷��̾ �ִ��� Ȯ��
        if (enemyMove.TargetPlayer != null)
        {
            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStats>();

            if (targetPlayer != null && targetPlayer.currentHP > 0)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

                // ���� ���� ���� �� ����
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
