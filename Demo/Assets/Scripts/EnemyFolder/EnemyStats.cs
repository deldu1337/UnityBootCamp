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

    /// <summary> 적이 데미지를 받음 </summary>
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

    /// <summary> 적 사망 처리 </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} Died!");
        // 사망 처리 로직 추가 가능
        // 예: DropItem(), Destroy(gameObject), Animation Trigger 등
        Destroy(gameObject);
    }

    /// <summary> HP 회복 </summary>
    public void Heal(float amount)
    {
        if (currentHP <= 0) return; // 이미 죽은 경우 회복 불가
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }
}
