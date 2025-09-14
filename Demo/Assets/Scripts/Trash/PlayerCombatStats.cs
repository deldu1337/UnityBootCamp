using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [Header("기본 스탯")]
    private float baseMaxHP = 100f;
    private float baseMaxMP = 50f;
    private float baseAtk = 5f;
    private float baseDef = 5f;
    private float baseDex = 10f;
    private float baseAttackSpeed = 2f;
    private float baseCritChance = 0.1f;
    private float baseCritDamage = 1.5f;

    [Header("현재 상태")]
    public float currentHP { get; private set; }
    public float currentMP { get; private set; }

    // 최종 스탯
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
        RecalculateStats(null); // 초기화
        currentHP = MaxHP;
        currentMP = MaxMP;
    }

    /// <summary>
    /// 장비 기반으로 최종 스탯 재계산
    /// </summary>
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    {
        // 기본 스탯 초기화
        MaxHP = baseMaxHP;
        MaxMP = baseMaxMP;
        Atk = baseAtk;
        Def = baseDef;
        Dex = baseDex;
        AttackSpeed = baseAttackSpeed;
        CritChance = baseCritChance;
        CritDamage = baseCritDamage;

        if (equippedSlots == null) return;

        // 장착 중인 장비 스탯 합산
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

        // 현재 HP/MP가 Max보다 크면 보정
        currentHP = Mathf.Min(currentHP, MaxHP);
        currentMP = Mathf.Min(currentMP, MaxMP);
    }

    // === HP/MP 관리 ===
    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - Def, 1f);
        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log($"피해 {finalDamage} → HP {currentHP}/{MaxHP}");

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

    // === 전투 관련 ===
    public float CalculateDamage()
    {
        float damage = Atk;
        if (Random.value <= CritChance)
        {
            damage *= CritDamage;
            Debug.Log($" 치명타! {damage} 데미지");
        }
        return damage;
    }

    private void Die() => Debug.Log("Player Died!");
}
