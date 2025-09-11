using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DraggableItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<int, int> onItemDropped; // (fromIndex, toIndex) ����
    public Action<int> onItemRemoved;      // �κ��丮 �ܺη� �巡�� �� ȣ�� (fromIndex)
    public Action<int> onItemEquipped;     // ��Ŭ�� �� ���â�� ����
    public InventoryItem Item { get; private set; }

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Action<int, int> dropCallback, int slotIndex,
        Action<int> removeCallback = null, Action<int> equipCallback = null,
        InventoryItem item = null)
    {
        onItemDropped = dropCallback;
        onItemRemoved = removeCallback;
        onItemEquipped = equipCallback;
        originalIndex = slotIndex;
        Item = item; // ������ ������ ����
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ��Ŭ�� �� ������ ���� �õ�
            onItemEquipped?.Invoke(originalIndex);
            Debug.Log($"��Ŭ��: ���� {originalIndex} ������ ���� �õ�");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    // ���� ���� ������ �巡���ߴ��� üũ
    //    bool isOutside = !RectTransformUtility.RectangleContainsScreenPoint(
    //        originalParent as RectTransform,
    //        eventData.position,
    //        canvas.worldCamera);

    //    if (isOutside)
    //    {
    //        // UI ��Ȱ��ȭ
    //        gameObject.SetActive(false);
    //        canvasGroup.blocksRaycasts = true;

    //        // ��Ȱ��ȭ �� ���� �θ��� �� �Ʒ��� �̵�
    //        transform.SetParent(originalParent, false);
    //        transform.SetSiblingIndex(originalIndex);

    //        // �κ��丮���� �� ����
    //        onItemRemoved?.Invoke(originalIndex);

    //        Debug.Log("OnEndDrag - Removed Item (Deactivated and moved to bottom)");
    //    }
    //    else
    //    {
    //        // ���� ����� ���� ã��
    //        int closestIndex = 0;
    //        float closestDistance = float.MaxValue;
    //        for (int i = 0; i < originalParent.childCount; i++)
    //        {
    //            float dist = Vector2.SqrMagnitude(eventData.position - (Vector2)originalParent.GetChild(i).position);
    //            if (dist < closestDistance)
    //            {
    //                closestDistance = dist;
    //                closestIndex = i;
    //            }
    //        }

    //        // ���� ��ġ�� ����
    //        transform.SetParent(originalParent);
    //        transform.SetSiblingIndex(originalIndex);
    //        canvasGroup.blocksRaycasts = true;

    //        // Presenter�� ������ ���� ��û
    //        onItemDropped?.Invoke(originalIndex, closestIndex);
    //    }
    //}
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 1. �κ��丮 ���� ���� Ȯ��
        RectTransform inventoryRect = originalParent as RectTransform;
        bool inInventory = RectTransformUtility.RectangleContainsScreenPoint(
            inventoryRect, eventData.position, canvas.worldCamera);

        // 2. ���â ���� ��ư ���� Ȯ��
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

        if (inInventory)
        {
            // �κ��丮 �� �� ���� ����� �������� �̵�
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
        else if (inEquipmentSlot)
        {
            // ���â ���� �� �� ���� �õ�
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            onItemEquipped?.Invoke(originalIndex);
            Debug.Log($"���â ���� '{targetSlot.name}' ���� �巡�� �� ���� �õ�");
        }
        else
        {
            // ��¥ UI �� �� ���� ó��
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);

            onItemRemoved?.Invoke(originalIndex);
            Debug.Log("OnEndDrag - Removed Item (Outside Inventory & Equipment)");
        }
    }


}
