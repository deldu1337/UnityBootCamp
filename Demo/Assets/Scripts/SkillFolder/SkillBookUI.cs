using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBookUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panel;              // SkillBookUI 루트(켜고 끄는 패널)
    public Button closeButton;            // 우측 상단 X 버튼
    public Transform contentParent;       // 스킬 버튼/아이템을 담는 그리드/컨테이너
    public SkillBookItemDraggable itemPrefab; // 스킬 아이템 프리팹 (아이콘 + 잠금오버레이 + 드래깅)

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
        // 레벨업 이벤트 구독 → 잠금갱신
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

    // 스킬북 목록 채우기 (초기 1회)
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

    // 외부(매니저)에서 특정 스킬이 언락됨을 알려줌 → 보기도 갱신
    public void MarkUnlocked(string skillId)
    {
        if (items.TryGetValue(skillId, out var it))
            it.SetUnlocked(true);
    }
}
