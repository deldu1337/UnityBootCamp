using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlotView : MonoBehaviour, IDropHandler
{
    public string slotType; // "head", "weapon", "shield" 등
    public Action<string, InventoryItem> onItemDropped;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<DraggableItemView>();
        if (draggedItem == null || draggedItem.Item == null)
            return;

        // 슬롯 타입이 아이템 타입과 일치하는지 확인
        if (draggedItem.Item.data.type == slotType)
        {
            Debug.Log($"드롭 성공 → {slotType} 슬롯에 {draggedItem.Item.data.name}");
            onItemDropped?.Invoke(slotType, draggedItem.Item);
        }
        else
        {
            Debug.LogWarning($"드롭 실패: {slotType} 슬롯은 {draggedItem.Item.data.type} 아이템을 장착할 수 없음");
        }
    }
}
