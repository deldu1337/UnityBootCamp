using UnityEngine;

[RequireComponent(typeof(EnemyMove))] // EnemyMove ������Ʈ�� �ݵ�� �ʿ���
public class EnemyAttack : MonoBehaviour
{
    [Header("���� ����")]
    public float attackPower = 5f;        // �� ���ݷ�
    public float attackRange = 2f;        // ���� ���� ����
    public float attackCooldown = 1.5f;   // ���� ���� (��)

    private float lastAttackTime = 0f;    // ������ ���� �ð� ���
    private EnemyMove enemyMove;          // �̵� �� ���� ���� ������Ʈ
    private PlayerStats targetPlayer;     // ���� ��� �÷��̾� ����

    void Awake()
    {
        // EnemyMove ������Ʈ ��������
        enemyMove = GetComponent<EnemyMove>();
        if (enemyMove == null)
            Debug.LogError("EnemyMove ������Ʈ�� �ʿ��մϴ�.");
    }

    void Update()
    {
        // EnemyMove�� ���� ���� �÷��̾ �ִ��� Ȯ��
        if (enemyMove.TargetPlayer != null)
        {
            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStats>();

            // �÷��̾ �����ϰ� HP�� 0 �̻��̸� ���� ����
            if (targetPlayer != null && targetPlayer.currentHP > 0)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

                // ���� ���� ���̰� ��Ÿ���� �������� ���� ����
                if (distance <= attackRange && Time.time >= lastAttackTime)
                {
                    PerformAttack();                      // ���� ����
                    lastAttackTime = Time.time + attackCooldown; // ��Ÿ�� ����
                }
            }
        }
    }

    // ���� ���� ó��
    private void PerformAttack()
    {
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(attackPower); // �÷��̾� ü�� ����
            Debug.Log($"{name} attacked {targetPlayer.name} for {attackPower} damage"); // �α� ���
        }
    }
}