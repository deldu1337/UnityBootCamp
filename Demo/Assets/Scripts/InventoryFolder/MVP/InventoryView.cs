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

    public void Show(bool show)
    {
        inventoryPanel?.SetActive(show);
    }

    public void UpdateInventoryUI(IReadOnlyList<InventoryItem> items, Action<int, int> onItemDropped, Action<int> onItemRemoved)
    {
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

            // 드래그 연결
            var draggable = button.GetComponent<DraggableItemView>();
            if (draggable == null)
                draggable = button.gameObject.AddComponent<DraggableItemView>();

            draggable.Initialize(onItemDropped, i, onItemRemoved);
        }
    }
}
