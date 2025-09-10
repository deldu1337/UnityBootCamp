using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [Header("�⺻ ����")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float maxMP = 50f;
    [SerializeField] private float atk = 20f;        // ���ݷ�
    [SerializeField] private float def = 5f;         // ����
    [SerializeField] private float dex = 25f;         // ��ø�� (�̵� �ӵ�)
    [SerializeField] private float attackSpeed = 5f; // ���� �ӵ�
    [SerializeField] private float critChance = 0.1f; // ġ��Ÿ Ȯ�� (0~1)
    [SerializeField] private float critDamage = 1.5f; // ġ��Ÿ ���� (1.5 = 150%)

    [Header("���� ����")]
    public float currentHP { get; private set; }
    public float currentMP { get; private set; }

    void Awake()
    {
        // ���� ���� �� HP/MP Ǯ�� �ʱ�ȭ
        currentHP = maxHP;
        currentMP = maxMP;
    }

    // === �ܺο��� �б� ���� ������Ƽ ===
    public float MaxHP => maxHP;
    public float MaxMP => maxMP;
    public float Atk => atk;
    public float Def => def;
    public float Dex => dex;
    public float AttackSpeed => attackSpeed;
    public float CritChance => critChance;
    public float CritDamage => critDamage;

    // === HP/MP ���� ===
    public void TakeDamage(float damage)
    {
        // ���� ����
        float finalDamage = Mathf.Max(damage - def, 1f);
        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log($"���� {finalDamage} �� HP {currentHP}/{maxHP}");

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

    // === ���� ���� ===
    public float CalculateDamage()
    {
        float damage = atk;

        // ġ��Ÿ ����
        if (Random.value <= critChance)
        {
            damage *= critDamage;
            Debug.Log($" ġ��Ÿ! {damage} ������");
        }

        return damage;
    }

    // === ��� ó�� ===
    private void Die()
    {
        Debug.Log("Player Died!");
        // TODO: ���� ����, ������ �� �߰� ����
    }
}
