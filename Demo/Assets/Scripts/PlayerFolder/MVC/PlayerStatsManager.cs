using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerDataEntry : PlayerData
{
    public string race; // JSON에서 읽기용(대소문자 구분 없이 비교)
}

[Serializable]
public class PlayerDataCollection
{
    public PlayerDataEntry[] entries;
}

public class PlayerStatsManager : MonoBehaviour, IHealth
{
    public static PlayerStatsManager Instance { get; private set; }   // ← 추가

    // 전역 브로드캐스트 이벤트
    public static event Action OnPlayerDied;
    public static event Action OnPlayerDeathAnimFinished;
    public static event Action OnPlayerRevived;              

    [Header("Death/Revive Options")]
    public bool pauseEditorOnDeath = false;

    [Header("Pose Root (모델 루트)")]
    [Tooltip("플레이어 메시에 해당하는 모델 루트를 지정(비우면 이 오브젝트 자체를 사용)")]
    [SerializeField] private Transform poseRoot;

    // 죽음 1회 처리 가드
    private bool isDead = false;
    public bool IsDead => isDead;

    private PlayerSkeletonSnapshot lastAliveSnapshot;

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
    //    // --- 싱글톤 보장: 새로 스폰된 플레이어가 항상 최신 Instance가 되도록 ---
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(Instance.gameObject); // 이전 플레이어 제거
    //    }
    //    Instance = this;

    //    levelUpStrategy = new DefaultLevelUpStrategy();

    //    // 저장 로드
    //    PlayerData loaded = SaveLoadService.LoadPlayerDataOrNull();
    //    LoadData(loaded);
    //}
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        levelUpStrategy = new DefaultLevelUpStrategy();

        // (중요) 여기서는 바로 SaveLoad를 읽지 않고,
        // PlayerSpawn에서 프리팹 인스턴스 후 InitializeForSelectedRace()를 호출해 초기화한다.
        // 만약 PlayerSpawn이 호출 안 되는 특별 케이스를 대비해 아래 가드 추가:
        //if (Data == null)
        //{
        //    // 이어하기(씬 진입 직후) 등에서 PlayerSpawn 호출 전에
        //    // Instance가 먼저 살아날 수도 있으니, 세이브 있으면 임시로드
        //    var loaded = SaveLoadService.LoadPlayerDataForRaceOrNull(Data.Race);
        //    if (loaded != null)
        //    {
        //        LoadData(loaded);
        //    }
        //}
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (Data != null)
            Debug.Log(Data.Level);
    }

    /// <summary>
    /// PlayerSpawn이 프리팹 인스턴스 생성 직후 부르는 진입점.
    /// - 새 게임(캐릭터 선택 후)이라면 선택한 종족의 기본값을 리소스에서 로드
    /// - 이어하기(세이브 존재)라면 세이브를 우선, 단 세이브 종족 != 선택종족이면 선택종족 기본값으로 신규시작
    /// </summary>
    //public void InitializeForSelectedRace()
    //{
    //    string race = string.IsNullOrEmpty(GameContext.SelectedRace) ? "humanmale" : GameContext.SelectedRace;

    //    var saved = SaveLoadService.LoadPlayerDataOrNull();
    //    bool hasSave = (saved != null);

    //    if (GameContext.IsNewGame)
    //    {
    //        // 캐릭터 선택 직후 새게임: 선택한 종족의 기본값으로 시작
    //        LoadRaceData_FromSingleFile(race);
    //        GameContext.IsNewGame = false; // 초기화 완료
    //        SaveLoadService.SavePlayerData(Data); // 첫 저장
    //        return;
    //    }

    //    if (hasSave)
    //    {
    //        // 이어하기: 세이브가 있고, 세이브된 종족과 선택종족이 다르면 선택종족으로 새시작
    //        if (!string.IsNullOrEmpty(saved.Race) &&
    //            saved.Race.Equals(race, StringComparison.OrdinalIgnoreCase))
    //        {
    //            LoadData(saved);
    //        }
    //        else
    //        {
    //            LoadRaceData_FromSingleFile(race);
    //            SaveLoadService.SavePlayerData(Data);
    //        }
    //    }
    //    else
    //    {
    //        // 세이브가 없으면 선택종족 기본값
    //        LoadRaceData_FromSingleFile(race);
    //        SaveLoadService.SavePlayerData(Data);
    //    }
    //}
    public void InitializeForSelectedRace()
    {
        string race = string.IsNullOrEmpty(GameContext.SelectedRace) ? "humanmale" : GameContext.SelectedRace;

        // 0) 이어하기 경로: 해당 종족 세이브가 있으면 그걸 로드하고 끝
        var saved = SaveLoadService.LoadPlayerDataForRaceOrNull(race);
        if (!GameContext.IsNewGame && saved != null)
        {
            LoadData(saved);
            return;
        }

        // 1) 새 게임인데, 기존 세이브가 존재하는 경우 → 기본은 "덮지 않는다"
        if (GameContext.IsNewGame && saved != null && !GameContext.ForceReset)
        {
            // 사용자가 실수로 새 게임을 눌렀을 가능성 → 기존 저장을 존중
            LoadData(saved);
            return;
        }

        // 2) 여기 오면 (a) 새 게임 + 기존 없음, (b) 새 게임 + 강제 리셋, (c) 이어하기인데 저장이 없을 때
        LoadRaceData_FromSingleFile(race);
        Data.Race = race;
        SaveLoadService.SavePlayerDataForRace(race, Data); // 첫 저장 (또는 리셋 저장)

        // 새 게임 플래그 해제
        GameContext.IsNewGame = false;
        GameContext.ForceReset = false; // 있다면
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

    /// <summary>
    /// 하나의 JSON 파일(Entries 배열)에서 선택 종족 기본값 로드
    /// Resources/PlayerData/PlayerDataAll.json
    /// </summary>
    public void LoadRaceData_FromSingleFile(string raceName)
    {
        TextAsset json = Resources.Load<TextAsset>("PlayerData/PlayerDataAll");
        if (json == null)
        {
            Debug.LogError("Resources/PlayerData/PlayerDataAll.json 을 찾지 못했습니다.");
            LoadData(null); // 안전 fallback
            return;
        }

        var col = JsonUtility.FromJson<PlayerDataCollection>(json.text);
        if (col?.entries == null || col.entries.Length == 0)
        {
            Debug.LogError("PlayerDataAll.json: entries가 비어 있습니다.");
            LoadData(null);
            return;
        }

        foreach (var e in col.entries)
        {
            if (string.Equals(e.race, raceName, StringComparison.OrdinalIgnoreCase))
            {
                Data = new PlayerData
                {
                    Race = raceName,
                    MaxHP = e.MaxHP,
                    MaxMP = e.MaxMP,
                    Atk = e.Atk,
                    Def = e.Def,
                    Dex = e.Dex,
                    AttackSpeed = e.AttackSpeed,
                    CritChance = e.CritChance,
                    CritDamage = e.CritDamage,
                    CurrentHP = e.CurrentHP,
                    CurrentMP = e.CurrentMP,
                    Level = e.Level,
                    Exp = e.Exp,
                    ExpToNextLevel = e.ExpToNextLevel
                };
                UpdateUI();
                Debug.Log($"{raceName} 기본 스탯 로드 완료 (단일 파일).");
                return;
            }
        }

        Debug.LogError($"PlayerDataAll.json에 '{raceName}' 항목이 없습니다.");
        LoadData(null);
    }

    /// <summary>장비 기반으로 MaxHP만 계산</summary>
    //public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    //{
    //    float prevHP = Data.CurrentHP;
    //    float prevMP = Data.CurrentMP;

    //    // 기본값(레벨업 반영된 값)으로 초기화
    //    // 저장된 JSON에서 불러온 값이 이미 레벨업까지 반영된 값이므로,
    //    // "기본값"은 PlayerData의 MaxHP, MaxMP, Atk, Def 등을 복사해서 사용
    //    float baseHP = 100f + (Data.Level - 1) * 10f; // 레벨당 10씩 증가 (레벨업 로직과 맞춤)
    //    float baseMP = 50f;                           // 레벨업으로 MP가 변하는 로직이 있으면 수정
    //    float baseAtk = 5f + (Data.Level - 1) * 2f;   // 레벨당 2씩 증가
    //    float baseDef = 2f + (Data.Level - 1) * 0.5f;   // 레벨당 1씩 증가
    //    float baseDex = 10f;                          // 기본값 그대로
    //    float baseAS = 2f;                            // 기본값 그대로
    //    float baseCC = 0.1f;
    //    float baseCD = 1.5f;

    //    // 장비 스탯 합산
    //    float equipHP = 0f, equipMP = 0f, equipAtk = 0f, equipDef = 0f, equipDex = 0f;
    //    float equipAS = 0f, equipCC = 0f, equipCD = 0f;

    //    if (equippedSlots != null)
    //    {
    //        foreach (var slot in equippedSlots)
    //        {
    //            if (slot.equipped == null || slot.equipped.data == null || slot.equipped.rolled == null) continue;
    //            var eq = slot.equipped.rolled;
    //            equipHP += eq.hp;
    //            equipMP += eq.mp;
    //            equipAtk += eq.atk;
    //            equipDef += eq.def;
    //            equipDex += eq.dex;
    //            equipAS += eq.As;
    //            equipCC += eq.cc;
    //            equipCD += eq.cd;
    //        }
    //    }

    //    // 최종 스탯 = 기본값 + 장비값
    //    Data.MaxHP = baseHP + equipHP;
    //    Data.MaxMP = baseMP + equipMP;
    //    Data.Atk = baseAtk + equipAtk;
    //    Data.Def = baseDef + equipDef;
    //    Data.Dex = baseDex + equipDex;
    //    Data.AttackSpeed = baseAS + equipAS;
    //    Data.CritChance = baseCC + equipCC;
    //    Data.CritDamage = baseCD + equipCD;

    //    SaveLoadService.SavePlayerData(Data);
    //    UpdateUI();
    //}
    public void RecalculateStats(IReadOnlyList<EquipmentSlot> equippedSlots)
    {
        // 1) 베이스를 '현재 Data 값'에서 시작 (종족/레벨업 반영치)
        float baseHP = Data.MaxHP;
        float baseMP = Data.MaxMP;
        float baseAtk = Data.Atk;
        float baseDef = Data.Def;
        float baseDex = Data.Dex;
        float baseAS = Data.AttackSpeed;
        float baseCC = Data.CritChance;   // ★ 0.1f 고정 → Data.CritChance 로
        float baseCD = Data.CritDamage;

        // 2) 장비 보너스 합산
        float equipHP = 0, equipMP = 0, equipAtk = 0, equipDef = 0, equipDex = 0, equipAS = 0, equipCC = 0, equipCD = 0;
        if (equippedSlots != null)
        {
            foreach (var slot in equippedSlots)
            {
                if (slot.equipped == null || slot.equipped.data == null || slot.equipped.rolled == null) continue;
                var eq = slot.equipped.rolled;
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

        // 3) 최종치 = 베이스 + 장비
        Data.MaxHP = baseHP + equipHP;
        Data.MaxMP = baseMP + equipMP;
        Data.Atk = baseAtk + equipAtk;
        Data.Def = baseDef + equipDef;
        Data.Dex = baseDex + equipDex;
        Data.AttackSpeed = baseAS + equipAS;
        Data.CritChance = baseCC + equipCC;   // ★ 이제 0.2도 보존됨
        Data.CritDamage = baseCD + equipCD;

        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
        UpdateUI();
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return; // 이미 죽은 뒤엔 무시

        float finalDamage = Mathf.Max(damage - Data.Def, 1f);
        Data.CurrentHP = Mathf.Max(Data.CurrentHP - finalDamage, 0);
        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
        UpdateUI();

        if (Data.CurrentHP <= 0 && !isDead)
        {
            lastAliveSnapshot = PlayerSkeletonSnapshot.Capture(poseRoot, includeRootLocalTransform: false);
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        isDead = true;

        // 전역 알림: 모든 적이 즉시 반응 가능
        try { OnPlayerDied?.Invoke(); } catch (Exception e) { Debug.LogException(e); }

        // 이동/공격 등 플레이어 컨트롤러가 있다면 비활성화(선택)
        var move = GetComponent<PlayerMove>();
        if (move) move.enabled = false;
        var attacks = GetComponent<PlayerAttacks>();
        if (attacks) attacks.enabled = false;

        // 죽음 애니메이션 재생 후 에디터 일시정지
        StartCoroutine(PlayDeathAndPauseEditor());
    }

    private IEnumerator PlayDeathAndPauseEditor()
    {
        string deathAnim = "Death (ID 1 variation 0)";
        float duration = 0.7f; // 기본값(클립이 없을 때 대비)

        var anim = GetComponent<Animation>();
        if (anim && anim.GetClip(deathAnim))
        {
            var st = anim[deathAnim];
            st.wrapMode = WrapMode.Once;
            st.speed = 1f;
            anim.Stop();
            anim.Play(deathAnim);
            duration = st.length;
        }
        else
        {
            Debug.LogWarning($"[PlayerStatsManager] Death clip '{deathAnim}'을 찾지 못했습니다. 기본 대기시간({duration}s) 후 일시정지합니다.");
        }

        // 애니메이션이 끝날 때까지 대기
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // 애니 끝난 뒤 알림
        try { OnPlayerDeathAnimFinished?.Invoke(); } catch (Exception e) { Debug.LogException(e); }

        // 에디터에서만 일시정지
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPaused = true;
//#endif
    }

    /// <summary>
    /// 부활: revivePos/Rot 위치에서, 죽기 직전 스냅샷 포즈를 복원한 뒤 HP/MP 풀, EXP 0.
    /// </summary>
    public void ReviveAt(Vector3 reviveWorldPos, Quaternion reviveWorldRot)
    {
        if (!isDead) return;

        // 0) 위치/자세 먼저 설정
        transform.SetPositionAndRotation(reviveWorldPos, reviveWorldRot);

        // 1) 물리 초기화
        var rb = GetComponent<Rigidbody>();
        if (rb) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        // 2) (중요) 죽기 직전 포즈 스냅샷을 로컬 단위로 복원
        if (lastAliveSnapshot != null)
        {
            // 현재 reviveWorldPos/Rot 을 유지하면서, 포즈만 스냅샷으로
            lastAliveSnapshot.Apply(poseRoot, transform.position, transform.rotation);
        }

        // 3) 애니메이션 초기화 (잔상 방지)
        var anim = GetComponent<Animation>();
        if (anim)
        {
            anim.Stop();
            string idle = "Stand (ID 0 variation 0)";
            if (anim.GetClip(idle))
            {
                var st = anim[idle];
                st.wrapMode = WrapMode.Loop;
                st.time = 0f;
                anim.Play(idle); // 상태 갱신
                anim.Sample();   // 즉시 0프레임 적용
            }
        }

        // 4) 수치 회복
        Data.CurrentHP = Data.MaxHP;
        Data.CurrentMP = Data.MaxMP;
        Data.Exp = 0f;
        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
        UpdateUI();

        // 5) 컨트롤 재활성
        var move = GetComponent<PlayerMove>(); if (move) move.enabled = true;
        var attacks = GetComponent<PlayerAttacks>(); if (attacks) attacks.enabled = true;

        isDead = false;

        // 6) 스냅샷은 1회성으로 사용했으니 필요하면 파기(원한다면 유지 가능)
        lastAliveSnapshot = null;

        try { OnPlayerRevived?.Invoke(); } catch (Exception e) { Debug.LogException(e); }
    }

    public void Heal(float amount)
    {
        if (isDead) return; // 죽은 뒤엔 힐 무시 (원한다면 부활 로직 따로)
        Data.CurrentHP = Mathf.Min(Data.CurrentHP + amount, Data.MaxHP);
        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
        UpdateUI();
    }

    public bool UseMana(float amount)
    {
        if (Data.CurrentMP < amount) return false;
        Data.CurrentMP -= amount;
        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
        UpdateUI();
        return true;
    }

    public void RestoreMana(float amount)
    {
        Data.CurrentMP = Mathf.Min(Data.CurrentMP + amount, Data.MaxMP);
        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
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

        SaveLoadService.SavePlayerDataForRace(Data.Race, Data);
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
