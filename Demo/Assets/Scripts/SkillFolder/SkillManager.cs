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



//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using System.Collections;

//public class SkillManager : MonoBehaviour
//{
//    [SerializeField] private string playerClass = "warrior";

//    [Header("UI refs")]
//    public SkillQuickBar quickBar;     // (����ֵ� �ڵ����� ã�� ����)
//    public SkillBookUI skillBook;      // (����ֵ� �ڵ����� ã�� ����)

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
//        // ���⼱ �ƹ� �͵� �� �� (���̽� ����)
//    }

//    private void OnEnable()
//    {
//        StartCoroutine(InitializeWhenReady());
//    }

//    private IEnumerator InitializeWhenReady()
//    {
//        // 1) PlayerStatsManager �غ�� ������ ���
//        while (PlayerStatsManager.Instance == null)
//            yield return null;
//        stats = PlayerStatsManager.Instance;

//        // 2) UI �ڵ� ã�� (�����Ϳ��� �� �������� ���)
//        if (!quickBar)
//            quickBar = FindAnyObjectByType<SkillQuickBar>();
//        if (!skillBook)
//            skillBook = FindAnyObjectByType<SkillBookUI>();

//        // 3) skillData �ε�
//        var jsonFile = Resources.Load<TextAsset>("Datas/skillData");
//        if (jsonFile == null)
//        {
//            Debug.LogError("[SkillManager] Resources/Datas/skillData.json ����");
//            yield break;
//        }
//        SkillFactory.LoadSkillsFromJson(jsonFile.text);

//        // 4) ��ų�� ����
//        if (skillBook)
//        {
//            skillBook.Build(unlockDefs, GetIcon); // Content �ؿ� ������ ����
//            skillBook.Show(false);                // ó���� ����
//        }
//        else
//        {
//            Debug.LogWarning("[SkillManager] SkillBookUI �� ������ ã�� ����");
//        }

//        // 5) ���� ���� ���� ���� + �ڵ� ��ġ
//        ApplyUnlocks(stats.Data.Level);

//        // 6) ������ ����
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

//        // K�� ��ų�� ���
//        if (Input.GetKeyDown(KeyCode.K) && skillBook)
//            skillBook.Toggle();

//        // ����Ű �� ���� �ε���
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

//            // �̹� �Ҵ�� ������ ǥ�ø�
//            if (IsSkillAlreadyAssigned(def.skillId))
//            {
//                skillBook?.MarkUnlocked(def.skillId);
//                continue;
//            }

//            // �� ���� �ڵ� ��ġ
//            Sprite icon = GetIcon(def.skillId);
//            if (quickBar != null && quickBar.AssignToFirstEmpty(def.skillId, icon))
//            {
//                skillBook?.MarkUnlocked(def.skillId);
//                Debug.Log($"[{def.skillId}] �ڵ� �Ҵ� �Ϸ� (���� {level})");
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
//            Debug.Log($"{skill.Name} ��ų ��Ÿ�� ��...");
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

// SkillManager.cs (�ٽɸ�)
using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";

    [Header("UI refs")]
    public SkillQuickBar quickBar;   // ������� Start���� �ڵ����� ã��
    public SkillBookUI skillBook;    // ������� Start���� �ڵ����� ã��

    private PlayerStatsManager stats;
    private readonly Dictionary<string, float> skillCooldowns = new();

    // ���� ���� ��Ģ (���ϸ� �� �߰�)
    private readonly List<SkillUnlockDef> unlockDefs = new()
    {
        new SkillUnlockDef("slash", 1),
        new SkillUnlockDef("smash", 3),
        new SkillUnlockDef("charge", 5),
    };

    // ������ �δ� (Resources/SkillIcons/{id}.png)
    private Sprite GetIcon(string skillId) => Resources.Load<Sprite>($"SkillIcons/{skillId}");

    private void Awake()
    {
        // PlayerStatsManager �̱����� �̹� ������ ���, ������ Start���� �ٽ� �õ�
        stats = PlayerStatsManager.Instance;
    }

    private void Start()
    {
        // ���۷��� �ڵ� ã��
        if (!quickBar) quickBar = FindFirstObjectByType<SkillQuickBar>(FindObjectsInactive.Include);
        if (!skillBook) skillBook = FindFirstObjectByType<SkillBookUI>(FindObjectsInactive.Include);

        // ��ų ������ �ε�
        var jsonFile = Resources.Load<TextAsset>("Datas/skillData");
        if (!jsonFile)
        {
            Debug.LogError("[SkillManager] Resources/Datas/skillData.json ����");
            return;
        }
        SkillFactory.LoadSkillsFromJson(jsonFile.text);

        // ��ų�� ���� �� �ʱ� ����
        if (skillBook)
        {
            skillBook.Build(unlockDefs, GetIcon);
            skillBook.Show(false); // ������ ����
            var curLv = stats ? stats.Data.Level : 1;
            skillBook.RefreshLocks(curLv); // ���� ���� �������� ���/���� ����ũ�� ����
        }

        // ���� �ڵ� �輱 + ����� ���� �ҷ�����
        if (quickBar)
        {
            quickBar.AutoWireSlots();

            // ���� �ҷ����� (������ null)
            var save = QuickBarPersistence.Load();
            if (save != null)
            {
                // ���� ���� �������� ��� ������ ��ų�� ����
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

            // ���� ���� ����� ������ �ڵ� ���� (�巡��&���/���� ��)
            quickBar.OnChanged += () =>
            {
                QuickBarPersistence.Save(quickBar.ToSaveData());
            };
        }

        // ������ �̺�Ʈ ���� (������ ���� �ڵ� ��ġ)
        if (stats != null)
        {
            stats.OnLevelUp -= OnLevelUp;
            stats.OnLevelUp += OnLevelUp;
        }
    }

    private void OnDestroy()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;

        // ���������� �ѹ� �� ����(����)
        if (quickBar != null)
            QuickBarPersistence.Save(quickBar.ToSaveData());
    }

    private void Update()
    {
        // ��ų�� ���
        if (Input.GetKeyDown(KeyCode.K) && skillBook != null)
            skillBook.Toggle();

        // ����Ű �Է� �� ���� �ε��� ����
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

    // ============ ������ �ÿ��� �ڵ� ��ġ ============
    private void OnLevelUp(int level)
    {
        // ��ų�� ��� ����
        skillBook?.RefreshLocks(level);

        // �� ���Կ� �ڵ� ��ġ (�̹� �ִ� ��ų�� �ǳʶ�)
        ApplyUnlocks(level);

        // ����
        if (quickBar != null)
            QuickBarPersistence.Save(quickBar.ToSaveData());
    }

    private void ApplyUnlocks(int level)
    {
        if (!quickBar) return;

        foreach (var def in unlockDefs)
        {
            if (level < def.unlockLevel) continue;

            // ��ų ���� Ȯ��
            var skill = SkillFactory.GetSkill(playerClass, def.skillId);
            if (skill == null) continue;

            // �̹� ��ġ�Ǿ� ������ �н� (��ų���� ���� ǥ�ø�)
            if (IsSkillAlreadyAssigned(def.skillId))
            {
                skillBook?.MarkUnlocked(def.skillId);
                continue;
            }

            // �� ���� ������ �ڵ� ��ġ
            Sprite icon = GetIcon(def.skillId);
            if (quickBar.AssignToFirstEmpty(def.skillId, icon))
            {
                skillBook?.MarkUnlocked(def.skillId);
                Debug.Log($"[{def.skillId}] �ڵ� �Ҵ� �Ϸ� (���� {level})");
            }
            else
            {
                // �� ���� ��� ���� ��ŷ�� �ص�
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

    // ============ ��ų ���� & ��ٿ� ============
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
            Debug.Log($"{skill.Name} ��ų ��Ÿ�� ��...");
            return;
        }

        if (skill.Execute(gameObject, stats))
        {
            // ��Ÿ�� ����
            skillCooldowns[skill.Id] = Time.time + skill.Cooldown;

            var slot = quickBar.GetSlot(slotIndex);
            if (slot != null && slot.cooldownUI != null)
                slot.cooldownUI.StartCooldown(skill.Cooldown);
            else
                Debug.LogWarning($"[SkillManager] ���� {slotIndex}�� cooldownUI�� �����ϴ�.");

            // ��� �� ����(����)
            QuickBarPersistence.Save(quickBar.ToSaveData());
        }
    }
}

