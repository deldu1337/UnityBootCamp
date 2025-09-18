using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";
    private PlayerStatsManager stats;

    private Dictionary<string, float> skillCooldowns = new();

    // �ڵ� �Ҵ�� �迭
    private SkillCooldownUI[] quickSlotCooldowns;

    // ���Կ� �����Ǵ� ��ų ID (A,S,D,F... ���� ����)
    private readonly string[] slotSkillIds = { "slash", "smash", "fireball", "heal", "", "", "", "", "" };

    void Awake()
    {
        //stats = GetComponent<PlayerStatsManager>();
        stats = PlayerStatsManager.Instance; // �� �̱���
    }

    void Start()
    {
        // SkillUI Panel �ؿ� �ִ� ��� SkillCooldownUI �ڵ� ����
        var skillUI = GameObject.Find("SkillUI");
        if (skillUI != null)
        {
            quickSlotCooldowns = skillUI.GetComponentsInChildren<SkillCooldownUI>(true);
            Debug.Log($"��ٿ� UI {quickSlotCooldowns.Length}�� �ڵ� �Ҵ� �Ϸ�");
        }

        // JSON �ε�
        TextAsset jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (jsonFile == null)
        {
            Debug.LogError("skillData.json�� ã�� �� �����ϴ�!");
            return;
        }
        SkillFactory.LoadSkillsFromJson(jsonFile.text);

        // ���� ������ Ȱ��/��Ȱ�� ó��
        for (int i = 0; i < quickSlotCooldowns.Length; i++)
        {
            string skillId = (i < slotSkillIds.Length) ? slotSkillIds[i] : "";
            ISkill skill = string.IsNullOrEmpty(skillId) ? null : SkillFactory.GetSkill(playerClass, skillId);

            if (skill == null)
            {
                // ��ų ���� �� ������ ����
                var icon = quickSlotCooldowns[i].transform.GetChild(0).GetComponentInChildren<Image>();
                if (icon != null) icon.enabled = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) TryUseSkill("slash", 0);
        if (Input.GetKeyDown(KeyCode.S)) TryUseSkill("smash", 1);
        // �ʿ��ϸ� D=2, F=3 ... ������ Ȯ��
    }

    private void TryUseSkill(string skillId, int slotIndex)
    {
        ISkill skill = SkillFactory.GetSkill(playerClass, skillId);
        if (skill == null) return;

        // ��Ÿ�� üũ
        if (skillCooldowns.ContainsKey(skill.Id) && Time.time < skillCooldowns[skill.Id])
        {
            Debug.Log($"{skill.Name} ��ų ��Ÿ�� ��...");
            return;
        }

        // ���� �����ÿ��� ��Ÿ�� ����
        if (skill.Execute(gameObject, stats))
        {
            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;
            if (quickSlotCooldowns != null && slotIndex < quickSlotCooldowns.Length)
                quickSlotCooldowns[slotIndex].StartCooldown(skill.Cooldown);
        }
    }

}
