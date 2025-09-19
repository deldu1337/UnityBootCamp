using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBookUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;              // SkillBookUI ��Ʈ(�Ѱ� ���� �г�)
    public Button closeButton;            // ���� ��� X ��ư
    public Transform contentParent;       // ��ų ��ư/�������� ��� �׸���/�����̳�
    public SkillBookItemDraggable itemPrefab; // ��ų ������ ������ (������ + ��ݿ������� + �巡��)

    private Dictionary<string, SkillBookItemDraggable> items = new();
    private PlayerStatsManager stats;

    void Awake()
    {
        stats = PlayerStatsManager.Instance;
        if (closeButton) closeButton.onClick.AddListener(() => Show(false));
        Show(false);
    }

    void OnEnable()
    {
        // ������ �̺�Ʈ ���� �� ��ݰ���
        if (stats != null)
        {
            stats.OnLevelUp -= OnLevelUp;
            stats.OnLevelUp += OnLevelUp;
        }
    }
    void OnDisable()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;
    }

    public void Show(bool visible) => panel.SetActive(visible);

    public void Toggle() => Show(!panel.activeSelf);

    // ��ų�� ��� ä��� (�ʱ� 1ȸ)
    public void Build(List<SkillUnlockDef> defs, System.Func<string, Sprite> iconResolver)
    {
        foreach (Transform t in contentParent) Destroy(t.gameObject);
        items.Clear();

        foreach (var def in defs)
        {
            var item = Instantiate(itemPrefab, contentParent);
            var sp = iconResolver?.Invoke(def.skillId);
            item.Setup(def.skillId, sp, def.unlockLevel, false);
            items[def.skillId] = item;
        }
        RefreshLocks(stats?.Data?.Level ?? 1);
    }

    void OnLevelUp(int level) => RefreshLocks(level);

    public void RefreshLocks(int level)
    {
        foreach (var kv in items)
        {
            var item = kv.Value;
            item.SetUnlocked(level >= item.UnlockLevel);
        }
    }

    // �ܺ�(�Ŵ���)���� Ư�� ��ų�� ������� �˷��� �� ���⵵ ����
    public void MarkUnlocked(string skillId)
    {
        if (items.TryGetValue(skillId, out var it))
            it.SetUnlocked(true);
    }
}
