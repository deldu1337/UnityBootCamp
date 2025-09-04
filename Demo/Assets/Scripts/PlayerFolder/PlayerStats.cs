using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHP = 100f;          // �÷��̾� �ִ� ü��
    public float currentHP { get; private set; } // ���� ü�� (�ܺο����� �б⸸ ����)

    public float attackPower = 20f;     // �÷��̾� ���ݷ�

    void Awake()
    {
        // ���� ���� �� ���� ü���� �ִ� ü������ �ʱ�ȭ
        currentHP = maxHP;
    }

    // �÷��̾ ���ظ� ���� �� ȣ��
    public void TakeDamage(float damage)
    {
        currentHP -= damage;                         // ü�� ����
        currentHP = Mathf.Max(currentHP, 0);         // ü���� 0 �̸����� �������� �ʵ��� ����
        Debug.Log($"Player HP: {currentHP}/{maxHP}"); // ���� ü�� ���

        if (currentHP <= 0)
        {
            Die(); // ü���� 0�̸� ��� ó��
        }
    }

    // �÷��̾� ��� ó��
    private void Die()
    {
        Debug.Log("Player Died!"); // ��� �α� ���
        // TODO: ���� ����, ������ �� ��� ���� ���� �߰� ����
    }
    
    // �÷��̾� ü�� ȸ��
    public void Heal(float amount)
    {
        currentHP += amount;                     // ü�� ����
        currentHP = Mathf.Min(currentHP, maxHP); // �ִ� ü�� �ʰ����� �ʵ��� ����
    }
}