using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IHealth
{
    public PlayerData Data { get; private set; }
    private ILevelUpStrategy levelUpStrategy;

    public float CurrentHP => Data.CurrentHP;
    public float MaxHP => Data.MaxHP;

    public event Action<float, float> OnHPChanged;
    public event Action<float, float> OnMPChanged;
    public event Action<int, float> OnExpChanged;
    public event Action<int> OnLevelUp;

    //void Awake()
    //{
    //    // PlayerData.json�� ���� ��� �⺻�� ����
    //    Data = new PlayerData
    //    {
    //        Level = 1,
    //        Exp = 0,
    //        ExpToNextLevel = 50f,
    //        MaxHP = 100f,
    //        MaxMP = 50f,
    //        Atk = 5f,
    //        Def = 5f,
    //        Dex = 10f,
    //        AttackSpeed = 2f,
    //        CritChance = 0.1f,
    //        CritDamage = 1.5f,
    //        CurrentHP = 100f,
    //        CurrentMP = 50f
    //    };

    //    levelUpStrategy = new DefaultLevelUpStrategy();
    //}

    //public void LoadData(PlayerData loaded)
    //{
    //    if (loaded != null)
    //    {
    //        Data = loaded;
    //    }
    //    else
    //    {
    //        // json�� ���� ��� �⺻������ ����
    //        Data.CurrentHP = Data.MaxHP;
    //        Data.CurrentMP = Data.MaxMP;
    //    }

    //    UpdateUI();
    //}

    ////public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    ////{
    ////    // �⺻ ���� �ʱ�ȭ
    ////    float baseHP = 100f, baseMP = 50f, baseAtk = 5f, baseDef = 5f, baseDex = 10f;
    ////    float baseAS = 2f, baseCC = 0.1f, baseCD = 1.5f;

    ////    Data.MaxHP = baseHP;
    ////    Data.MaxMP = baseMP;
    ////    Data.Atk = baseAtk;
    ////    Data.Def = baseDef;
    ////    Data.Dex = baseDex;
    ////    Data.AttackSpeed = baseAS;
    ////    Data.CritChance = baseCC;
    ////    Data.CritDamage = baseCD;

    ////    // ��� ���� �ݿ�
    ////    if (equippedSlots != null)
    ////    {
    ////        foreach (var slot in equippedSlots)
    ////        {
    ////            if (slot.equipped == null || slot.equipped.data == null) continue;
    ////            var eq = slot.equipped.data;
    ////            Data.MaxHP += eq.hp;
    ////            Data.MaxMP += eq.mp;
    ////            Data.Atk += eq.atk;
    ////            Data.Def += eq.def;
    ////            Data.Dex += eq.dex;
    ////            Data.AttackSpeed += eq.As;
    ////            Data.CritChance += eq.cc;
    ////            Data.CritDamage += eq.cd;
    ////        }
    ////    }

    ////    // HP/MP ����
    ////    if (Data.CurrentHP <= 0)
    ////        Data.CurrentHP = Data.MaxHP;
    ////    else
    ////        Data.CurrentHP = Mathf.Min(Data.CurrentHP, Data.MaxHP);

    ////    Data.CurrentMP = Mathf.Min(Data.CurrentMP, Data.MaxMP);

    ////    SaveLoadManager.SavePlayerData(Data);
    ////    UpdateUI();
    ////}
    //public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots, bool isNewGame = false)
    //{
    //    // ����� HP/MP ���밪 ����
    //    float prevHP = Data.CurrentHP;
    //    float prevMP = Data.CurrentMP;

    //    // �⺻ ���� �ʱ�ȭ
    //    float baseHP = 100f, baseMP = 50f, baseAtk = 5f, baseDef = 5f, baseDex = 10f;
    //    float baseAS = 2f, baseCC = 0.1f, baseCD = 1.5f;

    //    Data.MaxHP = baseHP;
    //    Data.MaxMP = baseMP;
    //    Data.Atk = baseAtk;
    //    Data.Def = baseDef;
    //    Data.Dex = baseDex;
    //    Data.AttackSpeed = baseAS;
    //    Data.CritChance = baseCC;
    //    Data.CritDamage = baseCD;

    //    // ��� ���� ����
    //    if (equippedSlots != null)
    //    {
    //        foreach (var slot in equippedSlots)
    //        {
    //            if (slot.equipped == null || slot.equipped.data == null) continue;
    //            var eq = slot.equipped.data;
    //            Data.MaxHP += eq.hp;
    //            Data.MaxMP += eq.mp;
    //            Data.Atk += eq.atk;
    //            Data.Def += eq.def;
    //            Data.Dex += eq.dex;
    //            Data.AttackSpeed += eq.As;
    //            Data.CritChance += eq.cc;
    //            Data.CritDamage += eq.cd;
    //        }
    //    }

    //    // �� ������ ���� Ǯ�� ����
    //    if (isNewGame)
    //    {
    //        Data.CurrentHP = Data.MaxHP;
    //        Data.CurrentMP = Data.MaxMP;
    //    }
    //    else
    //    {
    //        // ���� ���尪 ����, �� MaxHP �Ѿ�� ����
    //        Data.CurrentHP = Mathf.Clamp(prevHP, 0, Data.MaxHP);
    //        Data.CurrentMP = Mathf.Clamp(prevMP, 0, Data.MaxMP);
    //    }

    //    SaveLoadManager.SavePlayerData(Data);
    //    UpdateUI();
    //}
    void Awake()
    {
        levelUpStrategy = new DefaultLevelUpStrategy();
        // ���⼭ �ʱⰪ ����
        // LoadData���� �����ϰ� �ٲ�
    }

    /// <summary>����� ������ �ҷ�����</summary>
    public void LoadData(PlayerData loaded)
    {
        if (loaded != null)
        {
            Data = loaded; // ����� ������ ���
        }
        else
        {
            // json�� ������ �⺻�� ����
            Data = new PlayerData
            {
                Level = 1,
                Exp = 0,
                ExpToNextLevel = 50f,
                MaxHP = 100f,
                MaxMP = 50f,
                Atk = 5f,
                Def = 5f,
                Dex = 10f,
                AttackSpeed = 2f,
                CritChance = 0.1f,
                CritDamage = 1.5f,
                CurrentHP = 100f,
                CurrentMP = 50f
            };
        }

        UpdateUI();
    }

    /// <summary>��� ������� MaxHP�� ���</summary>
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots, bool isNewGame = false)
    {
        float prevHP = Data.CurrentHP;
        float prevMP = Data.CurrentMP;

        // �⺻��
        float baseHP = 100f, baseMP = 50f, baseAtk = 5f, baseDef = 5f, baseDex = 10f;
        float baseAS = 2f, baseCC = 0.1f, baseCD = 1.5f;

        Data.MaxHP = baseHP;
        Data.MaxMP = baseMP;
        Data.Atk = baseAtk;
        Data.Def = baseDef;
        Data.Dex = baseDex;
        Data.AttackSpeed = baseAS;
        Data.CritChance = baseCC;
        Data.CritDamage = baseCD;

        // ��� ���� ����
        if (equippedSlots != null)
        {
            foreach (var slot in equippedSlots)
            {
                if (slot.equipped == null || slot.equipped.data == null) continue;
                var eq = slot.equipped.data;
                Data.MaxHP += eq.hp;
                Data.MaxMP += eq.mp;
                Data.Atk += eq.atk;
                Data.Def += eq.def;
                Data.Dex += eq.dex;
                Data.AttackSpeed += eq.As;
                Data.CritChance += eq.cc;
                Data.CritDamage += eq.cd;
            }
        }

        // HP/MP�� ���� ���� (���� ���� ���� ����)
        if (isNewGame)
        {
            Data.CurrentHP = Data.MaxHP;
            Data.CurrentMP = Data.MaxMP;
        }
        else
        {
            Data.CurrentHP = Mathf.Clamp(prevHP, 0, Data.MaxHP);
            Data.CurrentMP = Mathf.Clamp(prevMP, 0, Data.MaxMP);
        }

        SaveLoadManager.SavePlayerData(Data);
        UpdateUI();
    }


    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - Data.Def, 1f);
        Data.CurrentHP = Mathf.Max(Data.CurrentHP - finalDamage, 0);
        SaveLoadManager.SavePlayerData(Data);
        UpdateUI();

        if (Data.CurrentHP <= 0)
            Debug.Log("Player Died!");
    }

    public void Heal(float amount)
    {
        Data.CurrentHP = Mathf.Min(Data.CurrentHP + amount, Data.MaxHP);
        SaveLoadManager.SavePlayerData(Data);
        UpdateUI();
    }

    public bool UseMana(float amount)
    {
        if (Data.CurrentMP < amount) return false;
        Data.CurrentMP -= amount;
        SaveLoadManager.SavePlayerData(Data);
        UpdateUI();
        return true;
    }

    public void RestoreMana(float amount)
    {
        Data.CurrentMP = Mathf.Min(Data.CurrentMP + amount, Data.MaxMP);
        SaveLoadManager.SavePlayerData(Data);
        UpdateUI();
    }

    public void GainExp(float amount)
    {
        Data.Exp += amount;
        while (Data.Exp >= Data.ExpToNextLevel)
        {
            Data.Exp -= Data.ExpToNextLevel;
            LevelUp();
        }
        SaveLoadManager.SavePlayerData(Data);
        UpdateUI();
    }

    private void LevelUp()
    {
        levelUpStrategy?.ApplyLevelUp(Data);
        Debug.Log($"������! ���� ���� {Data.Level}");
    }

    public float CalculateDamage()
    {
        float damage = Data.Atk;
        if (UnityEngine.Random.value <= Data.CritChance)
        {
            damage *= Data.CritDamage;
            Debug.Log($"ġ��Ÿ! {damage} ������");
        }
        return damage;
    }

    private void UpdateUI()
    {
        OnHPChanged?.Invoke(Data.CurrentHP, Data.MaxHP);
        OnMPChanged?.Invoke(Data.CurrentMP, Data.MaxMP);
        OnExpChanged?.Invoke(Data.Level, Data.Exp);
        OnLevelUp?.Invoke(Data.Level);
    }
}
