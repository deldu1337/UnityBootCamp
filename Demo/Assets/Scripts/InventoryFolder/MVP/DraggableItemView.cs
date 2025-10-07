using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private InventoryPresenter inventoryPresenter;

    private GameObject placeholder; // �� �������� �� placeholder

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        if (!inventoryPresenter) inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
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
        if (InventoryGuards.IsInvalid(item))
        {
            Debug.LogWarning("[DraggableItemView] ��ȿ ���������� �ʱ�ȭ �õ� �� ��Ȱ��ȭ");
            gameObject.SetActive(false);
            return;
        }

        Item = item;
        uniqueId = item.uniqueId;
        originType = origin;

        onItemDropped = dropCallback;
        onItemRemoved = removeCallback;
        onItemEquipped = equipCallback;
        onItemUnequipped = unequipCallback;

    }

    public void SnapBackToOriginal()
    {
        if (originalParent)
        {
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[OnPointerClick] obj={gameObject.name}, origin={originType}, button={eventData.button}");
        
        // ��Ŭ���� ��� ���� ���� �� �׳� ����
        if (eventData.button == PointerEventData.InputButton.Left)
            return;

        if (eventData.button != PointerEventData.InputButton.Right) return;

        // 1. ���â ���� �ȿ��� �������� ���� �˻�
        bool inEquipmentSlot = false;
        string slotType = null;

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
                        slotType = button.name.Replace("Button", "").ToLower();
                        break;
                    }
                }
            }
        }

        // 2. �κ��丮 �������̸� �� ���� �õ�
        if (originType == ItemOrigin.Inventory && !inEquipmentSlot)
        {
            onItemEquipped?.Invoke(uniqueId, originType);
        }
        // 3. ���â ���� �ȿ��� ��Ŭ�� �� ���� �õ�
        else if (inEquipmentSlot)
        {
            Debug.Log($"[Equipment] {uniqueId} �� {slotType} ���� �õ�");
            onItemUnequipped?.Invoke(slotType, ItemOrigin.Equipment);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide(); // �� �߰�

        if (originType == ItemOrigin.Equipment)
        {
            // ���â������ �巡�� ����
            return;
        }

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
        if (originType == ItemOrigin.Equipment)
        {
            // ���â������ �巡�� ����
            return;
        }

        rectTransform.position = eventData.position;
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    ItemTooltipUI.Instance?.Hide(); // �� �߰�

    //    if (originType == ItemOrigin.Equipment)
    //    {
    //        // ���â������ �巡�� ����
    //        return;
    //    }

    //    canvasGroup.blocksRaycasts = true;

    //    // 1. �κ��丮 ���� Ȯ��
    //    RectTransform inventoryRect = originalParent as RectTransform;
    //    bool inInventory = RectTransformUtility.RectangleContainsScreenPoint(
    //        inventoryRect, eventData.position, canvas.worldCamera);

    //    // 2. ���â ���� Ȯ��
    //    bool inEquipmentSlot = false;
    //    Button targetSlot = null;
    //    var equipmentUI = GameObject.Find("EquipmentUI");
    //    if (equipmentUI != null)
    //    {
    //        var buttonPanel = equipmentUI.transform.Find("ButtonPanel");
    //        if (buttonPanel != null)
    //        {
    //            foreach (var button in buttonPanel.GetComponentsInChildren<Button>(true))
    //            {
    //                RectTransform btnRect = button.GetComponent<RectTransform>();
    //                if (RectTransformUtility.RectangleContainsScreenPoint(btnRect, eventData.position, canvas.worldCamera))
    //                {
    //                    inEquipmentSlot = true;
    //                    targetSlot = button;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    // placeholder ��ġ�� ������ �̵�
    //    int newIndex = placeholder.transform.GetSiblingIndex();

    //    bool inPotionSlot = false;
    //    int potionIndex = -1;
    //    Transform potionUI = null;

    //    var itemCanvas = GameObject.Find("ItemCanvas");
    //    if (itemCanvas != null)
    //    {
    //        potionUI = itemCanvas.transform.Find("PotionUI");
    //        if (potionUI != null)
    //        {
    //            // Potion1~4 �г� Rect �ȿ� ����ƴ��� �˻�
    //            for (int i = 0; i < 4; i++)
    //            {
    //                var pn = potionUI.Find($"Potion{i + 1}");
    //                if (!pn) continue;
    //                var pr = pn.GetComponent<RectTransform>();
    //                if (!pr) continue;

    //                if (RectTransformUtility.RectangleContainsScreenPoint(pr, eventData.position, canvas.worldCamera))
    //                {
    //                    inPotionSlot = true;
    //                    potionIndex = i;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    // 3. �κ��丮 �� �� ���� ��ü
    //    if (inInventory)
    //    {
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

    //        string toId = null;
    //        if (closestIndex < originalParent.childCount)
    //        {
    //            if (newIndex == closestIndex)
    //            {
    //                var target = originalParent.GetChild(closestIndex).GetComponent<DraggableItemView>();
    //                if (target != null)
    //                    toId = target.Item.uniqueId;
    //            }
    //            else
    //            {
    //                var target = originalParent.GetChild(closestIndex).GetComponent<DraggableItemView>();
    //                if (target != null)
    //                    toId = target.Item.uniqueId;
    //            }
    //        }

    //        transform.SetParent(originalParent);
    //        transform.SetSiblingIndex(originalIndex);
    //        onItemDropped?.Invoke(uniqueId, toId);
    //    }
    //    // 4. ���â ���� �� ���� ó��
    //    else if (inEquipmentSlot)
    //    {
    //        if (originType == ItemOrigin.Inventory)
    //        {
    //            transform.SetParent(originalParent, false);
    //            transform.SetSiblingIndex(originalIndex);

    //            onItemEquipped?.Invoke(uniqueId, originType);
    //            Debug.Log($"�巡�� ���� �� �κ��丮 ���� {originalIndex} ����, ���â ���� '{targetSlot.name}' ����");
    //        }
    //        else if (originType == ItemOrigin.Equipment)
    //        {
    //            // ���â���� �巡�� �� ���� ����
    //            transform.SetParent(originalParent);
    //            transform.SetSiblingIndex(originalIndex);
    //        }
    //    }

    //    //else if (inPotionSlot)
    //    //{
    //    //    if (Item != null && Item.data != null &&
    //    //        string.Equals(Item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
    //    //    {
    //    //        // ������ �ε�
    //    //        Sprite s = null;
    //    //        if (!string.IsNullOrEmpty(Item.iconPath))
    //    //            s = Resources.Load<Sprite>(Item.iconPath);

    //    //        // placeholder�� "����" �θ𿡼� ����(Detach) �ı� ����
    //    //        if (placeholder)
    //    //        {
    //    //            // 1) �θ𿡼� �и�: �κ��丮 �������ð� �� ������Ʈ�� �� �̻� ���� ����
    //    //            placeholder.transform.SetParent(null, false);

    //    //            // 2) �ı� ���� (������ ���� ������� ����)
    //    //            Destroy(placeholder);
    //    //            placeholder = null;
    //    //        }

    //    //        // �巡�� ���̴� ��ư�� ��� ���� (�������ð� �籸���� ��)
    //    //        gameObject.SetActive(false);

    //    //        // ���ٷ� �̰�(���ο��� �κ� ���� + ����)
    //    //        var qb = PotionQuickBar.Instance;
    //    //        if (qb != null)
    //    //        {
    //    //            qb.Assign(potionIndex, Item, s);
    //    //        }

    //    //        inventoryPresenter.Refresh();

    //    //        // ���⼭ �� (�Ʒ� ���� placeholder �ı��� �������� �ʰ�)
    //    //        return;
    //    //    }
    //    //    else
    //    //    {
    //    //        if (placeholder)
    //    //        {
    //    //            placeholder.transform.SetParent(null, false);
    //    //            Destroy(placeholder);
    //    //            placeholder = null;
    //    //        }
    //    //        SnapBackToOriginal();
    //    //        return;
    //    //    }
    //    //}

    //    // === 6) � �������� �ش����� ���� �� UI �� ���: ������ ���� ó�� ===
    //    else
    //    {
    //        // placeholder ����(Detach �� �ı�) - Ȥ�� ��������
    //        if (placeholder)
    //        {
    //            placeholder.transform.SetParent(null, false);
    //            Destroy(placeholder);
    //            placeholder = null;
    //        }

    //        // �巡�� �������� ĵ���� �ؿ� ������ �ʰ� ��� ����
    //        gameObject.SetActive(false);                 // ��ư ����
    //        transform.SetParent(originalParent, false);  // ���� �θ�� ����
    //        transform.SetSiblingIndex(originalIndex);    // �ε��� ����

    //        // �𵨿��� ����(InventoryPresenter �� InventoryModel.RemoveById)
    //        onItemRemoved?.Invoke(uniqueId);

    //        Debug.Log("OnEndDrag - Removed Item (Outside any UI region)");
    //        return; // �б� ����
    //    }

    //    // �������� Ȥ�� �����ִٸ� �� �� �� ���
    //    if (placeholder)
    //    {
    //        placeholder.transform.SetParent(null, false);
    //        Destroy(placeholder);
    //        placeholder = null;
    //    }
    //}
    public void OnEndDrag(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide();

        if (originType == ItemOrigin.Equipment)
            return;

        canvasGroup.blocksRaycasts = true;

        // 0) �� ���� ���� �� üũ: ��� ó���� OnDrop���� �ϹǷ� ���⼱ '����'�� ���� ����
        var potionSlotUnderPointer = eventData.pointerEnter
            ? eventData.pointerEnter.GetComponentInParent<PotionSlotUI>()
            : null;
        if (potionSlotUnderPointer != null)
        {
            // placeholder�� �����ϰ� �ƹ� �͵� ���� �ʴ´�.
            if (placeholder)
            {
                placeholder.transform.SetParent(null, false);
                Destroy(placeholder);
                placeholder = null;
            }
            return; // �� ���� �б�� ���� �ʰ� ���� ����
        }

        // 1) �κ��丮 ���� Ȯ��
        RectTransform inventoryRect = originalParent as RectTransform;
        bool inInventory = RectTransformUtility.RectangleContainsScreenPoint(
            inventoryRect, eventData.position, canvas.worldCamera);

        // 2) ���â ���� Ȯ��
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
        int newIndex = placeholder ? placeholder.transform.GetSiblingIndex() : originalIndex;

        if (inInventory)
        {
            int closestIndex = 0;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < originalParent.childCount; i++)
            {
                float dist = Vector2.SqrMagnitude(eventData.position - (Vector2)originalParent.GetChild(i).position);
                if (dist < closestDistance) { closestDistance = dist; closestIndex = i; }
            }

            string toId = null;
            if (closestIndex < originalParent.childCount)
            {
                var target = originalParent.GetChild(closestIndex).GetComponent<DraggableItemView>();
                if (target != null) toId = target.Item.uniqueId;
            }

            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);
            onItemDropped?.Invoke(uniqueId, toId);
        }
        else if (inEquipmentSlot)
        {
            if (originType == ItemOrigin.Inventory)
            {
                transform.SetParent(originalParent, false);
                transform.SetSiblingIndex(originalIndex);
                onItemEquipped?.Invoke(uniqueId, originType);
            }
            else
            {
                transform.SetParent(originalParent, false);
                transform.SetSiblingIndex(originalIndex);
            }
        }
        else
        {
            // �� ����� ���� 'UI ��'�̹Ƿ� ����. (���� ������ ��� ������ �̹� return ����)
            if (placeholder)
            {
                placeholder.transform.SetParent(null, false);
                Destroy(placeholder);
                placeholder = null;
            }

            gameObject.SetActive(false);
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);
            onItemRemoved?.Invoke(uniqueId);
            return;
        }

        // ������ ����
        if (placeholder)
        {
            placeholder.transform.SetParent(null, false);
            Destroy(placeholder);
            placeholder = null;
        }
    }

}

