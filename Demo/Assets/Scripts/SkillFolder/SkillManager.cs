using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private string playerClass = "warrior";
    private Button SkillButton;

    [Header("UI refs")]
    public SkillQuickBar quickBar;   // ������� Start���� �ڵ����� ã��
    public SkillBookUI skillBook;    // ������� Start���� �ڵ����� ã��

    private PlayerStatsManager stats;
    private readonly Dictionary<string, float> skillCooldowns = new();

    // ���� ���� ��Ģ (���ϸ� �� �߰�)
    private readonly List<SkillUnlockDef> unlockDefs = new()
    {
        new SkillUnlockDef("slash", 2),
        new SkillUnlockDef("smash", 3),
        new SkillUnlockDef("charge", 4),
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
        SkillButton = GameObject.Find("QuickUI").transform.GetChild(0).GetComponent<Button>();
        SkillButton.onClick.AddListener(skillBook.Toggle);

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
            skillBook.RefreshLocks(curLv);
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
        // ��ų�� ��� (ESC ó���� �߾� ESCView + UIEscapeStack�� ���)
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
