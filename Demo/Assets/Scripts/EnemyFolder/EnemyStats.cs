using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHP = 50f;
    public float currentHP { get; private set; }

    public float attackPower = 10f;

    void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary> ���� �������� ���� </summary>
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log($"{gameObject.name} HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary> �� ��� ó�� </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} Died!");
        // ��� ó�� ���� �߰� ����
        // ��: DropItem(), Destroy(gameObject), Animation Trigger ��
        Destroy(gameObject);
    }

    /// <summary> HP ȸ�� </summary>
    public void Heal(float amount)
    {
        if (currentHP <= 0) return; // �̹� ���� ��� ȸ�� �Ұ�
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }
}
