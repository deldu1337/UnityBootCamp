using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlotView : MonoBehaviour, IDropHandler
{
    public string slotType; // "head", "weapon", "shield" ��
    public Action<string, InventoryItem> onItemDropped;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<DraggableItemView>();
        if (draggedItem == null || draggedItem.Item == null)
            return;

        // ���� Ÿ���� ������ Ÿ�԰� ��ġ�ϴ��� Ȯ��
        if (draggedItem.Item.data.type == slotType)
        {
            Debug.Log($"��� ���� �� {slotType} ���Կ� {draggedItem.Item.data.name}");
            onItemDropped?.Invoke(slotType, draggedItem.Item);
        }
        else
        {
            Debug.LogWarning($"��� ����: {slotType} ������ {draggedItem.Item.data.type} �������� ������ �� ����");
        }
    }
}
