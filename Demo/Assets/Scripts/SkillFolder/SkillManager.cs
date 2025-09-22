//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;

//public class SkillManager : MonoBehaviour
//{
//    [SerializeField] private string playerClass = "warrior";
//    private PlayerStatsManager stats;

//    private Dictionary<string, float> skillCooldowns = new();

//    // 자동 할당될 배열
//    private SkillCooldownUI[] quickSlotCooldowns;

//    // 슬롯에 대응되는 스킬 ID (A,S,D,F... 순서 매핑)
//    private readonly string[] slotSkillIds = { "slash", "smash", "fireball", "heal", "", "", "", "", "" };

//    void Awake()
//    {
//        //stats = GetComponent<PlayerStatsManager>();
//        stats = PlayerStatsManager.Instance; // ← 싱글톤
//    }

//    void Start()
//    {
//        // SkillUI Panel 밑에 있는 모든 SkillCooldownUI 자동 수집
//        var skillUI = GameObject.Find("SkillUI");
//        if (skillUI != null)
//        {
//            quickSlotCooldowns = skillUI.GetComponentsInChildren<SkillCooldownUI>(true);
//            Debug.Log($"쿨다운 UI {quickSlotCooldowns.Length}개 자동 할당 완료");
//        }

//        // JSON 로드
//        TextAsset jsonFile = Resources.Load<TextAsset>("Datas/skillData");
//        if (jsonFile == null)
//        {
//            Debug.LogError("skillData.json을 찾을 수 없습니다!");
//            return;
//        }
//        SkillFactory.LoadSkillsFromJson(jsonFile.text);

//        // 슬롯 아이콘 활성/비활성 처리
//        for (int i = 0; i < quickSlotCooldowns.Length; i++)
//        {
//            string skillId = (i < slotSkillIds.Length) ? slotSkillIds[i] : "";
//            ISkill skill = string.IsNullOrEmpty(skillId) ? null : SkillFactory.GetSkill(playerClass, skillId);

//            if (skill == null)
//            {
//                // 스킬 없음 → 아이콘 숨김
//                var icon = quickSlotCooldowns[i].transform.GetChild(0).GetComponentInChildren<Image>();
//                if (icon != null) icon.enabled = false;
//            }
//        }
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.A)) TryUseSkill("slash", 0);
//        if (Input.GetKeyDown(KeyCode.S)) TryUseSkill("smash", 1);
//        // 필요하면 D=2, F=3 ... 식으로 확장
//    }

//    private void TryUseSkill(string skillId, int slotIndex)
//    {
//        ISkill skill = SkillFactory.GetSkill(playerClass, skillId);
//        if (skill == null) return;

//        // 쿨타임 체크
//        if (skillCooldowns.ContainsKey(skill.Id) && Time.time < skillCooldowns[skill.Id])
//        {
//            Debug.Log($"{skill.Name} 스킬 쿨타임 중...");
//            return;
//        }

//        // 실행 성공시에만 쿨타임 적용
//        if (skill.Execute(gameObject, stats))
//        {
//            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;
//            if (quickSlotCooldowns != null && slotIndex < quickSlotCooldowns.Length)
//                quickSlotCooldowns[slotIndex].StartCooldown(skill.Cooldown);
//        }
//    }

//}



//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using System.Collections;

//public class SkillManager : MonoBehaviour
//{
//    [SerializeField] private string playerClass = "warrior";

//    [Header("UI refs")]
//    public SkillQuickBar quickBar;     // (비워둬도 자동으로 찾아 연결)
//    public SkillBookUI skillBook;      // (비워둬도 자동으로 찾아 연결)

//    private PlayerStatsManager stats;
//    private readonly Dictionary<string, float> skillCooldowns = new();

//    private readonly List<SkillUnlockDef> unlockDefs = new()
//    {
//        new SkillUnlockDef("slash", 1),
//        new SkillUnlockDef("smash", 3),
//    };

//    private Sprite GetIcon(string skillId)
//    => Resources.Load<Sprite>($"SkillIcons/{skillId}");

//    private bool _ready;

//    private void Awake()
//    {
//        // 여기선 아무 것도 안 함 (레이스 방지)
//    }

//    private void OnEnable()
//    {
//        StartCoroutine(InitializeWhenReady());
//    }

//    private IEnumerator InitializeWhenReady()
//    {
//        // 1) PlayerStatsManager 준비될 때까지 대기
//        while (PlayerStatsManager.Instance == null)
//            yield return null;
//        stats = PlayerStatsManager.Instance;

//        // 2) UI 자동 찾기 (에디터에서 안 연결했을 경우)
//        if (!quickBar)
//            quickBar = FindAnyObjectByType<SkillQuickBar>();
//        if (!skillBook)
//            skillBook = FindAnyObjectByType<SkillBookUI>();

//        // 3) skillData 로드
//        var jsonFile = Resources.Load<TextAsset>("Datas/skillData");
//        if (jsonFile == null)
//        {
//            Debug.LogError("[SkillManager] Resources/Datas/skillData.json 없음");
//            yield break;
//        }
//        SkillFactory.LoadSkillsFromJson(jsonFile.text);

//        // 4) 스킬북 구성
//        if (skillBook)
//        {
//            skillBook.Build(unlockDefs, GetIcon); // Content 밑에 아이템 생성
//            skillBook.Show(false);                // 처음엔 닫힘
//        }
//        else
//        {
//            Debug.LogWarning("[SkillManager] SkillBookUI 를 씬에서 찾지 못함");
//        }

//        // 5) 현재 레벨 기준 해제 + 자동 배치
//        ApplyUnlocks(stats.Data.Level);

//        // 6) 레벨업 구독
//        stats.OnLevelUp -= OnLevelUp;
//        stats.OnLevelUp += OnLevelUp;

//        _ready = true;
//    }

//    private void OnDisable()
//    {
//        if (stats != null)
//            stats.OnLevelUp -= OnLevelUp;
//    }

//    private void Update()
//    {
//        if (!_ready) return;

//        // K로 스킬북 토글
//        if (Input.GetKeyDown(KeyCode.K) && skillBook)
//            skillBook.Toggle();

//        // 단축키 → 슬롯 인덱스
//        if (Input.GetKeyDown(KeyCode.A)) UseSlot(0);
//        if (Input.GetKeyDown(KeyCode.S)) UseSlot(1);
//        if (Input.GetKeyDown(KeyCode.D)) UseSlot(2);
//        if (Input.GetKeyDown(KeyCode.F)) UseSlot(3);
//        if (Input.GetKeyDown(KeyCode.G)) UseSlot(4);
//        if (Input.GetKeyDown(KeyCode.Z)) UseSlot(5);
//        if (Input.GetKeyDown(KeyCode.X)) UseSlot(6);
//        if (Input.GetKeyDown(KeyCode.C)) UseSlot(7);
//        if (Input.GetKeyDown(KeyCode.V)) UseSlot(8);
//    }

//    private void OnLevelUp(int level) => ApplyUnlocks(level);

//    private void ApplyUnlocks(int level)
//    {
//        foreach (var def in unlockDefs)
//        {
//            if (level < def.unlockLevel) continue;

//            var skill = SkillFactory.GetSkill(playerClass, def.skillId);
//            if (skill == null) continue;

//            // 이미 할당돼 있으면 표시만
//            if (IsSkillAlreadyAssigned(def.skillId))
//            {
//                skillBook?.MarkUnlocked(def.skillId);
//                continue;
//            }

//            // 빈 슬롯 자동 배치
//            Sprite icon = GetIcon(def.skillId);
//            if (quickBar != null && quickBar.AssignToFirstEmpty(def.skillId, icon))
//            {
//                skillBook?.MarkUnlocked(def.skillId);
//                Debug.Log($"[{def.skillId}] 자동 할당 완료 (레벨 {level})");
//            }
//            else
//            {
//                skillBook?.MarkUnlocked(def.skillId);
//            }
//        }
//    }

//    private bool IsSkillAlreadyAssigned(string skillId)
//    {
//        if (quickBar == null || quickBar.slots == null) return false;
//        foreach (var s in quickBar.slots)
//        {
//            if (s != null && s.SkillId == skillId) return true;
//        }
//        return false;
//    }

//    private void UseSlot(int index)
//    {
//        if (quickBar == null) return;
//        string skillId = quickBar.GetSkillAt(index);
//        if (string.IsNullOrEmpty(skillId)) return;
//        TryUseSkill(skillId, index);
//    }

//    private void TryUseSkill(string skillId, int slotIndex)
//    {
//        var skill = SkillFactory.GetSkill(playerClass, skillId);
//        if (skill == null) return;

//        if (skillCooldowns.TryGetValue(skill.Id, out float next) && Time.time < next)
//        {
//            Debug.Log($"{skill.Name} 스킬 쿨타임 중...");
//            return;
//        }

//        if (skill.Execute(gameObject, stats))
//        {
//            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;

//            if (quickBar.slots != null && slotIndex >= 0 && slotIndex < quickBar.slots.Length)
//            {
//                var slot = quickBar.slots[slotIndex];
//                if (slot && slot.cooldownUI)
//                    slot.cooldownUI.StartCooldown(skill.Cooldown);
//            }
//        }
//    }
//}

// SkillManager.cs (핵심만)
using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";

    [Header("UI refs")]
    public SkillQuickBar quickBar;   // 비워놔도 Start에서 자동으로 찾음
    public SkillBookUI skillBook;    // 비워놔도 Start에서 자동으로 찾음

    private PlayerStatsManager stats;
    private readonly Dictionary<string, float> skillCooldowns = new();

    // 레벨 해제 규칙 (원하면 더 추가)
    private readonly List<SkillUnlockDef> unlockDefs = new()
    {
        new SkillUnlockDef("slash", 1),
        new SkillUnlockDef("smash", 3),
        new SkillUnlockDef("charge", 5),
    };

    // 아이콘 로더 (Resources/SkillIcons/{id}.png)
    private Sprite GetIcon(string skillId) => Resources.Load<Sprite>($"SkillIcons/{skillId}");

    private void Awake()
    {
        // PlayerStatsManager 싱글톤이 이미 있으면 잡고, 없으면 Start에서 다시 시도
        stats = PlayerStatsManager.Instance;
    }

    private void Start()
    {
        // 레퍼런스 자동 찾기
        if (!quickBar) quickBar = FindFirstObjectByType<SkillQuickBar>(FindObjectsInactive.Include);
        if (!skillBook) skillBook = FindFirstObjectByType<SkillBookUI>(FindObjectsInactive.Include);

        // 스킬 데이터 로드
        var jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (!jsonFile)
        {
            Debug.LogError("[SkillManager] Resources/Datas/skillData.json 없음");
            return;
        }
        SkillFactory.LoadSkillsFromJson(jsonFile.text);

        // 스킬북 구성 및 초기 상태
        if (skillBook)
        {
            skillBook.Build(unlockDefs, GetIcon);
            skillBook.Show(false); // 시작은 닫힘
            var curLv = stats ? stats.Data.Level : 1;
            skillBook.RefreshLocks(curLv); // 현재 레벨 기준으로 잠금/해제 마스크만 갱신
        }

        // 퀵바 자동 배선 + 저장된 슬롯 불러오기
        if (quickBar)
        {
            quickBar.AutoWireSlots();

            // 저장 불러오기 (없으면 null)
            var save = QuickBarPersistence.Load();
            if (save != null)
            {
                // 현재 레벨 기준으로 사용 가능한 스킬만 적용
                quickBar.ApplySaveData(
                    save,
                    GetIcon,
                    (skillId) =>
                    {
                        int lv = stats ? stats.Data.Level : 1;
                        var def = unlockDefs.Find(d => d.skillId == skillId);
                        return def != null && lv >= def.unlockLevel;
                    }
                );
            }

            // 슬롯 구성 변경될 때마다 자동 저장 (드래그&드롭/스왑 등)
            quickBar.OnChanged += () =>
            {
                QuickBarPersistence.Save(quickBar.ToSaveData());
            };
        }

        // 레벨업 이벤트 구독 (레벨업 때만 자동 배치)
        if (stats != null)
        {
            stats.OnLevelUp -= OnLevelUp;
            stats.OnLevelUp += OnLevelUp;
        }
    }

    private void OnDestroy()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;

        // 마지막으로 한번 더 저장(선택)
        if (quickBar != null)
            QuickBarPersistence.Save(quickBar.ToSaveData());
    }

    private void Update()
    {
        // 스킬북 토글
        if (Input.GetKeyDown(KeyCode.K) && skillBook != null)
            skillBook.Toggle();

        // 단축키 입력 → 슬롯 인덱스 실행
        if (Input.GetKeyDown(KeyCode.A)) UseSlot(0);
        if (Input.GetKeyDown(KeyCode.S)) UseSlot(1);
        if (Input.GetKeyDown(KeyCode.D)) UseSlot(2);
        if (Input.GetKeyDown(KeyCode.F)) UseSlot(3);
        if (Input.GetKeyDown(KeyCode.G)) UseSlot(4);
        if (Input.GetKeyDown(KeyCode.Z)) UseSlot(5);
        if (Input.GetKeyDown(KeyCode.X)) UseSlot(6);
        if (Input.GetKeyDown(KeyCode.C)) UseSlot(7);
        if (Input.GetKeyDown(KeyCode.V)) UseSlot(8);
    }

    // ============ 레벨업 시에만 자동 배치 ============
    private void OnLevelUp(int level)
    {
        // 스킬북 잠금 갱신
        skillBook?.RefreshLocks(level);

        // 빈 슬롯에 자동 배치 (이미 있는 스킬은 건너뜀)
        ApplyUnlocks(level);

        // 저장
        if (quickBar != null)
            QuickBarPersistence.Save(quickBar.ToSaveData());
    }

    private void ApplyUnlocks(int level)
    {
        if (!quickBar) return;

        foreach (var def in unlockDefs)
        {
            if (level < def.unlockLevel) continue;

            // 스킬 존재 확인
            var skill = SkillFactory.GetSkill(playerClass, def.skillId);
            if (skill == null) continue;

            // 이미 배치되어 있으면 패스 (스킬북은 해제 표시만)
            if (IsSkillAlreadyAssigned(def.skillId))
            {
                skillBook?.MarkUnlocked(def.skillId);
                continue;
            }

            // 빈 슬롯 있으면 자동 배치
            Sprite icon = GetIcon(def.skillId);
            if (quickBar.AssignToFirstEmpty(def.skillId, icon))
            {
                skillBook?.MarkUnlocked(def.skillId);
                Debug.Log($"[{def.skillId}] 자동 할당 완료 (레벨 {level})");
            }
            else
            {
                // 빈 슬롯 없어도 해제 마킹은 해둠
                skillBook?.MarkUnlocked(def.skillId);
            }
        }
    }

    private bool IsSkillAlreadyAssigned(string skillId)
    {
        if (quickBar == null || quickBar.slots == null) return false;
        foreach (var s in quickBar.slots)
        {
            if (s != null && s.SkillId == skillId)
                return true;
        }
        return false;
    }

    // ============ 스킬 실행 & 쿨다운 ============
    private void UseSlot(int index)
    {
        if (quickBar == null) return;
        string skillId = quickBar.GetSkillAt(index);
        if (string.IsNullOrEmpty(skillId)) return;

        TryUseSkill(skillId, index);
    }

    private void TryUseSkill(string skillId, int slotIndex)
    {
        var skill = SkillFactory.GetSkill(playerClass, skillId);
        if (skill == null) return;

        if (skillCooldowns.TryGetValue(skill.Id, out float next) && Time.time < next)
        {
            Debug.Log($"{skill.Name} 스킬 쿨타임 중...");
            return;
        }

        if (skill.Execute(gameObject, stats))
        {
            // 쿨타임 시작
            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;

            var slot = quickBar.GetSlot(slotIndex);
            if (slot != null && slot.cooldownUI != null)
                slot.cooldownUI.StartCooldown(skill.Cooldown);
            else
                Debug.LogWarning($"[SkillManager] 슬롯 {slotIndex}에 cooldownUI가 없습니다.");

            // 사용 후 저장(선택)
            QuickBarPersistence.Save(quickBar.ToSaveData());
        }
    }
}

