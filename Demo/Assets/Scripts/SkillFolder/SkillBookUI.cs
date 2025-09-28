// SkillBookUI.cs (핵심만)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBookUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;                 // 비워져 있으면 gameObject 사용
    public Button closeButton;
    public Transform contentParent;
    public SkillBookItemDraggable itemPrefab;

    private Dictionary<string, SkillBookItemDraggable> items = new();
    private PlayerStatsManager stats;

    void Awake()
    {
        stats = PlayerStatsManager.Instance;
        if (!panel) panel = gameObject;               // ★ 방어
        if (closeButton) closeButton.onClick.AddListener(() => Show(false));
        Show(false);
    }

    void OnEnable()
    {
        if (stats != null) { stats.OnLevelUp -= OnLevelUp; stats.OnLevelUp += OnLevelUp; }
    }
    void OnDisable()
    {
        if (stats != null) stats.OnLevelUp -= OnLevelUp;
    }

    public void Show(bool visible) => panel.SetActive(visible);
    public void Toggle() => Show(!panel.activeSelf);

    public bool IsOpen => panel != null && panel.activeSelf;

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
        foreach (var kv in items) kv.Value.SetUnlocked(level >= kv.Value.UnlockLevel);
    }

    public void MarkUnlocked(string skillId)
    {
        if (items.TryGetValue(skillId, out var it)) it.SetUnlocked(true);
    }
}
