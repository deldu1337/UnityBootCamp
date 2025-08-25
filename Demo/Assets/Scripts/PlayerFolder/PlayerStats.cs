using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP { get; private set; }


    public float attackPower = 20f;

    void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary> �÷��̾ �������� ���� </summary>
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log($"Player HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary> �÷��̾� ��� ó�� </summary>
    private void Die()
    {
        Debug.Log("Player Died!");
        // ��� ó�� ���� �߰� ���� (���� ����, ������ ��)
    }

    /// <summary> HP ȸ�� </summary>
    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }
}
