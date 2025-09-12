using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �κ��丮 UI ���� �� ��ư/�巡�� ����
/// </summary>
public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;   // �κ��丮 ��ü �г�
    [SerializeField] private Transform buttonContainer;   // ���� ��ư �����̳�
    [SerializeField] private Button exitButton;           // �ݱ� ��ư

    private Button[] inventoryButtons;

    /// <summary>
    /// �ʱ�ȭ - �ݱ� ��ư ���
    /// </summary>
    public void Initialize(Action onExit)
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(() => onExit?.Invoke());

        inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        Show(false);
    }

    /// <summary>
    /// UI ǥ��/����
    /// </summary>
    public void Show(bool show)
    {
        inventoryPanel?.SetActive(show);
    }

    /// <summary>
    /// ������ ����Ʈ ��� UI ����
    /// </summary>
    public void UpdateInventoryUI(IReadOnlyList<InventoryItem> items, Action<int, int> onItemDropped,
        Action<int> onItemRemoved, Action<int> equipCallback)
    {
        // ��� ��ư ����
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        for (int i = 0; i < items.Count && i < inventoryButtons.Length; i++)
        {
            var item = items[i];
            var button = inventoryButtons[i];

            button.gameObject.SetActive(true);

            // ������ ����
            var image = button.GetComponent<Image>();
            if (image != null && !string.IsNullOrEmpty(item.iconPath))
            {
                var icon = Resources.Load<Sprite>(item.iconPath);
                if (icon != null)
                    image.sprite = icon;
            }

            // �巡�� ó�� ����
            var draggable = button.GetComponent<DraggableItemView>();
            if (draggable == null)
                draggable = button.gameObject.AddComponent<DraggableItemView>();

            // equipCallback (Action<int>) �� Draggable�� ����ϴ� Action<int, ItemOrigin> ���� ����
            Action<int, ItemOrigin> wrappedEquip = null;
            if (equipCallback != null)
                wrappedEquip = (idx, origin) => equipCallback.Invoke(idx);

            draggable.Initialize(
                item,                       // InventoryItem
                ItemOrigin.Inventory,       // ���� ��ġ
                i,                          // ���� �ε���
                onItemDropped,              // ��� �ݹ�
                onItemRemoved,              // ���� �ݹ�
                wrappedEquip,               // ���� �ݹ�
                null                        // ���� �ݹ� (�κ��丮������ �ʿ����)
            );
        }
    }
}
