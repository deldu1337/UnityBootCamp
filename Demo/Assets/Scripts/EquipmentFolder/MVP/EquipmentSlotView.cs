using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EquipmentSlotView : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public string slotType; // ���� Ÿ��
    public Action<string, InventoryItem> onItemDropped; // ��� �̺�Ʈ �ݹ�

    /// <summary>������ �巡�� �� ��� ó��</summary>
    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<DraggableItemView>();
        if (draggedItem == null || draggedItem.Item == null)
            return;

        // ���� Ÿ�԰� ������ Ÿ�� ��ġ Ȯ��
        if (draggedItem.Item.data.type == slotType)
        {
            Debug.Log($"��� ���� �� {slotType} ���Կ� {draggedItem.Item.data.name}");
            onItemDropped?.Invoke(slotType, draggedItem.Item);
        }
        else
        {
            Debug.LogWarning($"��� ����: {slotType} ������ {draggedItem.Item.data.type} ������ ���� �Ұ�");
        }
    }

    /// <summary>���� Ŭ�� ���� (��Ŭ�� �� ���� Ȯ�� ����)</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Click detected on {slotType}: {eventData.button}");
    }
}
