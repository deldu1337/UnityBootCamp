using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHP = 50f;             // ���� �ִ� ü��
    public float currentHP { get; private set; } // ���� ü�� (�ܺο��� �б⸸ ����)

    public float attackPower = 10f;       // �� ���ݷ�
    private ItemDropManager dropManager;   // ���� ���� �� ������ ��� ����

    void Awake()
    {
        currentHP = maxHP;                // �ʱ� ü���� �ִ� ü������ ����
        dropManager = GetComponent<ItemDropManager>(); // ��� �Ŵ��� ��������
    }

    // ���� �������� ����
    public void TakeDamage(float damage)
    {
        currentHP -= damage;               // ü�� ����
        currentHP = Mathf.Max(currentHP, 0); // ü���� 0 �Ʒ��� �������� �ʵ��� ����
        Debug.Log($"{gameObject.name} HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();                         // ü���� 0�̸� ��� ó��
            dropManager.DropItems();       // ��� �� ������ ���
        }
    }

    // �� ��� ó��
    private void Die()
    {
        Debug.Log($"{gameObject.name} Died!");
        // ��� ó�� ���� �߰� ����
        // ��: DropItem(), Destroy(gameObject), Animation Trigger ��
        Destroy(gameObject);               // ������Ʈ ����
    }

    // HP ȸ��
    public void Heal(float amount)
    {
        if (currentHP <= 0) return;       // �̹� ���� ��� ȸ�� �Ұ�
        currentHP += amount;               // ü�� ȸ��
        currentHP = Mathf.Min(currentHP, maxHP); // �ִ� ü�� ����
    }
}