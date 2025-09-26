using System;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour
{
    private InventoryModel model;
    private InventoryView view;
    private bool isOpen;

    void Start()
    {
        view = FindAnyObjectByType<InventoryView>();
        if (view == null) return;

        model = new InventoryModel();
        view.Initialize(CloseInventory);
        isOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
        if(Input.GetKeyDown(KeyCode.Escape))
            CloseInventory();
    }

    private void ToggleInventory()
    {
        if (view == null) return;
        isOpen = !isOpen;
        view.Show(isOpen);
        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    private void CloseInventory()
    {
        isOpen = false;
        view.Show(false);
    }

    public void AddItem(int id, Sprite icon, string prefabPath)
    {
        var dataManager = DataManager.Instance;
        if (!dataManager.dicItemDatas.ContainsKey(id))
        {
            Debug.LogWarning($"������ ID {id}�� DataManager�� ����!");
            return;
        }

        var item = new InventoryItem
        {
            uniqueId = Guid.NewGuid().ToString(),
            id = id,
            data = dataManager.dicItemDatas[id],
            iconPath = "Icons/" + icon.name,
            prefabPath = prefabPath
        };

        // ���� ���
        if (InventoryGuards.IsInvalid(item))
        {
            Debug.LogWarning("[InventoryPresenter] ������ �������� ��ȿ �� �߰� ���");
            return;
        }

        model.AddItem(item);
        Refresh();
    }

    // ��ȯ������ '���� ���� ����' ���θ� �� �� �ְ� ����
    public bool RemoveItemFromInventory(string uniqueId)
    {
        if (model == null) model = new InventoryModel(); // ������

        var before = model.GetItemById(uniqueId) != null;
        model.RemoveById(uniqueId);
        var after = model.GetItemById(uniqueId) != null;

        // �κ� UI�� ���� �־ ������ �ѹ� ����(���̴� ���¶�� ��� �ݿ�)
        ForceRefresh();

        return before && !after;
    }

    public void ForceRefresh() => view?.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);

    private void OnItemEquipped(string uniqueId)
    {
        var item = model.GetItemById(uniqueId);
        if (item == null) return;

        if (item.data != null && string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
        {
            var stats = PlayerStatsManager.Instance; // �̱���
            if (stats != null)
            {
                if (item.data.hp > 0) stats.Heal(item.data.hp);
                if (item.data.mp > 0) stats.RestoreMana(item.data.mp);
            }
            // �κ��丮���� �ش� ���� 1�� ����
            model.RemoveById(uniqueId);

            // UI ����
            Refresh();
            return; // ��� ���� �������� ���� �ʵ��� ����
        }

        var equipPresenter = FindAnyObjectByType<EquipmentPresenter>();
        equipPresenter?.HandleEquipItem(item);

        Refresh();
    }


    private void OnItemRemoved(string uniqueId)
    {
        model.RemoveById(uniqueId);
        Refresh();
    }

    private void OnItemDropped(string fromId, string toId)
    {
        model.ReorderByUniqueId(fromId, toId);
        Refresh();
    }

    public void AddExistingItem(InventoryItem item)
    {
        model.Add(item);
        Refresh();
    }

    public InventoryItem GetItemByUniqueId(string uniqueId)
    {
        // model.GetItemById�� �̹� ������, �״�� �����ص� �˴ϴ�.
        return model.GetItemById(uniqueId);
    }

    //public void RemoveItemFromInventory(string uniqueId) => model.RemoveById(uniqueId);

    public void Refresh()
    {
        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }
}
