using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";

    private PlayerStatsManager stats;
    private Dictionary<string, float> skillCooldowns = new();

    void Awake()
    {
        stats = GetComponent<PlayerStatsManager>();
    }

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (jsonFile == null)
        {
            Debug.LogError("skillData.json을 찾을 수 없습니다!");
            return;
        }

        SkillFactory.LoadSkillsFromJson(jsonFile.text);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) TryUseSkill("slash");
        if (Input.GetKeyDown(KeyCode.S)) TryUseSkill("smash");
    }

    private void TryUseSkill(string skillId)
    {
        ISkill skill = SkillFactory.GetSkill(playerClass, skillId);
        if (skill == null) return;

        // 쿨타임 체크
        if (skillCooldowns.ContainsKey(skill.Id) && Time.time < skillCooldowns[skill.Id])
        {
            Debug.Log($"{skill.Name} 스킬 쿨타임 중...");
            return;
        }

        skill.Execute(gameObject, stats);
        skillCooldowns[skill.Id] = Time.time + skill.Cooldown;
    }
}
