using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button exitButton;

    private Button[] inventoryButtons;

    public void Initialize(Action onExit)
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(() => onExit?.Invoke());

        inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        Show(false);
    }

    public void Show(bool show) => inventoryPanel?.SetActive(show);

    public void UpdateInventoryUI(
        IReadOnlyList<InventoryItem> items,
        Action<string, string> onItemDropped,
        Action<string> onItemRemoved,
        Action<string> onItemEquipped
    )
    {
        // UpdateInventoryUI ���� �κ�
        foreach (Transform child in buttonContainer)
        {
            if (child && (child.name == "Placeholder"))
                GameObject.Destroy(child.gameObject);
        }

        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        for (int i = 0; i < items.Count && i < inventoryButtons.Length; i++)
        {
            var item = items[i];
            if (InventoryGuards.IsInvalid(item))
                continue; // UI ���� ���� ����

            var button = inventoryButtons[i];
            button.gameObject.SetActive(true);

            var image = button.GetComponent<Image>();
            if (image != null && !string.IsNullOrEmpty(item.iconPath))
            {
                var icon = Resources.Load<Sprite>(item.iconPath);
                if (icon != null) image.sprite = icon;
            }

            // �� ���ϴ� ���� ��(�ʿ� �� ���� �� ����)
            var qty = EnsureQtyLabel(button.transform);
            if (item.data != null && item.data.type == "potion" && item.quantity >= 1)
            {
                qty.text = item.quantity.ToString();
                qty.enabled = true;
            }
            else
            {
                qty.text = "";
                qty.enabled = false;
            }

            var draggable = button.GetComponent<DraggableItemView>();
            if (draggable == null)
                draggable = button.gameObject.AddComponent<DraggableItemView>();

            Action<string, ItemOrigin> wrappedEquip = null;
            if (onItemEquipped != null)
                wrappedEquip = (uid, origin) => onItemEquipped.Invoke(uid);

            draggable.Initialize(item, ItemOrigin.Inventory, onItemDropped, onItemRemoved, wrappedEquip, null);

            var hover = button.GetComponent<ItemHoverTooltip>();
            if (hover == null) hover = button.gameObject.AddComponent<ItemHoverTooltip>();
            hover.SetItem(item);
            hover.SetContext(ItemOrigin.Inventory);   // �� �߰�: �κ��丮 ���ؽ�Ʈ

            var CanvasGroup = button.GetComponent<CanvasGroup>();
            CanvasGroup.blocksRaycasts = true;
        }
    }

    // ���� ���� using ����:
    // using UnityEngine.UI;

    private Text EnsureQtyLabel(Transform parent)
    {
        var t = parent.Find("Qty") as RectTransform;
        if (t == null)
        {
            var go = new GameObject("Qty", typeof(RectTransform));
            t = go.GetComponent<RectTransform>();
            t.SetParent(parent, false);
            t.anchorMin = new Vector2(1, 0);
            t.anchorMax = new Vector2(1, 0);
            t.pivot = new Vector2(1, 0);
            t.anchoredPosition = new Vector2(-4, 4);
            t.sizeDelta = new Vector2(60, 24);

            var txt = go.AddComponent<Text>();
            txt.alignment = TextAnchor.LowerRight;

            // �� ����� �κ�: ���� ��Ʈ �̸� ��ü + ����
            Font f = null;
            try
            {
                // �� ���� ��Ʈ �̸�
                f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch { /* �Ϻ� �������� ���� ���� */ }

            // ���� ������ �� ã���� OS ��Ʈ ���� �õ�
            if (f == null)
            {
                try { f = Font.CreateDynamicFontFromOSFont("Arial", 18); } catch { }
            }

            txt.font = f;            // f�� null�̾ Text�� ���������� �����ϸ� ���õ�
            txt.fontSize = 16;
            txt.raycastTarget = false;

            var outline = go.AddComponent<Outline>();
            outline.effectDistance = new Vector2(1, -1);
            outline.useGraphicAlpha = true;
        }
        return t.GetComponent<Text>();
    }

}

