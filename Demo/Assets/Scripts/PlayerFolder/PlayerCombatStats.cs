using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float maxMP = 50f;
    [SerializeField] private float atk = 20f;        // 공격력
    [SerializeField] private float def = 5f;         // 방어력
    [SerializeField] private float dex = 25f;         // 민첩성 (이동 속도)
    [SerializeField] private float attackSpeed = 5f; // 공격 속도
    [SerializeField] private float critChance = 0.1f; // 치명타 확률 (0~1)
    [SerializeField] private float critDamage = 1.5f; // 치명타 배율 (1.5 = 150%)

    [Header("현재 상태")]
    public float currentHP { get; private set; }
    public float currentMP { get; private set; }

    void Awake()
    {
        // 게임 시작 시 HP/MP 풀로 초기화
        currentHP = maxHP;
        currentMP = maxMP;
    }

    // === 외부에서 읽기 위한 프로퍼티 ===
    public float MaxHP => maxHP;
    public float MaxMP => maxMP;
    public float Atk => atk;
    public float Def => def;
    public float Dex => dex;
    public float AttackSpeed => attackSpeed;
    public float CritChance => critChance;
    public float CritDamage => critDamage;

    // === HP/MP 관리 ===
    public void TakeDamage(float damage)
    {
        // 방어력 적용
        float finalDamage = Mathf.Max(damage - def, 1f);
        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log($"피해 {finalDamage} → HP {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public bool UseMana(float amount)
    {
        if (currentHP < amount) return false;
        currentHP -= amount;
        return true;
    }

    public void RestoreMana(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxMP);
    }

    // === 전투 관련 ===
    public float CalculateDamage()
    {
        float damage = atk;

        // 치명타 판정
        if (Random.value <= critChance)
        {
            damage *= critDamage;
            Debug.Log($" 치명타! {damage} 데미지");
        }

        return damage;
    }

    // === 사망 처리 ===
    private void Die()
    {
        Debug.Log("Player Died!");
        // TODO: 게임 오버, 리스폰 등 추가 로직
    }
}
