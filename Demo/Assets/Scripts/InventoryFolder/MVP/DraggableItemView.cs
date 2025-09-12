using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

/// <summary>
/// �������� ��ó�� ���� (�κ��丮 / ���â)
/// </summary>
public enum ItemOrigin
{
    Inventory,
    Equipment
}

/// <summary>
/// �巡�� �� Ŭ�� ������ �κ��丮/��� UI ������ ��
/// </summary>
public class DraggableItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    // �̺�Ʈ �ݹ�
    public Action<int, ItemOrigin> onItemEquipped;   // �κ��丮 �� ���â ����
    public Action<int, ItemOrigin> onItemUnequipped; // ���â �� �κ��丮 ����
    public Action<int, int> onItemDropped;           // (fromIndex, toIndex) ����
    public Action<int> onItemRemoved;               // �κ��丮/���â �ܺ� ����

    // ������ ������
    public InventoryItem Item { get; private set; }

    private ItemOrigin originType;          // ������ ��ó (Inventory / Equipment)
    private Canvas canvas;                  // �ֻ��� UI ĵ����
    private RectTransform rectTransform;    // �巡�׿� RectTransform
    private CanvasGroup canvasGroup;        // �巡�� �� Raycast ó��
    private Transform originalParent;       // �巡�� ���� �� ���� �θ�
    private int originalIndex;              // �巡�� ���� �� ���� �ε���

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// ������ �� �ʱ�ȭ
    /// </summary>
    /// <param name="item">�κ��丮 ������ ������</param>
    /// <param name="origin">������ ��ó</param>
    /// <param name="slotIndex">���� �ε���</param>
    /// <param name="dropCallback">�巡�׾ص�� �ݹ�</param>
    /// <param name="removeCallback">���� �ݹ�</param>
    /// <param name="equipCallback">���� �ݹ�</param>
    /// <param name="unequipCallback">���� �ݹ�</param>
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
    /// ���콺 Ŭ�� ó�� (��Ŭ�� �� ����/����)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ���� ������Ʈ �̸� Ȯ��
        string clickedObjectName = eventData.pointerPress != null
            ? eventData.pointerPress.name
            : eventData.pointerCurrentRaycast.gameObject?.name ?? "None";

        Debug.Log($"Click detected: {eventData.button} on {clickedObjectName}");

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        // ��Ŭ�� �� ����/���� ó��
        if (originType == ItemOrigin.Inventory)
        {
            // �κ��丮 �� ���â
            onItemEquipped?.Invoke(originalIndex, originType);
            onItemRemoved?.Invoke(originalIndex); // �κ��丮���� ����
            Debug.Log($"[��Ŭ��] �κ��丮 ���� {originalIndex} �� ���â, �κ��丮���� ����");
        }
        else if (originType == ItemOrigin.Equipment)
        {
            // ���â �� �κ��丮
            onItemUnequipped?.Invoke(originalIndex, originType);
            onItemRemoved?.Invoke(originalIndex); // ���â���� ����
            Debug.Log($"[��Ŭ��] ���â ���� {originalIndex} �� �κ��丮, ���â���� ����");
        }
    }

    /// <summary>
    /// �巡�� ���� �� ó��
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;      // �巡�� �� Raycast ����
        transform.SetParent(canvas.transform, true); // �ֻ��� ĵ������ �̵�
    }

    /// <summary>
    /// �巡�� �� ��ġ ����
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    /// <summary>
    /// �巡�� ���� �� ó��
    /// - �κ��丮 ���� �� ���� ��ü
    /// - ���â ���� �� ����
    /// - UI �� �� ����
    /// </summary>
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

            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            onItemDropped?.Invoke(originalIndex, closestIndex);
        }
        // 4. ���â ���� �� ���� ó��
        else if (inEquipmentSlot)
        {
            if (originType == ItemOrigin.Inventory)
            {
                transform.SetParent(originalParent, false);
                transform.SetSiblingIndex(originalIndex);
                onItemEquipped?.Invoke(originalIndex, originType);
                onItemRemoved?.Invoke(originalIndex); // �κ��丮���� ����
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
            onItemRemoved?.Invoke(originalIndex);
            Debug.Log("OnEndDrag - Removed Item (Outside Inventory & Equipment)");
        }
    }
}
