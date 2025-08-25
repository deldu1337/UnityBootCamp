using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("���� ����")]
    public float attackPower = 20f;
    public float attackRange = 1f;        // ���� ���� ����
    public LayerMask enemyLayer;           // Enemy ���̾ ����

    [Header("��Ÿ��")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    private bool isAutoAttacking = false;  // �ڵ� ���� ����
    private EnemyStats targetEnemy;        // ���� ���� ���
    private HealthBarUI targetHealthBar;

    void Update()
    {
        // ���콺 ������ Ŭ�� �� ��� ����
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

        // �ڵ� ���� ����
        if (isAutoAttacking && targetEnemy != null && Time.time >= lastAttackTime)
        {
            // �÷��̾�� �� ���� �Ÿ� üũ
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
            if (distance <= attackRange && targetEnemy.currentHP > 0)
            {
                PerformAttack();
                lastAttackTime = Time.time + attackCooldown;
            }
            else if (targetEnemy.currentHP <= 0)
            {
                // ��� ��� �� �ڵ� ���� ����
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
