using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum ItemOrigin
{
    Inventory,
    Equipment
}

public class DraggableItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    public Action<string, ItemOrigin> onItemEquipped;    // �κ��丮 �� ���â
    public Action<string, ItemOrigin> onItemUnequipped;  // ���â �� �κ��丮
    public Action<string, string> onItemDropped;         // ���� ���� (fromId, toId)
    public Action<string> onItemRemoved;                // ���� �̺�Ʈ

    public InventoryItem Item { get; private set; }

    private string uniqueId;
    private ItemOrigin originType;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalIndex;

    private GameObject placeholder; // �� �������� �� placeholder

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(
        InventoryItem item,
        ItemOrigin origin,
        Action<string, string> dropCallback = null,
        Action<string> removeCallback = null,
        Action<string, ItemOrigin> equipCallback = null,
        Action<string, ItemOrigin> unequipCallback = null
    )
    {
        Item = item;
        uniqueId = item.uniqueId;
        originType = origin;

        onItemDropped = dropCallback;
        onItemRemoved = removeCallback;
        onItemEquipped = equipCallback;
        onItemUnequipped = unequipCallback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;

        if (originType == ItemOrigin.Inventory)
        {
            onItemEquipped?.Invoke(uniqueId, originType);
        }
        else if (originType == ItemOrigin.Equipment)
        {
            onItemUnequipped?.Invoke(uniqueId, originType);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        // Ȱ��ȭ�� ���� ���� ���
        int activeCount = 0;
        for (int i = 0; i < originalParent.childCount; i++)
        {
            if (originalParent.GetChild(i).gameObject.activeSelf)
                activeCount++;
        }

        // placeholder ����
        placeholder = new GameObject("Placeholder");
        var placeholderRect = placeholder.AddComponent<RectTransform>();
        placeholderRect.sizeDelta = rectTransform.sizeDelta;
        placeholder.transform.SetParent(originalParent);
        placeholder.transform.SetSiblingIndex(activeCount); // Ȱ�� ���� ������ ���� �ε���

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true); // �ֻ��� ĵ������ �̵�
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 1. �κ��丮 ���� Ȯ��
        RectTransform inventoryRect = originalParent as RectTransform;
        bool inInventory = RectTransformUtility.RectangleContainsScreenPoint(
            inventoryRect, eventData.position, canvas.worldCamera);

        // 2. ���â ���� Ȯ��
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

        // placeholder ��ġ�� ������ �̵�
        int newIndex = placeholder.transform.GetSiblingIndex();

        // 3. �κ��丮 �� �� ���� ��ü
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

            string toId = null;
            if (closestIndex < originalParent.childCount)
            {
                if (newIndex == closestIndex)
                {
                    var target = originalParent.GetChild(closestIndex).GetComponent<DraggableItemView>();
                    if (target != null)
                        toId = target.Item.uniqueId;
                }
                else
                {
                    var target = originalParent.GetChild(closestIndex).GetComponent<DraggableItemView>();
                    if (target != null)
                        toId = target.Item.uniqueId;
                }
            }

            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            onItemDropped?.Invoke(uniqueId, toId);

            Destroy(placeholder);
        }
        // 4. ���â ���� �� ���� ó��
        else if (inEquipmentSlot)
        {
            if (originType == ItemOrigin.Inventory)
            {
                transform.SetParent(originalParent, false);
                transform.SetSiblingIndex(originalIndex);
                onItemEquipped?.Invoke(uniqueId, originType);
                Debug.Log($"�巡�� ���� �� �κ��丮 ���� {originalIndex} ����, ���â ���� '{targetSlot.name}' ����");
            }
            else if (originType == ItemOrigin.Equipment)
            {
                // ���â���� �巡�� �� ���� ����
                transform.SetParent(originalParent);
                transform.SetSiblingIndex(originalIndex);
            }
        }
        // 5. UI �� �� ���� ó��
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);
            onItemRemoved?.Invoke(uniqueId);
            Debug.Log("OnEndDrag - Removed Item (Outside Inventory & Equipment)");
        }
    }
}

