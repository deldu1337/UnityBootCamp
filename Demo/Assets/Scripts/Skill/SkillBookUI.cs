using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBookUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;          // ����� ������ gameObject ���
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentParent;
    [SerializeField] private SkillBookItemDraggable itemPrefab;

    private readonly Dictionary<string, SkillBookItemDraggable> items = new();
    private PlayerStatsManager stats;

    public bool IsOpen => panel != null && panel.activeSelf;

    private const string ESC_KEY = "skillbook";

    void Awake()
    {
        stats = PlayerStatsManager.Instance;
        if (!panel) panel = gameObject; // ���
        if (closeButton) closeButton.onClick.AddListener(() => Show(false));
        Show(false);
    }

    void OnEnable()
    {
        if (stats != null)
        {
            stats.OnLevelUp -= OnLevelUp;
            stats.OnLevelUp += OnLevelUp;
        }
    }
    void OnDisable()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;
        // Ȥ�� ������ ����� ���� ���¶�� ���ÿ��� ����
        if (IsOpen) UIEscapeStack.GetOrCreate().Remove(ESC_KEY);
    }

    public void Toggle() => Show(!IsOpen);

    public void Show(bool visible)
    {
        if (!panel) return;
        if (visible == IsOpen) return;

        panel.SetActive(visible);

        var esc = UIEscapeStack.GetOrCreate();
        if (visible)
        {
            // ESC�� �ݱ� ���� ���
            esc.Push(
                key: ESC_KEY,
                close: () => Show(false),
                isOpen: () => IsOpen
            );
        }
        else
        {
            // ���� ����
            esc.Remove(ESC_KEY);
        }
    }

    public void Build(List<SkillUnlockDef> defs, System.Func<string, Sprite> iconResolver)
    {
        if (!contentParent || !itemPrefab) return;

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

    private void OnLevelUp(int level) => RefreshLocks(level);

    public void RefreshLocks(int level)
    {
        foreach (var kv in items)
            kv.Value.SetUnlocked(level >= kv.Value.UnlockLevel);
    }

    public void MarkUnlocked(string skillId)
    {
        if (items.TryGetValue(skillId, out var it))
            it.SetUnlocked(true);
    }
}
