////using UnityEngine;
////using UnityEngine.EventSystems;
////using System.Linq; // ★ 추가

////public class ItemHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
////{
////    private InventoryItem item;
////    private RectTransform selfRect;

////    void Awake()
////    {
////        selfRect = transform as RectTransform;
////    }

////    // 패널(아이템 슬롯/창)이 비활성화되면 즉시 툴팁 숨김
////    void OnDisable()
////    {
////        if (ItemTooltipUI.Instance != null)
////            ItemTooltipUI.Instance.Hide(this);
////    }

////    public void SetItem(InventoryItem it) => item = it;

////    //public void OnPointerEnter(PointerEventData eventData)
////    //{
////    //    if (item != null)
////    //        ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this);
////    //}

////    //public void OnPointerExit(PointerEventData eventData)
////    //{
////    //    ItemTooltipUI.Instance?.Hide();
////    //}
////    public void OnPointerEnter(PointerEventData eventData)
////    {
////        if (item == null)
////        {
////            ItemTooltipUI.Instance?.Hide(this);
////            return;
////        }

////        // 1) 같은 슬롯의 착용 장비 찾기
////        InventoryItem equipped = null;
////        var equipPresenter = Object.FindAnyObjectByType<EquipmentPresenter>();
////        if (equipPresenter != null)
////        {
////            var slots = equipPresenter.GetEquipmentSlots();
////            if (slots != null)
////            {
////                var same = slots.FirstOrDefault(s => s.slotType == item.data.type);
////                equipped = same?.equipped;
////            }
////        }

////        // 2) 비교 호출 (있으면 비교, 없으면 기존)
////        if (equipped != null)
////        {
////            ItemTooltipUI.Instance?.ShowNextToWithCompare(item, equipped, selfRect, this);
////        }
////        else
////        {
////            ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this);
////        }
////    }

////    public void OnPointerExit(PointerEventData eventData)
////    {
////        ItemTooltipUI.Instance?.Hide(this);
////    }
////}
//// ItemHoverTooltip.cs

//using UnityEngine;
//using UnityEngine.EventSystems;
//using System.Linq;

//public class ItemHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    private InventoryItem item;
//    private RectTransform selfRect;

//    // ▼ 추가: 어디서 뜬 툴팁인지(인벤/장비) 컨텍스트
//    private ItemOrigin context = ItemOrigin.Inventory;
//    public void SetContext(ItemOrigin origin) => context = origin;

//    void Awake()
//    {
//        selfRect = transform as RectTransform;
//    }

//    void OnDisable()
//    {
//        if (ItemTooltipUI.Instance != null)
//            ItemTooltipUI.Instance.Hide(this);
//    }

//    public void SetItem(InventoryItem it) => item = it;

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (item == null)
//        {
//            ItemTooltipUI.Instance?.Hide(this);
//            return;
//        }

//        // 장비창이면: 비교 없이 단일 툴팁만
//        if (context == ItemOrigin.Equipment)
//        {
//            ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this);
//            return;
//        }

//        // 인벤토리면: 같은 슬롯에 착용 중인 장비가 있을 때만 비교
//        InventoryItem equipped = null;
//        var equipPresenter = Object.FindAnyObjectByType<EquipmentPresenter>();
//        if (equipPresenter != null)
//        {
//            var slots = equipPresenter.GetEquipmentSlots();
//            if (slots != null)
//            {
//                var same = slots.FirstOrDefault(s => s.slotType == item.data.type);
//                equipped = same?.equipped;
//            }
//        }

//        if (equipped != null)
//            ItemTooltipUI.Instance?.ShowNextToWithCompare(item, equipped, selfRect, this);
//        else
//            ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this);
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        ItemTooltipUI.Instance?.Hide(this);
//    }
//}
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class ItemHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryItem item;
    private RectTransform selfRect;

    private ItemOrigin context = ItemOrigin.Inventory;
    public void SetContext(ItemOrigin origin) => context = origin;

    void Awake() => selfRect = transform as RectTransform;

    void OnDisable()
    {
        if (ItemTooltipUI.Instance != null)
            ItemTooltipUI.Instance.Hide(this);
    }

    public void SetItem(InventoryItem it) => item = it;

    // ★ 착용 아이템 유효성 검사 (유령 레퍼런스 차단)
    private static bool IsValidEquipped(InventoryItem it)
        => it != null
        && it.data != null
        && !string.IsNullOrEmpty(it.uniqueId)
        && it.id != 0;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
        {
            ItemTooltipUI.Instance?.Hide(this);
            return;
        }

        // 장비창: 비교 없이 단일 툴팁
        if (context == ItemOrigin.Equipment)
        {
            ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this); // 내부에서 compareRoot off
            return;
        }

        // 인벤: 같은 슬롯의 "진짜" 착용 아이템만 비교
        InventoryItem equipped = null;
        var equipPresenter = Object.FindAnyObjectByType<EquipmentPresenter>();
        if (equipPresenter != null)
        {
            var slots = equipPresenter.GetEquipmentSlots();
            if (slots != null)
            {
                var same = slots.FirstOrDefault(s => s.slotType == item.data.type);
                if (same != null && IsValidEquipped(same.equipped))
                    equipped = same.equipped;
            }
        }

        if (IsValidEquipped(equipped))
            ItemTooltipUI.Instance?.ShowNextToWithCompare(item, equipped, selfRect, this);
        else
            ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this); // compareRoot off
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide(this);
    }
}
