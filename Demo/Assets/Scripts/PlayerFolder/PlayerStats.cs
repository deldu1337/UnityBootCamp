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

    /// <summary> 플레이어가 데미지를 받음 </summary>
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

    /// <summary> 플레이어 사망 처리 </summary>
    private void Die()
    {
        Debug.Log("Player Died!");
        // 사망 처리 로직 추가 가능 (게임 오버, 리스폰 등)
    }

    /// <summary> HP 회복 </summary>
    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }
}
