using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";
    private PlayerStatsManager stats;

    private Dictionary<string, float> skillCooldowns = new();

    // 자동 할당될 배열
    private SkillCooldownUI[] quickSlotCooldowns;

    // 슬롯에 대응되는 스킬 ID (A,S,D,F... 순서 매핑)
    private readonly string[] slotSkillIds = { "slash", "smash", "fireball", "heal", "", "", "", "", "" };

    void Awake()
    {
        //stats = GetComponent<PlayerStatsManager>();
        stats = PlayerStatsManager.Instance; // ← 싱글톤
    }

    void Start()
    {
        // SkillUI Panel 밑에 있는 모든 SkillCooldownUI 자동 수집
        var skillUI = GameObject.Find("SkillUI");
        if (skillUI != null)
        {
            quickSlotCooldowns = skillUI.GetComponentsInChildren<SkillCooldownUI>(true);
            Debug.Log($"쿨다운 UI {quickSlotCooldowns.Length}개 자동 할당 완료");
        }

        // JSON 로드
        TextAsset jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (jsonFile == null)
        {
            Debug.LogError("skillData.json을 찾을 수 없습니다!");
            return;
        }
        SkillFactory.LoadSkillsFromJson(jsonFile.text);

        // 슬롯 아이콘 활성/비활성 처리
        for (int i = 0; i < quickSlotCooldowns.Length; i++)
        {
            string skillId = (i < slotSkillIds.Length) ? slotSkillIds[i] : "";
            ISkill skill = string.IsNullOrEmpty(skillId) ? null : SkillFactory.GetSkill(playerClass, skillId);

            if (skill == null)
            {
                // 스킬 없음 → 아이콘 숨김
                var icon = quickSlotCooldowns[i].transform.GetChild(0).GetComponentInChildren<Image>();
                if (icon != null) icon.enabled = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) TryUseSkill("slash", 0);
        if (Input.GetKeyDown(KeyCode.S)) TryUseSkill("smash", 1);
        // 필요하면 D=2, F=3 ... 식으로 확장
    }

    private void TryUseSkill(string skillId, int slotIndex)
    {
        ISkill skill = SkillFactory.GetSkill(playerClass, skillId);
        if (skill == null) return;

        // 쿨타임 체크
        if (skillCooldowns.ContainsKey(skill.Id) && Time.time < skillCooldowns[skill.Id])
        {
            Debug.Log($"{skill.Name} 스킬 쿨타임 중...");
            return;
        }

        // 실행 성공시에만 쿨타임 적용
        if (skill.Execute(gameObject, stats))
        {
            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;
            if (quickSlotCooldowns != null && slotIndex < quickSlotCooldowns.Length)
                quickSlotCooldowns[slotIndex].StartCooldown(skill.Cooldown);
        }
    }

}
