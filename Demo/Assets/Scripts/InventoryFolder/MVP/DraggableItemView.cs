using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// 아이템의 출처를 구분 (인벤토리 / 장비창)
/// </summary>
public enum ItemOrigin
{
    Inventory,
    Equipment
}

/// <summary>
/// 드래그 및 클릭 가능한 인벤토리/장비 UI 아이템 뷰
/// </summary>
public class DraggableItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    // 이벤트 콜백
    public Action<int, ItemOrigin> onItemEquipped;   // 인벤토리 → 장비창 장착
    public Action<int, ItemOrigin> onItemUnequipped; // 장비창 → 인벤토리 해제
    public Action<int, int> onItemDropped;           // (fromIndex, toIndex) 전달
    public Action<int> onItemRemoved;               // 인벤토리/장비창 외부 삭제

    // 아이템 데이터
    public InventoryItem Item { get; private set; }

    private ItemOrigin originType;          // 아이템 출처 (Inventory / Equipment)
    private Canvas canvas;                  // 최상위 UI 캔버스
    private RectTransform rectTransform;    // 드래그용 RectTransform
    private CanvasGroup canvasGroup;        // 드래그 시 Raycast 처리
    private Transform originalParent;       // 드래그 시작 시 원래 부모
    private int originalIndex;              // 드래그 시작 시 원래 인덱스

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 아이템 뷰 초기화
    /// </summary>
    /// <param name="item">인벤토리 아이템 데이터</param>
    /// <param name="origin">아이템 출처</param>
    /// <param name="slotIndex">슬롯 인덱스</param>
    /// <param name="dropCallback">드래그앤드롭 콜백</param>
    /// <param name="removeCallback">삭제 콜백</param>
    /// <param name="equipCallback">장착 콜백</param>
    /// <param name="unequipCallback">해제 콜백</param>
    public void Initialize(
        InventoryItem item,
        ItemOrigin origin,
        int slotIndex,
        Action<int, int> dropCallback = null,
        Action<int> removeCallback = null,
        Action<int, ItemOrigin> equipCallback = null,
        Action<int, ItemOrigin> unequipCallback = null
    )
    {
        Item = item;
        originType = origin;
        originalIndex = slotIndex;

        onItemDropped = dropCallback;
        onItemRemoved = removeCallback;
        onItemEquipped = equipCallback;
        onItemUnequipped = unequipCallback;
    }

    /// <summary>
    /// 마우스 클릭 처리 (우클릭 시 장착/해제)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭된 오브젝트 이름 확인
        string clickedObjectName = eventData.pointerPress != null
            ? eventData.pointerPress.name
            : eventData.pointerCurrentRaycast.gameObject?.name ?? "None";

        Debug.Log($"Click detected: {eventData.button} on {clickedObjectName}");

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        // 우클릭 시 장착/해제 처리
        if (originType == ItemOrigin.Inventory)
        {
            // 인벤토리 → 장비창
            onItemEquipped?.Invoke(originalIndex, originType);
            onItemRemoved?.Invoke(originalIndex); // 인벤토리에서 제거
            Debug.Log($"[우클릭] 인벤토리 슬롯 {originalIndex} → 장비창, 인벤토리에서 제거");
        }
        else if (originType == ItemOrigin.Equipment)
        {
            // 장비창 → 인벤토리
            onItemUnequipped?.Invoke(originalIndex, originType);
            onItemRemoved?.Invoke(originalIndex); // 장비창에서 제거
            Debug.Log($"[우클릭] 장비창 슬롯 {originalIndex} → 인벤토리, 장비창에서 제거");
        }
    }

    /// <summary>
    /// 드래그 시작 시 처리
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;      // 드래그 중 Raycast 차단
        transform.SetParent(canvas.transform, true); // 최상위 캔버스로 이동
    }

    /// <summary>
    /// 드래그 중 위치 갱신
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    /// <summary>
    /// 드래그 종료 시 처리
    /// - 인벤토리 내부 → 슬롯 교체
    /// - 장비창 슬롯 → 장착
    /// - UI 밖 → 삭제
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 1. 인벤토리 영역 확인
        RectTransform inventoryRect = originalParent as RectTransform;
        bool inInventory = RectTransformUtility.RectangleContainsScreenPoint(
            inventoryRect, eventData.position, canvas.worldCamera);

        // 2. 장비창 영역 확인
        bool inEquipmentSlot = false;
        Button targetSlot = null;
        var equipmentUI = GameObject.Find("EquipmentUI");
        if (equipmentUI != null)
        {
            var buttonPanel = equipmentUI.transform.Find("ButtonPanel");
            if (buttonPanel != null)
            {
                foreach (var button in buttonPanel.GetComponentsInChildren<Button>(true))
                {
                    RectTransform btnRect = button.GetComponent<RectTransform>();
                    if (RectTransformUtility.RectangleContainsScreenPoint(btnRect, eventData.position, canvas.worldCamera))
                    {
                        inEquipmentSlot = true;
                        targetSlot = button;
                        break;
                    }
                }
            }
        }

        // 3. 인벤토리 내 → 슬롯 교체
        if (inInventory)
        {
            int closestIndex = 0;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < originalParent.childCount; i++)
            {
                float dist = Vector2.SqrMagnitude(eventData.position - (Vector2)originalParent.GetChild(i).position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestIndex = i;
                }
            }

            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            onItemDropped?.Invoke(originalIndex, closestIndex);
        }
        // 4. 장비창 슬롯 → 장착 처리
        else if (inEquipmentSlot)
        {
            if (originType == ItemOrigin.Inventory)
            {
                transform.SetParent(originalParent, false);
                transform.SetSiblingIndex(originalIndex);
                onItemEquipped?.Invoke(originalIndex, originType);
                onItemRemoved?.Invoke(originalIndex); // 인벤토리에서 제거
                Debug.Log($"드래그 장착 → 인벤토리 슬롯 {originalIndex} 제거, 장비창 슬롯 '{targetSlot.name}' 장착");
            }
            else if (originType == ItemOrigin.Equipment)
            {
                // 장비창에서 드래그 → 장착 금지
                transform.SetParent(originalParent);
                transform.SetSiblingIndex(originalIndex);
            }
        }
        // 5. UI 밖 → 삭제 처리
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);
            onItemRemoved?.Invoke(originalIndex);
            Debug.Log("OnEndDrag - Removed Item (Outside Inventory & Equipment)");
        }
    }
}
