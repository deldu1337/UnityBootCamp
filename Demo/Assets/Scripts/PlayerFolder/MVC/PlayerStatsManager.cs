using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IHealth
{
    public static PlayerStatsManager Instance { get; private set; }   // �� �߰�

    public PlayerData Data { get; private set; }
    private ILevelUpStrategy levelUpStrategy;

    public float CurrentHP => Data.CurrentHP;
    public float MaxHP => Data.MaxHP;

    public event Action<float, float> OnHPChanged;
    public event Action<float, float> OnMPChanged;
    public event Action<int, float> OnExpChanged;
    public event Action<int> OnLevelUp;

    void Awake()
    {
        // --- �̱��� ����: ���� ������ �÷��̾ �׻� �ֽ� Instance�� �ǵ��� ---
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); // ���� �÷��̾� ����
        }
        Instance = this;

        levelUpStrategy = new DefaultLevelUpStrategy();

        // ���� �ε�
        PlayerData loaded = SaveLoadService.LoadPlayerDataOrNull();
        LoadData(loaded);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        Debug.Log(Data.Level);
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
                Def = 0f,
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
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    {
        float prevHP = Data.CurrentHP;
        float prevMP = Data.CurrentMP;

        // �⺻��(������ �ݿ��� ��)���� �ʱ�ȭ
        // ����� JSON���� �ҷ��� ���� �̹� ���������� �ݿ��� ���̹Ƿ�,
        // "�⺻��"�� PlayerData�� MaxHP, MaxMP, Atk, Def ���� �����ؼ� ���
        float baseHP = 100f + (Data.Level - 1) * 10f; // ������ 10�� ���� (������ ������ ����)
        float baseMP = 50f;                           // ���������� MP�� ���ϴ� ������ ������ ����
        float baseAtk = 5f + (Data.Level - 1) * 2f;   // ������ 2�� ����
        float baseDef = 0f + (Data.Level - 1) * 0.5f;   // ������ 1�� ����
        float baseDex = 10f;                          // �⺻�� �״��
        float baseAS = 2f;                            // �⺻�� �״��
        float baseCC = 0.1f;
        float baseCD = 1.5f;

        // ��� ���� �ջ�
        float equipHP = 0f, equipMP = 0f, equipAtk = 0f, equipDef = 0f, equipDex = 0f;
        float equipAS = 0f, equipCC = 0f, equipCD = 0f;

        if (equippedSlots != null)
        {
            foreach (var slot in equippedSlots)
            {
                if (slot.equipped == null || slot.equipped.data == null) continue;
                var eq = slot.equipped.data;
                equipHP += eq.hp;
                equipMP += eq.mp;
                equipAtk += eq.atk;
                equipDef += eq.def;
                equipDex += eq.dex;
                equipAS += eq.As;
                equipCC += eq.cc;
                equipCD += eq.cd;
            }
        }

        // ���� ���� = �⺻�� + ���
        Data.MaxHP = baseHP + equipHP;
        Data.MaxMP = baseMP + equipMP;
        Data.Atk = baseAtk + equipAtk;
        Data.Def = baseDef + equipDef;
        Data.Dex = baseDex + equipDex;
        Data.AttackSpeed = baseAS + equipAS;
        Data.CritChance = baseCC + equipCC;
        Data.CritDamage = baseCD + equipCD;

        SaveLoadService.SavePlayerData(Data);
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - Data.Def, 1f);
        Data.CurrentHP = Mathf.Max(Data.CurrentHP - finalDamage, 0);
        SaveLoadService.SavePlayerData(Data);
        UpdateUI();

        if (Data.CurrentHP <= 0)
            Debug.Log("Player Died!");
    }

    public void Heal(float amount)
    {
        Data.CurrentHP = Mathf.Min(Data.CurrentHP + amount, Data.MaxHP);
        SaveLoadService.SavePlayerData(Data);
        UpdateUI();
    }

    public bool UseMana(float amount)
    {
        if (Data.CurrentMP < amount) return false;
        Data.CurrentMP -= amount;
        SaveLoadService.SavePlayerData(Data);
        UpdateUI();
        return true;
    }

    public void RestoreMana(float amount)
    {
        Data.CurrentMP = Mathf.Min(Data.CurrentMP + amount, Data.MaxMP);
        SaveLoadService.SavePlayerData(Data);
        UpdateUI();
    }

    public void GainExp(float amount)
    {
        Data.Exp += amount;
        Debug.Log($"���� EXP: {Data.Exp}/{Data.ExpToNextLevel}");

        // ������ ����
        while (Data.Exp >= Data.ExpToNextLevel)
        {
            Data.Exp -= Data.ExpToNextLevel; // ���� EXP ����
            LevelUp();
        }

        SaveLoadService.SavePlayerData(Data);
        UpdateUI();
    }

    private void LevelUp()
    {
        Data.Level++;
        // ������ �� �ʿ��� EXP ���� (���ϸ� � ���� ����)
        Data.ExpToNextLevel = Mathf.Round(Data.ExpToNextLevel * 1.2f);

        // ������ ���ʽ� (���ϴ´�� Ŀ���� ����)
        Data.MaxHP += 10f;
        Data.Atk += 2f;
        Data.Def += 1f;

        Data.CurrentHP = Data.MaxHP; // ������ �� Ǯ�� ȸ��
        Data.CurrentMP = Data.MaxMP;
        Debug.Log($"������! ���� ���� {Data.Level}");

        OnLevelUp?.Invoke(Data.Level);
    }

    public float CalculateDamage() // ���� �״�� ���� (ȣȯ��)
    {
        bool _;
        return CalculateDamage(out _);
    }

    // ġ��Ÿ ���θ� �Բ� ��ȯ�ϴ� �����ε�
    public float CalculateDamage(out bool isCrit)
    {
        float damage = Data.Atk;
        isCrit = UnityEngine.Random.value <= Data.CritChance;
        if (isCrit)
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
    }
}
