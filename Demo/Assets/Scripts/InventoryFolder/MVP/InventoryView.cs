using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 UI 갱신 및 버튼/드래그 연결
/// </summary>
public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;   // 인벤토리 전체 패널
    [SerializeField] private Transform buttonContainer;   // 슬롯 버튼 컨테이너
    [SerializeField] private Button exitButton;           // 닫기 버튼

    private Button[] inventoryButtons;

    /// <summary>
    /// 초기화 - 닫기 버튼 등록
    /// </summary>
    public void Initialize(Action onExit)
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(() => onExit?.Invoke());

        inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        Show(false);
    }

    /// <summary>
    /// UI 표시/숨김
    /// </summary>
    public void Show(bool show)
    {
        inventoryPanel?.SetActive(show);
    }

    /// <summary>
    /// 아이템 리스트 기반 UI 갱신
    /// </summary>
    public void UpdateInventoryUI(IReadOnlyList<InventoryItem> items, Action<int, int> onItemDropped,
        Action<int> onItemRemoved, Action<int> equipCallback)
    {
        // 모든 버튼 숨김
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        for (int i = 0; i < items.Count && i < inventoryButtons.Length; i++)
        {
            var item = items[i];
            var button = inventoryButtons[i];

            button.gameObject.SetActive(true);

            // 아이콘 적용
            var image = button.GetComponent<Image>();
            if (image != null && !string.IsNullOrEmpty(item.iconPath))
            {
                var icon = Resources.Load<Sprite>(item.iconPath);
                if (icon != null)
                    image.sprite = icon;
            }

            // 드래그 처리 연결
            var draggable = button.GetComponent<DraggableItemView>();
            if (draggable == null)
                draggable = button.gameObject.AddComponent<DraggableItemView>();

            // equipCallback (Action<int>) 를 Draggable이 기대하는 Action<int, ItemOrigin> 으로 래핑
            Action<int, ItemOrigin> wrappedEquip = null;
            if (equipCallback != null)
                wrappedEquip = (idx, origin) => equipCallback.Invoke(idx);

            draggable.Initialize(
                item,                       // InventoryItem
                ItemOrigin.Inventory,       // 원본 위치
                i,                          // 슬롯 인덱스
                onItemDropped,              // 드롭 콜백
                onItemRemoved,              // 삭제 콜백
                wrappedEquip,               // 장착 콜백
                null                        // 해제 콜백 (인벤토리에서는 필요없음)
            );
        }
    }
}
