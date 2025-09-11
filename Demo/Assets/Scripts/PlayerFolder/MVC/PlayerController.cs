using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;

    // 이벤트
    public event Action<float, float> OnHPChanged;
    public event Action<float, float> OnMPChanged;
    public event Action<int, float> OnExpChanged;
    public event Action<int> OnLevelUp;

    void Awake()
    {
        playerData = new PlayerData
        {
            Level = 1,
            Exp = 0,
            ExpToNextLevel = 50f,
            CurrentHP = 100f,
            CurrentMP = 50f,
            MaxHP = 100f,
            MaxMP = 50f,
            Atk = 5f,
            Def = 5f,
            Dex = 10f,
            AttackSpeed = 2f,
            CritChance = 0.1f,
            CritDamage = 1.5f
        };
    }

    // 장비 기반 최종 스텟 계산
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    {
        playerData.MaxHP = 100f;
        playerData.MaxMP = 50f;
        playerData.Atk = 5f;
        playerData.Def = 5f;
        playerData.Dex = 10f;
        playerData.AttackSpeed = 2f;
        playerData.CritChance = 0.1f;
        playerData.CritDamage = 1.5f;

        if (equippedSlots != null)
        {
            foreach (var slot in equippedSlots)
            {
                if (slot.equipped == null || slot.equipped.data == null) continue;
                var eq = slot.equipped.data;
                playerData.MaxHP += eq.hp;
                playerData.MaxMP += eq.mp;
                playerData.Atk += eq.atk;
                playerData.Def += eq.def;
                playerData.Dex += eq.dex;
                playerData.AttackSpeed += eq.As;
                playerData.CritChance += eq.cc;
                playerData.CritDamage += eq.cd;
            }
        }

        playerData.CurrentHP = Math.Min(playerData.CurrentHP, playerData.MaxHP);
        playerData.CurrentMP = Math.Min(playerData.CurrentMP, playerData.MaxMP);

        OnHPChanged?.Invoke(playerData.CurrentHP, playerData.MaxHP);
        OnMPChanged?.Invoke(playerData.CurrentMP, playerData.MaxMP);
    }

    // HP/MP 관리
    public void TakeDamage(float damage)
    {
        float finalDamage = Math.Max(damage - playerData.Def, 1f);
        playerData.CurrentHP -= finalDamage;
        playerData.CurrentHP = Math.Max(playerData.CurrentHP, 0);
        OnHPChanged?.Invoke(playerData.CurrentHP, playerData.MaxHP);

        if (playerData.CurrentHP <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        playerData.CurrentHP = Math.Min(playerData.CurrentHP + amount, playerData.MaxHP);
        OnHPChanged?.Invoke(playerData.CurrentHP, playerData.MaxHP);
    }

    public bool UseMana(float amount)
    {
        if (playerData.CurrentMP < amount) return false;
        playerData.CurrentMP -= amount;
        OnMPChanged?.Invoke(playerData.CurrentMP, playerData.MaxMP);
        return true;
    }

    public void RestoreMana(float amount)
    {
        playerData.CurrentMP = Math.Min(playerData.CurrentMP + amount, playerData.MaxMP);
        OnMPChanged?.Invoke(playerData.CurrentMP, playerData.MaxMP);
    }

    // 경험치 획득
    public void GainExp(float amount)
    {
        playerData.Exp += amount;
        OnExpChanged?.Invoke(playerData.Level, playerData.Exp);

        while (playerData.Exp >= playerData.ExpToNextLevel)
        {
            playerData.Exp -= playerData.ExpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerData.Level++;
        playerData.ExpToNextLevel = 50f * playerData.Level; // 예시
        RecalculateStats(null); // 필요 시 스탯 보정
        OnLevelUp?.Invoke(playerData.Level);
        Debug.Log($"레벨업! 현재 레벨 {playerData.Level}");
    }

    // 데미지 계산
    public float CalculateDamage()
    {
        float damage = playerData.Atk;
        if (UnityEngine.Random.value <= playerData.CritChance)
        {
            damage *= playerData.CritDamage;
            Debug.Log($"치명타! {damage} 데미지");
        }
        return damage;
    }

    private void Die() => Debug.Log("Player Died!");

    // JSON 저장/로드
    public void SaveToJson(string path)
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path, json);
    }

    public void LoadFromJson(string path)
    {
        if (!File.Exists(path)) return;
        string json = File.ReadAllText(path);
        playerData = JsonUtility.FromJson<PlayerData>(json);
        OnHPChanged?.Invoke(playerData.CurrentHP, playerData.MaxHP);
        OnMPChanged?.Invoke(playerData.CurrentMP, playerData.MaxMP);
        OnExpChanged?.Invoke(playerData.Level, playerData.Exp);
        OnLevelUp?.Invoke(playerData.Level);
    }
}
