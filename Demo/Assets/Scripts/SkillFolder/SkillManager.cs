//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;

//public class SkillManager : MonoBehaviour
//{
//    [SerializeField] private string playerClass = "warrior";
//    private PlayerStatsManager stats;

//    private Dictionary<string, float> skillCooldowns = new();

//    // �ڵ� �Ҵ�� �迭
//    private SkillCooldownUI[] quickSlotCooldowns;

//    // ���Կ� �����Ǵ� ��ų ID (A,S,D,F... ���� ����)
//    private readonly string[] slotSkillIds = { "slash", "smash", "fireball", "heal", "", "", "", "", "" };

//    void Awake()
//    {
//        //stats = GetComponent<PlayerStatsManager>();
//        stats = PlayerStatsManager.Instance; // �� �̱���
//    }

//    void Start()
//    {
//        // SkillUI Panel �ؿ� �ִ� ��� SkillCooldownUI �ڵ� ����
//        var skillUI = GameObject.Find("SkillUI");
//        if (skillUI != null)
//        {
//            quickSlotCooldowns = skillUI.GetComponentsInChildren<SkillCooldownUI>(true);
//            Debug.Log($"��ٿ� UI {quickSlotCooldowns.Length}�� �ڵ� �Ҵ� �Ϸ�");
//        }

//        // JSON �ε�
//        TextAsset jsonFile = Resources.Load<TextAsset>("Datas/skillData");
//        if (jsonFile == null)
//        {
//            Debug.LogError("skillData.json�� ã�� �� �����ϴ�!");
//            return;
//        }
//        SkillFactory.LoadSkillsFromJson(jsonFile.text);

//        // ���� ������ Ȱ��/��Ȱ�� ó��
//        for (int i = 0; i < quickSlotCooldowns.Length; i++)
//        {
//            string skillId = (i < slotSkillIds.Length) ? slotSkillIds[i] : "";
//            ISkill skill = string.IsNullOrEmpty(skillId) ? null : SkillFactory.GetSkill(playerClass, skillId);

//            if (skill == null)
//            {
//                // ��ų ���� �� ������ ����
//                var icon = quickSlotCooldowns[i].transform.GetChild(0).GetComponentInChildren<Image>();
//                if (icon != null) icon.enabled = false;
//            }
//        }
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.A)) TryUseSkill("slash", 0);
//        if (Input.GetKeyDown(KeyCode.S)) TryUseSkill("smash", 1);
//        // �ʿ��ϸ� D=2, F=3 ... ������ Ȯ��
//    }

//    private void TryUseSkill(string skillId, int slotIndex)
//    {
//        ISkill skill = SkillFactory.GetSkill(playerClass, skillId);
//        if (skill == null) return;

//        // ��Ÿ�� üũ
//        if (skillCooldowns.ContainsKey(skill.Id) && Time.time < skillCooldowns[skill.Id])
//        {
//            Debug.Log($"{skill.Name} ��ų ��Ÿ�� ��...");
//            return;
//        }

//        // ���� �����ÿ��� ��Ÿ�� ����
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
    public SkillQuickBar quickBar;     // SkillUI �� ���Ե鿡 �پ��ִ� SkillSlotUI[] �� ���� ������Ʈ
    public SkillBookUI skillBook;      // SkillBookUI(��ų�� �г� ����)

    private PlayerStatsManager stats;

    // ��ų ��Ÿ�� ����
    private readonly Dictionary<string, float> skillCooldowns = new();

    // ���� ���� ��Ģ (�ʿ� �� ����)
    private readonly List<SkillUnlockDef> unlockDefs = new()
    {
        new SkillUnlockDef("slash", 1),
        new SkillUnlockDef("smash", 3),
        // ���� ��ų���� ���⿡ �߰�
    };

    // ������ �δ�(����: Resources/Icons/Skills/{id})
    private Sprite GetIcon(string skillId)
        => Resources.Load<Sprite>($"Icons/Skills/{skillId}");

    void Awake()
    {
        stats = PlayerStatsManager.Instance;
    }

    void Start()
    {
        // JSON �ε� (������ ����)
        var jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (jsonFile == null)
        {
            Debug.LogError("skillData.json�� ã�� �� �����ϴ�!");
            return;
        }
        SkillFactory.LoadSkillsFromJson(jsonFile.text);

        // ��ų�� UI ����
        if (skillBook != null)
        {
            skillBook.Build(unlockDefs, GetIcon);
        }

        // ���� ���� �������� �� �� ��� ���� ���� + �� ���� �ڵ� �Ҵ�
        ApplyUnlocks(stats.Data.Level);

        // ������ �̺�Ʈ ����
        stats.OnLevelUp += OnLevelUp;
    }

    void OnDestroy()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;
    }

    void Update()
    {
        // K�� ��ų�� ��/��
        if (Input.GetKeyDown(KeyCode.K) && skillBook != null)
            skillBook.Toggle();

        // ����Ű �� ���� �ε��� ����
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

            // ��ų ���� Ȯ��
            var skill = SkillFactory.GetSkill(playerClass, def.skillId);
            if (skill == null) continue;

            // �̹� �����Կ� �ִ��� Ȯ��
            if (IsSkillAlreadyAssigned(def.skillId))
            {
                // ��ų�Ͽ��� ���� ǥ�ô� �ص���
                skillBook?.MarkUnlocked(def.skillId);
                continue;
            }

            // �� ���Կ� �ڵ� �Ҵ�
            Sprite icon = GetIcon(def.skillId);
            if (quickBar != null && quickBar.AssignToFirstEmpty(def.skillId, icon))
            {
                skillBook?.MarkUnlocked(def.skillId);
                Debug.Log($"[{def.skillId}] �ڵ� �Ҵ� �Ϸ� (���� {level})");
            }
            else
            {
                // �� ������ ������ ���� ���� ��ü�� ǥ��
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

        // ��Ÿ�� üũ
        if (skillCooldowns.TryGetValue(skill.Id, out float next) && Time.time < next)
        {
            Debug.Log($"{skill.Name} ��ų ��Ÿ�� ��...");
            return;
        }

        // ���� �����ÿ��� ������ + UI �ݿ�
        if (skill.Execute(gameObject, stats))
        {
            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;

            // ������ ��ٿ� ǥ��
            if (quickBar.slots != null && slotIndex >= 0 && slotIndex < quickBar.slots.Length)
            {
                var slot = quickBar.slots[slotIndex];
                if (slot && slot.cooldownUI)
                    slot.cooldownUI.StartCooldown(skill.Cooldown);
            }
        }
    }
}
