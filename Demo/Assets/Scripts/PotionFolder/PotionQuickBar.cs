using System;
using UnityEngine;

public class PotionQuickBar : MonoBehaviour
{
    public static PotionQuickBar Instance { get; private set; }

    [Header("Slots (0~3)")]
    public PotionSlotUI[] slots = new PotionSlotUI[4];

    [Header("옵션")]
    public KeyCode key1 = KeyCode.Alpha1;
    public KeyCode key2 = KeyCode.Alpha2;
    public KeyCode key3 = KeyCode.Alpha3;
    public KeyCode key4 = KeyCode.Alpha4;

    [SerializeField] private InventoryPresenter inventoryPresenter; // 인스펙터로 연결 권장
    private PlayerStatsManager stats;

    // 슬롯별 캐시
    private string[] slotUID = new string[4];
    private int[] slotItemId = new int[4];
    private string[] slotIconPath = new string[4];
    private string[] slotPrefabPath = new string[4];

    private float[] cachedHP = new float[4];
    private float[] cachedMP = new float[4];

    // 이벤트(선택): 슬롯 구성 변경 때 바깥에서 후킹하고 싶으면
    public event System.Action OnChanged;

    void Awake() => Instance = this;

    void Start()
    {
        AutoWireByHierarchy();
        if (!inventoryPresenter) inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        stats = PlayerStatsManager.Instance ?? FindAnyObjectByType<PlayerStatsManager>();

        for (int i = 0; i < slots.Length; i++)
            slots[i]?.Clear();

        // 저장 불러와 복원
        var save = PotionQuickBarPersistence.Load();
        if (save != null) ApplySaveData(save);
    }

    void Update()
    {
        if (Input.GetKeyDown(key1)) Use(0);
        if (Input.GetKeyDown(key2)) Use(1);
        if (Input.GetKeyDown(key3)) Use(2);
        if (Input.GetKeyDown(key4)) Use(3);
    }

    public void Assign(int index, InventoryItem item, Sprite icon)
    {
        if (!ValidIndex(index) || item == null || item.data == null) return;
        if (!string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase)) return;

        // 만약 슬롯에 기존 포션이 있으면 먼저 인벤토리로 반환(아이템 유실 방지)
        if (!string.IsNullOrEmpty(slotUID[index]))
            ReturnToInventory(index, refreshUI: false); // 아래에 구현

        // 1) 슬롯 UI/캐시 갱신
        slots[index].Set(item, icon);
        slotUID[index] = item.uniqueId;
        slotItemId[index] = item.id;
        slotIconPath[index] = item.iconPath;
        slotPrefabPath[index] = item.prefabPath;
        cachedHP[index] = item.data.hp;
        cachedMP[index] = item.data.mp;

        // 2) 인벤토리에서 제거(이관)
        if (!inventoryPresenter)
            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        inventoryPresenter?.RemoveItemFromInventory(item.uniqueId);

        // 3) 저장
        PotionQuickBarPersistence.Save(ToSaveData());
    }



    public void Move(int from, int to)
    {
        if (!ValidIndex(from) || !ValidIndex(to) || from == to) return;
        var a = slots[from]; var b = slots[to]; if (!a || !b) return;

        (a.boundUniqueId, b.boundUniqueId) = (b.boundUniqueId, a.boundUniqueId);
        (slotUID[from], slotUID[to]) = (slotUID[to], slotUID[from]);
        (slotItemId[from], slotItemId[to]) = (slotItemId[to], slotItemId[from]);
        (slotIconPath[from], slotIconPath[to]) = (slotIconPath[to], slotIconPath[from]);
        (slotPrefabPath[from], slotPrefabPath[to]) = (slotPrefabPath[to], slotPrefabPath[from]);
        (cachedHP[from], cachedHP[to]) = (cachedHP[to], cachedHP[from]);
        (cachedMP[from], cachedMP[to]) = (cachedMP[to], cachedMP[from]);

        var spA = a.icon ? a.icon.sprite : null;
        var spB = b.icon ? b.icon.sprite : null;
        if (a.icon) { a.icon.sprite = spB; a.icon.enabled = (spB != null) && !string.IsNullOrEmpty(a.boundUniqueId); }
        if (b.icon) { b.icon.sprite = spA; b.icon.enabled = (spA != null) && !string.IsNullOrEmpty(b.boundUniqueId); }

        a.RefreshEmptyOverlay(); b.RefreshEmptyOverlay();

        PotionQuickBarPersistence.Save(ToSaveData());
    }


    public void Clear(int index)
    {
        if (!ValidIndex(index) || slots[index] == null) return;

        slots[index].Clear();
        slotUID[index] = null;
        slotItemId[index] = 0;
        slotIconPath[index] = null;
        slotPrefabPath[index] = null;
        cachedHP[index] = cachedMP[index] = 0f;

        PotionQuickBarPersistence.Save(ToSaveData());
    }


    public void Use(int index)
    {
        if (!ValidIndex(index) || slots[index] == null) return;
        if (string.IsNullOrEmpty(slotUID[index])) return;

        if (stats == null)
            stats = PlayerStatsManager.Instance ?? FindAnyObjectByType<PlayerStatsManager>();

        int hp = Mathf.RoundToInt(cachedHP[index]);
        int mp = Mathf.RoundToInt(cachedMP[index]);

        if (stats != null) { stats.Heal(hp); stats.RestoreMana(mp); }

        Clear(index); // 내부에서 Save 호출
    }

    public void ReturnToInventory(int index, bool refreshUI = true)
    {
        if (!ValidIndex(index)) return;
        if (string.IsNullOrEmpty(slotUID[index])) return;

        // 슬롯 캐시로 InventoryItem 재구성
        var dataMgr = DataManager.Instance;
        if (!dataMgr || !dataMgr.dicItemDatas.ContainsKey(slotItemId[index]))
        {
            Debug.LogWarning("[PotionQuickBar] DataManager에 itemId가 없어 반환 실패");
            return;
        }

        var item = new InventoryItem
        {
            uniqueId = slotUID[index],           // 기존 UID 유지
            id = slotItemId[index],
            data = dataMgr.dicItemDatas[slotItemId[index]],
            iconPath = slotIconPath[index],
            prefabPath = slotPrefabPath[index]
        };

        if (!inventoryPresenter)
            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();

        inventoryPresenter?.AddExistingItem(item);

        Clear(index); // 슬롯 비움 + Save
    }


    public PotionQuickBarSave ToSaveData()
    {
        var save = new PotionQuickBarSave();
        for (int i = 0; i < slots.Length; i++)
        {
            if (string.IsNullOrEmpty(slotUID[i])) continue;
            save.slots.Add(new PotionSlotEntry
            {
                index = i,
                uniqueId = slotUID[i],
                itemId = slotItemId[i],
                iconPath = slotIconPath[i],
                prefabPath = slotPrefabPath[i],
                hp = cachedHP[i],
                mp = cachedMP[i]
            });
        }
        return save;
    }

    public void ApplySaveData(PotionQuickBarSave save)
    {
        for (int i = 0; i < slots.Length; i++) Clear(i);

        if (save == null) return;

        foreach (var e in save.slots)
        {
            if (!ValidIndex(e.index)) continue;

            slotUID[e.index] = e.uniqueId;
            slotItemId[e.index] = e.itemId;
            slotIconPath[e.index] = e.iconPath;
            slotPrefabPath[e.index] = e.prefabPath;
            cachedHP[e.index] = e.hp;
            cachedMP[e.index] = e.mp;

            Sprite sp = null;
            if (!string.IsNullOrEmpty(e.iconPath))
                sp = Resources.Load<Sprite>(e.iconPath);

            slots[e.index].SetBySave(e.uniqueId, sp);
        }

        // 로드 후에도 저장 한 번(정합성 보장 차원, 선택)
        PotionQuickBarPersistence.Save(ToSaveData());
    }


    public bool TryGetSlotIndexAtScreenPosition(Vector2 screenPos, out int index)
    {
        index = -1;
        var cam = slots[0] ? slots[0].GetCanvasCamera() : null;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            var rt = slots[i].GetRect();
            if (rt && RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos, cam))
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    private bool ValidIndex(int i) => i >= 0 && i < (slots?.Length ?? 0);

    private void AutoWireByHierarchy()
    {
        var canvas = GameObject.Find("ItemCanvas");
        if (!canvas) return;
        var potionUI = canvas.transform.Find("PotionUI");
        if (!potionUI) return;

        slots = new PotionSlotUI[4];
        for (int i = 0; i < 4; i++)
        {
            var panel = potionUI.Find($"Potion{i + 1}");
            if (!panel) continue;

            var slot = panel.GetComponent<PotionSlotUI>();
            if (!slot) slot = panel.gameObject.AddComponent<PotionSlotUI>();
            slot.index = i;
            slot.AutoWireIconByChildName($"{i + 1}");

            // 아이콘에 드래그 핸들 자동 부착
            if (slot.icon && !slot.icon.gameObject.GetComponent<QuickSlotDraggable>())
            {
                var d = slot.icon.gameObject.AddComponent<QuickSlotDraggable>();
                d.slot = slot;
                d.canvas = canvas.GetComponent<Canvas>();
            }

            slots[i] = slot;
        }
    }
}
