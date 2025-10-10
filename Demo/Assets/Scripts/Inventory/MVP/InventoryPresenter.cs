////using System;
////using UnityEngine;

////public class InventoryPresenter : MonoBehaviour
////{
////    private InventoryModel model;
////    private InventoryView view;
////    private bool isOpen;

////    void Start()
////    {
////        view = FindAnyObjectByType<InventoryView>();
////        if (view == null) return;

////        model = new InventoryModel();
////        view.Initialize(CloseInventory);
////        isOpen = false;
////    }

////    void Update()
////    {
////        if (Input.GetKeyDown(KeyCode.I))
////            ToggleInventory();
////        if(Input.GetKeyDown(KeyCode.Escape))
////            CloseInventory();
////    }

////    private void ToggleInventory()
////    {
////        if (view == null) return;
////        isOpen = !isOpen;
////        view.Show(isOpen);
////        if (isOpen)
////            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
////    }

////    private void CloseInventory()
////    {
////        isOpen = false;
////        view.Show(false);
////    }

////    public void AddItem(int id, Sprite icon, string prefabPath)
////    {
////        var dataManager = DataManager.Instance;
////        if (!dataManager.dicItemDatas.ContainsKey(id))
////        {
////            Debug.LogWarning($"아이템 ID {id}가 DataManager에 없음!");
////            return;
////        }

////        var item = new InventoryItem
////        {
////            uniqueId = Guid.NewGuid().ToString(),
////            id = id,
////            data = dataManager.dicItemDatas[id],
////            iconPath = "Icons/" + icon.name,
////            prefabPath = prefabPath
////        };

////        // 최후 방어
////        if (InventoryGuards.IsInvalid(item))
////        {
////            Debug.LogWarning("[InventoryPresenter] 생성된 아이템이 무효 → 추가 취소");
////            return;
////        }

////        model.AddItem(item);
////        Refresh();
////    }

////    // 반환값으로 '실제 제거 성공' 여부를 알 수 있게 변경
////    public bool RemoveItemFromInventory(string uniqueId)
////    {
////        if (model == null) model = new InventoryModel(); // 안전망

////        var before = model.GetItemById(uniqueId) != null;
////        model.RemoveById(uniqueId);
////        var after = model.GetItemById(uniqueId) != null;

////        // 인벤 UI는 닫혀 있어도 강제로 한번 갱신(보이는 상태라면 즉시 반영)
////        ForceRefresh();

////        return before && !after;
////    }

////    public void ForceRefresh() => view?.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);

////    private void OnItemEquipped(string uniqueId)
////    {
////        var item = model.GetItemById(uniqueId);
////        if (item == null) return;

////        if (item.data != null && string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
////        {
////            var stats = PlayerStatsManager.Instance; // 싱글톤
////            if (stats != null)
////            {
////                if (item.data.hp > 0) stats.Heal(item.data.hp);
////                if (item.data.mp > 0) stats.RestoreMana(item.data.mp);
////            }
////            // 인벤토리에서 해당 포션 1개 제거
////            model.RemoveById(uniqueId);

////            // UI 갱신
////            Refresh();
////            return; // 장비 장착 로직으로 가지 않도록 종료
////        }

////        var equipPresenter = FindAnyObjectByType<EquipmentPresenter>();
////        equipPresenter?.HandleEquipItem(item);

////        Refresh();
////    }


////    private void OnItemRemoved(string uniqueId)
////    {
////        model.RemoveById(uniqueId);
////        Refresh();
////    }

////    private void OnItemDropped(string fromId, string toId)
////    {
////        model.ReorderByUniqueId(fromId, toId);
////        Refresh();
////    }

////    public void AddExistingItem(InventoryItem item)
////    {
////        model.Add(item);
////        Refresh();
////    }

////    public InventoryItem GetItemByUniqueId(string uniqueId)
////    {
////        // model.GetItemById는 이미 있으니, 그대로 래핑해도 됩니다.
////        return model.GetItemById(uniqueId);
////    }

////    //public void RemoveItemFromInventory(string uniqueId) => model.RemoveById(uniqueId);

////    public void Refresh()
////    {
////        if (isOpen)
////            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
////    }
////}

//using System;
//using UnityEngine;

//public class InventoryPresenter : MonoBehaviour
//{
//    private InventoryModel model;
//    private InventoryView view;
//    private bool isOpen;

//    void Start()
//    {
//        UIEscapeStack.GetOrCreate(); // 스택 보장
//        view = FindAnyObjectByType<InventoryView>();
//        if (view == null) return;

//        model = new InventoryModel();
//        view.Initialize(CloseInventory);
//        isOpen = false;
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.I))
//            ToggleInventory();

//        // 여기서 ESC로 닫지 않는다 (중앙 ESC 스택에서 처리)
//        // if (Input.GetKeyDown(KeyCode.Escape)) CloseInventory();
//    }

//    private void ToggleInventory()
//    {
//        if (view == null) return;
//        isOpen = !isOpen;
//        view.Show(isOpen);

//        if (isOpen)
//        {
//            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
//            UIEscapeStack.Instance.Push(
//                key: "inventory",
//                close: CloseInventory,
//                isOpen: () => isOpen
//            );
//        }
//        else
//        {
//            UIEscapeStack.Instance.Remove("inventory");
//        }
//    }

//    private void CloseInventory()
//    {
//        if (!isOpen) return;
//        isOpen = false;
//        view.Show(false);
//        UIEscapeStack.Instance.Remove("inventory");
//    }

//    /// <summary>
//    /// 아이템 “획득” 시점에 롤링을 확정해서 인벤토리에 저장.
//    /// </summary>
//    public void AddItem(int id, Sprite icon, string prefabPath)
//    {
//        var dataManager = DataManager.Instance;
//        if (dataManager == null || !dataManager.dicItemDatas.ContainsKey(id))
//        {
//            Debug.LogWarning($"아이템 ID {id}가 DataManager에 없음!");
//            return;
//        }

//        var baseData = dataManager.dicItemDatas[id];

//        var item = new InventoryItem
//        {
//            uniqueId = Guid.NewGuid().ToString(),
//            id = id,
//            data = baseData,
//            // 아이콘/프리팹 경로 캐싱
//            iconPath = icon ? "Icons/" + icon.name : null,
//            prefabPath = prefabPath,
//            // (중요) 획득 시점 롤링 확정
//            rolled = ItemRoller.CreateRolledStats(id)
//        };

//        if (InventoryGuards.IsInvalid(item))
//        {
//            Debug.LogWarning("[InventoryPresenter] 생성된 아이템이 무효 → 추가 취소");
//            return;
//        }

//        model.AddItem(item);
//        Refresh();
//    }

//    // 반환값으로 '실제 제거 성공' 여부를 알 수 있게 변경
//    public bool RemoveItemFromInventory(string uniqueId)
//    {
//        if (model == null) model = new InventoryModel(); // 안전망

//        var before = model.GetItemById(uniqueId) != null;
//        model.RemoveById(uniqueId);
//        var after = model.GetItemById(uniqueId) != null;

//        // 인벤 UI는 닫혀 있어도 강제로 한번 갱신(보이는 상태라면 즉시 반영)
//        ForceRefresh();

//        return before && !after;
//    }

//    public void ForceRefresh()
//        => view?.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);

//    private void OnItemEquipped(string uniqueId)
//    {
//        var item = model.GetItemById(uniqueId);
//        if (item == null) return;

//        // 포션은 즉시 사용
//        if (item.data != null && string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
//        {
//            var stats = PlayerStatsManager.Instance;
//            if (stats != null)
//            {
//                float hp = item.rolled != null && item.rolled.hasHp ? item.rolled.hp : item.data.hp;
//                float mp = item.rolled != null && item.rolled.hasMp ? item.rolled.mp : item.data.mp;
//                if (hp > 0) stats.Heal(hp);
//                if (mp > 0) stats.RestoreMana(mp);
//            }
//            model.RemoveById(uniqueId);
//            Refresh();
//            return;
//        }

//        // 장비 장착은 외부 프레젠터로
//        var equipPresenter = FindAnyObjectByType<EquipmentPresenter>();
//        equipPresenter?.HandleEquipItem(item);

//        Refresh();
//    }

//    private void OnItemRemoved(string uniqueId)
//    {
//        model.RemoveById(uniqueId);
//        Refresh();
//    }

//    private void OnItemDropped(string fromId, string toId)
//    {
//        model.ReorderByUniqueId(fromId, toId);
//        Refresh();
//    }

//    public void AddExistingItem(InventoryItem item)
//    {
//        model.Add(item);
//        Refresh();
//    }

//    public InventoryItem GetItemByUniqueId(string uniqueId)
//        => model.GetItemById(uniqueId);

//    public void Refresh()
//    {
//        if (isOpen)
//            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
//    }
//}
using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPresenter : MonoBehaviour
{
    private InventoryModel model;
    private InventoryView view;
    private bool isOpen;

    private Button InvenButton;

    void Start()
    {
        UIEscapeStack.GetOrCreate(); // 스택 보장
        view = FindAnyObjectByType<InventoryView>();
        if (view == null) return;

        // ★ 플레이어 종족 가져와서 종족별 인벤토리 사용
        var ps = PlayerStatsManager.Instance;
        string race = (ps != null && ps.Data != null && !string.IsNullOrEmpty(ps.Data.Race))
                        ? ps.Data.Race
                        : "humanmale";

        model = new InventoryModel(race);
        view.Initialize(CloseInventory);
        isOpen = false;

        InvenButton = GameObject.Find("QuickUI").transform.GetChild(3).GetComponent<Button>();
        InvenButton.onClick.AddListener(ToggleInventory);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
        // ESC는 중앙 스택에서 처리
    }

    private void ToggleInventory()
    {
        if (view == null) return;
        isOpen = !isOpen;
        view.Show(isOpen);

        if (isOpen)
        {
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
            UIEscapeStack.Instance.Push(
                key: "inventory",
                close: CloseInventory,
                isOpen: () => isOpen
            );
        }
        else
        {
            UIEscapeStack.Instance.Remove("inventory");
        }
    }

    private void CloseInventory()
    {
        if (!isOpen) return;
        isOpen = false;
        view.Show(false);
        UIEscapeStack.Instance.Remove("inventory");
    }

    /// <summary>아이템 “획득” 시점에 롤링을 확정해서 인벤토리에 저장.</summary>
    //public void AddItem(int id, Sprite icon, string prefabPath)
    //{
    //    var dataManager = DataManager.Instance;
    //    if (dataManager == null || !dataManager.dicItemDatas.ContainsKey(id))
    //    {
    //        Debug.LogWarning($"아이템 ID {id}가 DataManager에 없음!");
    //        return;
    //    }

    //    var baseData = dataManager.dicItemDatas[id];

    //    //var item = new InventoryItem
    //    //{
    //    //    uniqueId = Guid.NewGuid().ToString(),
    //    //    id = id,
    //    //    data = baseData,
    //    //    iconPath = icon ? "Icons/" + icon.name : null,
    //    //    prefabPath = prefabPath,
    //    //    rolled = ItemRoller.CreateRolledStats(id)
    //    //};

    //    // ★ 포션이면 먼저 합치기 시도
    //    if (baseData.type == "potion")
    //    {
    //        // 인벤에 같은 포션이 있으면 +1 하고 끝
    //        if (model.TryStackPotion(id, +1))
    //        {
    //            Refresh();
    //            return;
    //        }
    //    }

    //    // 새로 추가(포션이지만 기존 스택이 없을 때)
    //    var item = new InventoryItem
    //    {
    //        uniqueId = Guid.NewGuid().ToString(),
    //        id = id,
    //        data = baseData,
    //        iconPath = icon ? "Icons/" + icon.name : null,
    //        prefabPath = prefabPath,
    //        rolled = ItemRoller.CreateRolledStats(id),

    //        // ★ 스택 플래그/초기값
    //        stackable = baseData.type == "potion",
    //        quantity = 1,
    //        maxStack = 99
    //    };

    //    if (InventoryGuards.IsInvalid(item))
    //    {
    //        Debug.LogWarning("[InventoryPresenter] 생성된 아이템이 무효 → 추가 취소");
    //        return;
    //    }

    //    model.AddItem(item);
    //    Refresh();
    //}

    //public void AddItem(int id, Sprite icon, string prefabPath)
    //{
    //    var dataManager = DataManager.Instance;
    //    if (dataManager == null || !dataManager.dicItemDatas.ContainsKey(id))
    //    {
    //        Debug.LogWarning($"아이템 ID {id}가 DataManager에 없음!");
    //        return;
    //    }
    //    var baseData = dataManager.dicItemDatas[id];

    //    // ★ 1) 퀵슬롯에 같은 포션 있으면 거기에 +1
    //    if (baseData.type == "potion")
    //    {
    //        var qb = PotionQuickBar.Instance;
    //        if (qb != null && qb.TryAddToExistingSlot(id, +1))
    //        {
    //            // 퀵슬롯에 합쳐졌으니 인벤에는 추가하지 않음
    //            return;
    //        }
    //    }

    //    // ★ 2) 퀵슬롯에 없다면 인벤토리 스택 시도
    //    if (baseData.type == "potion")
    //    {
    //        if (model.TryStackPotion(id, +1))
    //        {
    //            Refresh();
    //            return;
    //        }
    //    }

    //    // ★ 3) 새 엔트리로 추가
    //    var item = new InventoryItem
    //    {
    //        uniqueId = Guid.NewGuid().ToString(),
    //        id = id,
    //        data = baseData,
    //        iconPath = icon ? "Icons/" + icon.name : null,
    //        prefabPath = prefabPath,
    //        rolled = ItemRoller.CreateRolledStats(id),
    //        stackable = baseData.type == "potion",
    //        quantity = 1,
    //        maxStack = 99
    //    };

    //    if (InventoryGuards.IsInvalid(item))
    //    {
    //        Debug.LogWarning("[InventoryPresenter] 생성된 아이템이 무효 → 추가 취소");
    //        return;
    //    }

    //    model.AddItem(item);
    //    Refresh();
    //}
    public void AddItem(int id, Sprite icon, string prefabPath)
    {
        var dataManager = DataManager.Instance;
        if (dataManager == null || !dataManager.dicItemDatas.ContainsKey(id))
        {
            Debug.LogWarning($"아이템 ID {id}가 DataManager에 없음!");
            return;
        }

        var baseData = dataManager.dicItemDatas[id];

        // ★ 1순위: 퀵슬롯 스택 증가 시도 (포션만)
        if (baseData.type == "potion" && PotionQuickBar.Instance != null)
        {
            if (PotionQuickBar.Instance.TryAddToExistingSlot(id, +1))
            {
                // 퀵슬롯 라벨은 TryAddToExistingSlot에서 갱신됨. 인벤 토글이 열려있다면 UI만 새로고침.
                Refresh();
                return;
            }
        }

        // ★ 2순위: 인벤토리 스택 증가 시도
        if (baseData.type == "potion")
        {
            if (model.TryStackPotion(id, +1))
            {
                Refresh();
                return;
            }
        }

        // 3순위: 새 아이템 생성
        var item = new InventoryItem
        {
            uniqueId = Guid.NewGuid().ToString(),
            id = id,
            data = baseData,
            iconPath = icon ? "Icons/" + icon.name : null,
            prefabPath = prefabPath,
            rolled = ItemRoller.CreateRolledStats(id),

            stackable = baseData.type == "potion",
            quantity = 1,
            maxStack = 99
        };

        if (InventoryGuards.IsInvalid(item))
        {
            Debug.LogWarning("[InventoryPresenter] 생성된 아이템이 무효 → 추가 취소");
            return;
        }

        model.AddItem(item);
        Refresh();
    }


    // 반환값으로 '실제 제거 성공' 여부
    public bool RemoveItemFromInventory(string uniqueId)
    {
        if (model == null)
        {
            var ps = PlayerStatsManager.Instance;
            string race = (ps != null && ps.Data != null && !string.IsNullOrEmpty(ps.Data.Race))
                            ? ps.Data.Race
                            : "humanmale";
            model = new InventoryModel(race); // 안전망
        }

        var before = model.GetItemById(uniqueId) != null;
        model.RemoveById(uniqueId);
        var after = model.GetItemById(uniqueId) != null;

        // 인벤 UI는 닫혀 있어도 강제로 갱신
        ForceRefresh();

        return before && !after;
    }

    public void ForceRefresh()
        => view?.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);

    private void OnItemEquipped(string uniqueId)
    {
        var item = model.GetItemById(uniqueId);
        if (item == null) return;

        //// 포션 즉시사용
        //if (item.data != null && string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
        //{
        //    var stats = PlayerStatsManager.Instance;
        //    if (stats != null)
        //    {
        //        float hp = item.rolled != null && item.rolled.hasHp ? item.rolled.hp : item.data.hp;
        //        float mp = item.rolled != null && item.rolled.hasMp ? item.rolled.mp : item.data.mp;
        //        if (hp > 0) stats.Heal(hp);
        //        if (mp > 0) stats.RestoreMana(mp);
        //    }
        //    model.RemoveById(uniqueId);
        //    Refresh();
        //    return;
        //}

        // ★ 포션 즉시사용(수량 1 감소)
        if (item.data != null && string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
        {
            var stats = PlayerStatsManager.Instance;
            if (stats != null)
            {
                float hp = item.rolled != null && item.rolled.hasHp ? item.rolled.hp : item.data.hp;
                float mp = item.rolled != null && item.rolled.hasMp ? item.rolled.mp : item.data.mp;
                if (hp > 0) stats.Heal(hp);
                if (mp > 0) stats.RestoreMana(mp);
            }

            // 기존엔 RemoveById였는데 → ★ 수량 감소 API로 변경
            model.ConsumePotionByUniqueId(uniqueId, 1);
            Refresh();
            return;
        }

        // 장비 장착은 장비 프레젠터에 위임
        var equipPresenter = FindAnyObjectByType<EquipmentPresenter>();
        equipPresenter?.HandleEquipItem(item);

        Refresh();
    }

    private void OnItemRemoved(string uniqueId)
    {
        model.RemoveById(uniqueId);
        Refresh();
    }

    private void OnItemDropped(string fromId, string toId)
    {
        model.ReorderByUniqueId(fromId, toId);
        Refresh();
    }

    public void AddExistingItem(InventoryItem item)
    {
        model.Add(item);
        Refresh();
    }

    public InventoryItem GetItemByUniqueId(string uniqueId)
        => model.GetItemById(uniqueId);

    public void Refresh()
    {
        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }
}

