using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [Header("�⺻ ����")]
    private float baseMaxHP = 100f;
    private float baseMaxMP = 50f;
    private float baseAtk = 5f;
    private float baseDef = 5f;
    private float baseDex = 10f;
    private float baseAttackSpeed = 2f;
    private float baseCritChance = 0.1f;
    private float baseCritDamage = 1.5f;

    [Header("���� ����")]
    public float currentHP { get; private set; }
    public float currentMP { get; private set; }

    // ���� ����
    public float MaxHP { get; private set; }
    public float MaxMP { get; private set; }
    public float Atk { get; private set; }
    public float Def { get; private set; }
    public float Dex { get; private set; }
    public float AttackSpeed { get; private set; }
    public float CritChance { get; private set; }
    public float CritDamage { get; private set; }

    void Awake()
    {
        RecalculateStats(null); // �ʱ�ȭ
        currentHP = MaxHP;
        currentMP = MaxMP;
    }

    /// <summary>
    /// ��� ������� ���� ���� ����
    /// </summary>
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    {
        // �⺻ ���� �ʱ�ȭ
        MaxHP = baseMaxHP;
        MaxMP = baseMaxMP;
        Atk = baseAtk;
        Def = baseDef;
        Dex = baseDex;
        AttackSpeed = baseAttackSpeed;
        CritChance = baseCritChance;
        CritDamage = baseCritDamage;

        if (equippedSlots == null) return;

        // ���� ���� ��� ���� �ջ�
        foreach (var slot in equippedSlots)
        {
            if (slot.equipped == null || slot.equipped.data == null) continue;
            var eq = slot.equipped.data;

            MaxHP += eq.hp;
            MaxMP += eq.mp;
            Atk += eq.atk;
            Def += eq.def;
            Dex += eq.dex;
            AttackSpeed += eq.As;
            CritChance += eq.cc;
            CritDamage += eq.cd;
        }

        // ���� HP/MP�� Max���� ũ�� ����
        currentHP = Mathf.Min(currentHP, MaxHP);
        currentMP = Mathf.Min(currentMP, MaxMP);
    }

    // === HP/MP ���� ===
    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - Def, 1f);
        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log($"���� {finalDamage} �� HP {currentHP}/{MaxHP}");

        if (currentHP <= 0)
            Die();
    }

    public void Heal(float amount) => currentHP = Mathf.Min(currentHP + amount, MaxHP);
    public bool UseMana(float amount)
    {
        if (currentMP < amount) return false;
        currentMP -= amount;
        return true;
    }
    public void RestoreMana(float amount) => currentMP = Mathf.Min(currentMP + amount, MaxMP);

    // === ���� ���� ===
    public float CalculateDamage()
    {
        float damage = Atk;
        if (Random.value <= CritChance)
        {
            damage *= CritDamage;
            Debug.Log($" ġ��Ÿ! {damage} ������");
        }
        return damage;
    }

    private void Die() => Debug.Log("Player Died!");
}
