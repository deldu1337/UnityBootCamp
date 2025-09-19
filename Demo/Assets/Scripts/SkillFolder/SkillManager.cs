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
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";

    [Header("UI refs")]
    public SkillQuickBar quickBar;     // SkillUI 밑 슬롯들에 붙어있는 SkillSlotUI[] 을 가진 오브젝트
    public SkillBookUI skillBook;      // SkillBookUI(스킬북 패널 제어)

    private PlayerStatsManager stats;

    // 스킬 쿨타임 관리
    private readonly Dictionary<string, float> skillCooldowns = new();

    // 레벨 해제 규칙 (필요 시 조정)
    private readonly List<SkillUnlockDef> unlockDefs = new()
    {
        new SkillUnlockDef("slash", 1),
        new SkillUnlockDef("smash", 3),
        // 이후 스킬들은 여기에 추가
    };

    // 아이콘 로더(예시: Resources/Icons/Skills/{id})
    private Sprite GetIcon(string skillId)
        => Resources.Load<Sprite>($"Icons/Skills/{skillId}");

    void Awake()
    {
        stats = PlayerStatsManager.Instance;
    }

    void Start()
    {
        // JSON 로드 (기존과 동일)
        var jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (jsonFile == null)
        {
            Debug.LogError("skillData.json을 찾을 수 없습니다!");
            return;
        }
        SkillFactory.LoadSkillsFromJson(jsonFile.text);

        // 스킬북 UI 구성
        if (skillBook != null)
        {
            skillBook.Build(unlockDefs, GetIcon);
        }

        // 현재 레벨 기준으로 한 번 잠금 해제 적용 + 빈 슬롯 자동 할당
        ApplyUnlocks(stats.Data.Level);

        // 레벨업 이벤트 구독
        stats.OnLevelUp += OnLevelUp;
    }

    void OnDestroy()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;
    }

    void Update()
    {
        // K로 스킬북 열/닫
        if (Input.GetKeyDown(KeyCode.K) && skillBook != null)
            skillBook.Toggle();

        // 단축키 → 슬롯 인덱스 매핑
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

    void OnLevelUp(int level)
    {
        ApplyUnlocks(level);
    }

    private void ApplyUnlocks(int level)
    {
        foreach (var def in unlockDefs)
        {
            if (level < def.unlockLevel) continue;

            // 스킬 존재 확인
            var skill = SkillFactory.GetSkill(playerClass, def.skillId);
            if (skill == null) continue;

            // 이미 퀵슬롯에 있는지 확인
            if (IsSkillAlreadyAssigned(def.skillId))
            {
                // 스킬북에도 해제 표시는 해두자
                skillBook?.MarkUnlocked(def.skillId);
                continue;
            }

            // 빈 슬롯에 자동 할당
            Sprite icon = GetIcon(def.skillId);
            if (quickBar != null && quickBar.AssignToFirstEmpty(def.skillId, icon))
            {
                skillBook?.MarkUnlocked(def.skillId);
                Debug.Log($"[{def.skillId}] 자동 할당 완료 (레벨 {level})");
            }
            else
            {
                // 빈 슬롯이 없더라도 해제 상태 자체는 표시
                skillBook?.MarkUnlocked(def.skillId);
            }
        }
    }

    private bool IsSkillAlreadyAssigned(string skillId)
    {
        if (quickBar == null || quickBar.slots == null) return false;
        foreach (var s in quickBar.slots)
        {
            if (s != null && s.SkillId == skillId) return true;
        }
        return false;
    }

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

        // 쿨타임 체크
        if (skillCooldowns.TryGetValue(skill.Id, out float next) && Time.time < next)
        {
            Debug.Log($"{skill.Name} 스킬 쿨타임 중...");
            return;
        }

        // 실행 성공시에만 쿨적용 + UI 반영
        if (skill.Execute(gameObject, stats))
        {
            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;

            // 슬롯의 쿨다운 표시
            if (quickBar.slots != null && slotIndex >= 0 && slotIndex < quickBar.slots.Length)
            {
                var slot = quickBar.slots[slotIndex];
                if (slot && slot.cooldownUI)
                    slot.cooldownUI.StartCooldown(skill.Cooldown);
            }
        }
    }
}
