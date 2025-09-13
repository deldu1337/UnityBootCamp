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
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        for (int i = 0; i < items.Count && i < inventoryButtons.Length; i++)
        {
            var item = items[i];
            var button = inventoryButtons[i];
            button.gameObject.SetActive(true);

            var image = button.GetComponent<Image>();
            if (image != null && !string.IsNullOrEmpty(item.iconPath))
            {
                var icon = Resources.Load<Sprite>(item.iconPath);
                if (icon != null) image.sprite = icon;
            }

            var draggable = button.GetComponent<DraggableItemView>();
            if (draggable == null)
                draggable = button.gameObject.AddComponent<DraggableItemView>();

            Action<string, ItemOrigin> wrappedEquip = null;
            if (onItemEquipped != null)
                wrappedEquip = (uid, origin) => onItemEquipped.Invoke(uid);

            draggable.Initialize(item, ItemOrigin.Inventory, onItemDropped, onItemRemoved, wrappedEquip, null);
        }
    }
}

