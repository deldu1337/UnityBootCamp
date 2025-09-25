using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IHealth
{
    public static PlayerStatsManager Instance { get; private set; }   // ← 추가

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
        // --- 싱글톤 보장: 새로 스폰된 플레이어가 항상 최신 Instance가 되도록 ---
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); // 이전 플레이어 제거
        }
        Instance = this;

        levelUpStrategy = new DefaultLevelUpStrategy();

        // 저장 로드
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

    /// <summary>저장된 데이터 불러오기</summary>
    public void LoadData(PlayerData loaded)
    {
        if (loaded != null)
        {
            Data = loaded; // 저장된 데이터 사용
        }
        else
        {
            // json이 없으면 기본값 세팅
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

    /// <summary>장비 기반으로 MaxHP만 계산</summary>
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    {
        float prevHP = Data.CurrentHP;
        float prevMP = Data.CurrentMP;

        // 기본값(레벨업 반영된 값)으로 초기화
        // 저장된 JSON에서 불러온 값이 이미 레벨업까지 반영된 값이므로,
        // "기본값"은 PlayerData의 MaxHP, MaxMP, Atk, Def 등을 복사해서 사용
        float baseHP = 100f + (Data.Level - 1) * 10f; // 레벨당 10씩 증가 (레벨업 로직과 맞춤)
        float baseMP = 50f;                           // 레벨업으로 MP가 변하는 로직이 있으면 수정
        float baseAtk = 5f + (Data.Level - 1) * 2f;   // 레벨당 2씩 증가
        float baseDef = 0f + (Data.Level - 1) * 0.5f;   // 레벨당 1씩 증가
        float baseDex = 10f;                          // 기본값 그대로
        float baseAS = 2f;                            // 기본값 그대로
        float baseCC = 0.1f;
        float baseCD = 1.5f;

        // 장비 스탯 합산
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

        // 최종 스탯 = 기본값 + 장비값
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
        Debug.Log($"현재 EXP: {Data.Exp}/{Data.ExpToNextLevel}");

        // 레벨업 루프
        while (Data.Exp >= Data.ExpToNextLevel)
        {
            Data.Exp -= Data.ExpToNextLevel; // 여분 EXP 유지
            LevelUp();
        }

        SaveLoadService.SavePlayerData(Data);
        UpdateUI();
    }

    private void LevelUp()
    {
        Data.Level++;
        // 레벨업 시 필요한 EXP 증가 (원하면 곡선 증가 가능)
        Data.ExpToNextLevel = Mathf.Round(Data.ExpToNextLevel * 1.2f);

        // 레벨업 보너스 (원하는대로 커스텀 가능)
        Data.MaxHP += 10f;
        Data.Atk += 2f;
        Data.Def += 1f;

        Data.CurrentHP = Data.MaxHP; // 레벨업 시 풀피 회복
        Data.CurrentMP = Data.MaxMP;
        Debug.Log($"레벨업! 현재 레벨 {Data.Level}");

        OnLevelUp?.Invoke(Data.Level);
    }

    public float CalculateDamage() // 기존 그대로 유지 (호환용)
    {
        bool _;
        return CalculateDamage(out _);
    }

    // 치명타 여부를 함께 반환하는 오버로드
    public float CalculateDamage(out bool isCrit)
    {
        float damage = Data.Atk;
        isCrit = UnityEngine.Random.value <= Data.CritChance;
        if (isCrit)
        {
            damage *= Data.CritDamage;
            Debug.Log($"치명타! {damage} 데미지");
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
