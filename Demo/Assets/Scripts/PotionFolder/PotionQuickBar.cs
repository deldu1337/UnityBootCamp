//using System;
//using UnityEngine;

//public class PotionQuickBar : MonoBehaviour
//{
//    public static PotionQuickBar Instance { get; private set; }

//    [Header("Slots (0~3)")]
//    public PotionSlotUI[] slots = new PotionSlotUI[4];

//    [Header("�ɼ�")]
//    public KeyCode key1 = KeyCode.Alpha1;
//    public KeyCode key2 = KeyCode.Alpha2;
//    public KeyCode key3 = KeyCode.Alpha3;
//    public KeyCode key4 = KeyCode.Alpha4;

//    [SerializeField] private InventoryPresenter inventoryPresenter; // �ν����ͷ� ���� ����
//    private PlayerStatsManager stats;
//    private DraggableItemView draggableItemView;

//    // ���Ժ� ĳ��
//    private string[] slotUID = new string[4];
//    private int[] slotItemId = new int[4];
//    private string[] slotIconPath = new string[4];
//    private string[] slotPrefabPath = new string[4];

//    private float[] cachedHP = new float[4];
//    private float[] cachedMP = new float[4];

//    private static int checkCount = 1; // Assign ���� ȣ�� �̽� �ذ� ����

//    // �̺�Ʈ(����): ���� ���� ���� �� �ٱ����� ��ŷ�ϰ� ������
//    public event System.Action OnChanged;

//    void Awake() => Instance = this;

//    void Start()
//    {
//        AutoWireByHierarchy();
//        if (!inventoryPresenter) inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
//        stats = PlayerStatsManager.Instance ?? FindAnyObjectByType<PlayerStatsManager>();
//        draggableItemView = FindAnyObjectByType<DraggableItemView>();

//        for (int i = 0; i < slots.Length; i++)
//            slots[i]?.Clear();

//        // ���� �ҷ��� ����
//        var save = PotionQuickBarPersistence.Load();
//        if (save != null) ApplySaveData(save);
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(key1)) Use(0);
//        if (Input.GetKeyDown(key2)) Use(1);
//        if (Input.GetKeyDown(key3)) Use(2);
//        if (Input.GetKeyDown(key4)) Use(3);
//    }

//    //public void Assign(int index, InventoryItem item, Sprite icon)
//    //{
//    //    // �κ��丮 -> �� ���� �� Assign ���� ȣ�� �Ǵ� �̽� �ذ�
//    //    if (checkCount % 2 == 0)
//    //    {
//    //        checkCount++;
//    //        return;
//    //    }
//    //    if (!ValidIndex(index) || item == null || item.data == null) return;
//    //    if (!string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase)) return;

//    //    // ���� ���Կ� ���� ������ ������ ���� �κ��丮�� ��ȯ(������ ���� ����)
//    //    if (!string.IsNullOrEmpty(slotUID[index]))
//    //        ReturnToInventory(index, refreshUI: false); // �Ʒ��� ����

//    //    // 1) ���� UI/ĳ�� ����
//    //    slots[index].Set(item, icon);
//    //    slotUID[index] = item.uniqueId;
//    //    slotItemId[index] = item.id;
//    //    slotIconPath[index] = item.iconPath;
//    //    slotPrefabPath[index] = item.prefabPath;
//    //    cachedHP[index] = item.data.hp;
//    //    cachedMP[index] = item.data.mp;

//    //    // 2) �κ��丮���� ����(�̰�)
//    //    if (!inventoryPresenter)
//    //        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
//    //    inventoryPresenter?.RemoveItemFromInventory(item.uniqueId);

//    //    // 3) ����
//    //    PotionQuickBarPersistence.Save(ToSaveData());

//    //    checkCount++;
//    //}
//    public void Assign(int index, InventoryItem item, Sprite icon)
//    {
//        if (!ValidIndex(index) || item == null || item.data == null)
//        {
//            Debug.LogWarning($"[PotionQuickBar] invalid index {index}");
//            return;
//        }
//        if (!string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase)) return;

//        // ����� ��: ���� ������ �κ��丮�� ����
//        if (!string.IsNullOrEmpty(slotUID[index]))
//            ReturnToInventory(index, refreshUI: false);

//        // 1) ���� UI/ĳ�� ����
//        slots[index].Set(item, icon);
//        slotUID[index] = item.uniqueId;
//        slotItemId[index] = item.id;
//        slotIconPath[index] = item.iconPath;
//        slotPrefabPath[index] = item.prefabPath;
//        cachedHP[index] = item.data.hp;
//        cachedMP[index] = item.data.mp;

//        // 2) �κ��丮���� ����(�̰�)
//        if (!inventoryPresenter)
//            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
//        inventoryPresenter?.RemoveItemFromInventory(item.uniqueId);

//        // 3) ����
//        PotionQuickBarPersistence.Save(ToSaveData());

//        // 4) �ܺ� �̺�Ʈ(�ʿ��)
//        OnChanged?.Invoke();
//    }


//    public void Move(int from, int to)
//    {
//        if (!ValidIndex(from) || !ValidIndex(to) || from == to) return;
//        var a = slots[from]; var b = slots[to]; if (!a || !b) return;

//        (a.boundUniqueId, b.boundUniqueId) = (b.boundUniqueId, a.boundUniqueId);
//        (slotUID[from], slotUID[to]) = (slotUID[to], slotUID[from]);
//        (slotItemId[from], slotItemId[to]) = (slotItemId[to], slotItemId[from]);
//        (slotIconPath[from], slotIconPath[to]) = (slotIconPath[to], slotIconPath[from]);
//        (slotPrefabPath[from], slotPrefabPath[to]) = (slotPrefabPath[to], slotPrefabPath[from]);
//        (cachedHP[from], cachedHP[to]) = (cachedHP[to], cachedHP[from]);
//        (cachedMP[from], cachedMP[to]) = (cachedMP[to], cachedMP[from]);

//        var spA = a.icon ? a.icon.sprite : null;
//        var spB = b.icon ? b.icon.sprite : null;
//        if (a.icon) { a.icon.sprite = spB; a.icon.enabled = (spB != null) && !string.IsNullOrEmpty(a.boundUniqueId); }
//        if (b.icon) { b.icon.sprite = spA; b.icon.enabled = (spA != null) && !string.IsNullOrEmpty(b.boundUniqueId); }

//        a.RefreshEmptyOverlay(); b.RefreshEmptyOverlay();

//        PotionQuickBarPersistence.Save(ToSaveData());
//    }


//    public void Clear(int index)
//    {
//        if (!ValidIndex(index) || slots[index] == null) return;

//        slots[index].Clear();
//        slotUID[index] = null;
//        slotItemId[index] = 0;
//        slotIconPath[index] = null;
//        slotPrefabPath[index] = null;
//        cachedHP[index] = cachedMP[index] = 0f;

//        PotionQuickBarPersistence.Save(ToSaveData());
//    }


//    public void Use(int index)
//    {
//        if (!ValidIndex(index) || slots[index] == null) return;
//        if (string.IsNullOrEmpty(slotUID[index])) return;

//        if (stats == null)
//            stats = PlayerStatsManager.Instance ?? FindAnyObjectByType<PlayerStatsManager>();

//        int hp = Mathf.RoundToInt(cachedHP[index]);
//        int mp = Mathf.RoundToInt(cachedMP[index]);

//        if (stats != null) { stats.Heal(hp); stats.RestoreMana(mp); }

//        Clear(index); // ���ο��� Save ȣ��
//    }

//    public void ReturnToInventory(int index, bool refreshUI = true)
//    {
//        if (!ValidIndex(index)) return;
//        if (string.IsNullOrEmpty(slotUID[index])) return;

//        // ���� ĳ�÷� InventoryItem �籸��
//        var dataMgr = DataManager.Instance;
//        if (!dataMgr || !dataMgr.dicItemDatas.ContainsKey(slotItemId[index]))
//        {
//            Debug.LogWarning("[PotionQuickBar] DataManager�� itemId�� ���� ��ȯ ����");
//            return;
//        }

//        var item = new InventoryItem
//        {
//            uniqueId = slotUID[index],           // ���� UID ����
//            id = slotItemId[index],
//            data = dataMgr.dicItemDatas[slotItemId[index]],
//            iconPath = slotIconPath[index],
//            prefabPath = slotPrefabPath[index]
//        };

//        if (!inventoryPresenter)
//            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();

//        inventoryPresenter?.AddExistingItem(item);

//        Clear(index); // ���� ��� + Save
//    }


//    public PotionQuickBarSave ToSaveData()
//    {
//        var save = new PotionQuickBarSave();
//        for (int i = 0; i < slots.Length; i++)
//        {
//            if (string.IsNullOrEmpty(slotUID[i])) continue;
//            save.slots.Add(new PotionSlotEntry
//            {
//                index = i,
//                uniqueId = slotUID[i],
//                itemId = slotItemId[i],
//                iconPath = slotIconPath[i],
//                prefabPath = slotPrefabPath[i],
//                hp = cachedHP[i],
//                mp = cachedMP[i]
//            });
//        }
//        return save;
//    }

//    public void ApplySaveData(PotionQuickBarSave save)
//    {
//        for (int i = 0; i < slots.Length; i++) Clear(i);

//        if (save == null) return;

//        foreach (var e in save.slots)
//        {
//            if (!ValidIndex(e.index)) continue;

//            slotUID[e.index] = e.uniqueId;
//            slotItemId[e.index] = e.itemId;
//            slotIconPath[e.index] = e.iconPath;
//            slotPrefabPath[e.index] = e.prefabPath;
//            cachedHP[e.index] = e.hp;
//            cachedMP[e.index] = e.mp;

//            Sprite sp = null;
//            if (!string.IsNullOrEmpty(e.iconPath))
//                sp = Resources.Load<Sprite>(e.iconPath);

//            slots[e.index].SetBySave(e.uniqueId, sp);
//        }

//        // �ε� �Ŀ��� ���� �� ��(���ռ� ���� ����, ����)
//        PotionQuickBarPersistence.Save(ToSaveData());
//    }


//    public bool TryGetSlotIndexAtScreenPosition(Vector2 screenPos, out int index)
//    {
//        index = -1;
//        var cam = slots[0] ? slots[0].GetCanvasCamera() : null;

//        for (int i = 0; i < slots.Length; i++)
//        {
//            if (slots[i] == null) continue;
//            var rt = slots[i].GetRect();
//            if (rt && RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos, cam))
//            {
//                index = i;
//                return true;
//            }
//        }
//        return false;
//    }

//    private bool ValidIndex(int i) => i >= 0 && i < (slots?.Length ?? 0);

//    private void AutoWireByHierarchy()
//    {
//        var canvas = GameObject.Find("ItemCanvas");
//        if (!canvas) return;
//        var potionUI = canvas.transform.Find("PotionUI");
//        if (!potionUI) return;

//        slots = new PotionSlotUI[4];
//        for (int i = 0; i < 4; i++)
//        {
//            var panel = potionUI.Find($"Potion{i + 1}");
//            if (!panel) continue;

//            var slot = panel.GetComponent<PotionSlotUI>();
//            if (!slot) slot = panel.gameObject.AddComponent<PotionSlotUI>();
//            slot.index = i;
//            slot.AutoWireIconByChildName($"{i + 1}");

//            // �����ܿ� �巡�� �ڵ� �ڵ� ����
//            if (slot.icon && !slot.icon.gameObject.GetComponent<QuickSlotDraggable>())
//            {
//                var d = slot.icon.gameObject.AddComponent<QuickSlotDraggable>();
//                d.slot = slot;
//                d.canvas = canvas.GetComponent<Canvas>();
//            }

//            slots[i] = slot;
//        }
//    }
//}
using System;
using UnityEngine;

public class PotionQuickBar : MonoBehaviour
{
    public static PotionQuickBar Instance { get; private set; }

    [Header("Slots (0~3)")]
    public PotionSlotUI[] slots = new PotionSlotUI[4];

    [Header("�ɼ�")]
    public KeyCode key1 = KeyCode.Alpha1;
    public KeyCode key2 = KeyCode.Alpha2;
    public KeyCode key3 = KeyCode.Alpha3;
    public KeyCode key4 = KeyCode.Alpha4;

    [SerializeField] private InventoryPresenter inventoryPresenter; // �ν����ͷ� ���� ����
    private PlayerStatsManager stats;

    // ���Ժ� ĳ��
    private string[] slotUID = new string[4];
    private int[] slotItemId = new int[4];
    private string[] slotIconPath = new string[4];
    private string[] slotPrefabPath = new string[4];

    private float[] cachedHP = new float[4];
    private float[] cachedMP = new float[4];

    // �̺�Ʈ(����): ���� ���� ���� �� �ٱ����� ��ŷ�ϰ� ������
    public event System.Action OnChanged;

    void Awake() => Instance = this;

    void Start()
    {
        AutoWireByHierarchy();
        if (!inventoryPresenter) inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        stats = PlayerStatsManager.Instance ?? FindAnyObjectByType<PlayerStatsManager>();

        for (int i = 0; i < slots.Length; i++)
            slots[i]?.Clear();

        // �� ���� �������� �ε� (������ ���Žÿ��� ���̱׷��̼�)
        var save = PotionQuickBarPersistence.LoadForRaceOrNew(CurrentRace());
        ApplySaveData(save);
    }

    /// <summary>
    /// ���� ������ ���̽� Ű. PlayerStatsManager�� ������ GameContext.SelectedRace�� ����.
    /// </summary>
    private string CurrentRace()
    {
        // 1) PlayerStatsManager�� ���� ���̽�
        var r = (stats != null && stats.Data != null) ? stats.Data.Race : null;
        if (!string.IsNullOrWhiteSpace(r)) return r.ToLower();

        // 2) ���� �ʱ�ȭ ���̸� ���� ���̽� ���
        var sel = GameContext.SelectedRace;
        if (!string.IsNullOrWhiteSpace(sel)) return sel.ToLower();

        // 3) ���� ������
        return "humanmale";
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

        // ����� �� ���� ���� �κ��丮�� ����
        if (!string.IsNullOrEmpty(slotUID[index]))
            ReturnToInventory(index, refreshUI: false);

        // ����/ĳ�� ����
        slots[index].Set(item, icon);
        slotUID[index] = item.uniqueId;
        slotItemId[index] = item.id;
        slotIconPath[index] = item.iconPath;
        slotPrefabPath[index] = item.prefabPath;
        cachedHP[index] = item.data.hp;
        cachedMP[index] = item.data.mp;

        // �κ����� ����
        if (!inventoryPresenter)
            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        inventoryPresenter?.RemoveItemFromInventory(item.uniqueId);

        // �� ������ ����
        PotionQuickBarPersistence.SaveForRace(CurrentRace(), ToSaveData());
        OnChanged?.Invoke();
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

        // �� ������ ����
        PotionQuickBarPersistence.SaveForRace(CurrentRace(), ToSaveData());
    }

    public void Clear(int index)
    {
        if (!ValidIndex(index) || slots[index] == null) return;

        slots[index].Clear();
        slotUID[index] = null;
        slotItemId[index] = 0;
        slotIconPath[index] = null;
        slotPrefabPath[index] = null;
        cachedHP[index] = 0f;
        cachedMP[index] = 0f;

        // �� ������ ����
        PotionQuickBarPersistence.SaveForRace(CurrentRace(), ToSaveData());
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

        Clear(index);
    }

    public void ReturnToInventory(int index, bool refreshUI = true)
    {
        if (!ValidIndex(index)) return;
        if (string.IsNullOrEmpty(slotUID[index])) return;

        // ���� ĳ�÷� InventoryItem �籸��
        var dataMgr = DataManager.Instance;
        if (!dataMgr || !dataMgr.dicItemDatas.ContainsKey(slotItemId[index]))
        {
            Debug.LogWarning("[PotionQuickBar] DataManager�� itemId�� ���� ��ȯ ����");
            return;
        }

        var item = new InventoryItem
        {
            uniqueId = slotUID[index],      // ���� UID ����
            id = slotItemId[index],
            data = dataMgr.dicItemDatas[slotItemId[index]],
            iconPath = slotIconPath[index],
            prefabPath = slotPrefabPath[index]
        };

        if (!inventoryPresenter)
            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        inventoryPresenter?.AddExistingItem(item);

        Clear(index); // ���ο��� �����
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

        // �ε� ���Ŀ��� ������ �������� ����ȭ(����)
        PotionQuickBarPersistence.SaveForRace(CurrentRace(), ToSaveData());
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

            // �����ܿ� �巡�� �ڵ� �ڵ� ����
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
