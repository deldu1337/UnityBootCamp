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
            Debug.LogError("skillData.json�� ã�� �� �����ϴ�!");
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

        // ��Ÿ�� üũ
        if (skillCooldowns.ContainsKey(skill.Id) && Time.time < skillCooldowns[skill.Id])
        {
            Debug.Log($"{skill.Name} ��ų ��Ÿ�� ��...");
            return;
        }

        skill.Execute(gameObject, stats);
        skillCooldowns[skill.Id] = Time.time + skill.Cooldown;
    }
}
