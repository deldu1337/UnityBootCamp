using System;
using UnityEngine;

/// <summary>
/// �κ��丮 UI�� ���� �����ϰ� �Է�/��ȣ�ۿ� ó��
/// </summary>
public class InventoryPresenter : MonoBehaviour
{
    private InventoryModel model;
    private InventoryView view;
    private bool isOpen;

    void Start()
    {
        view = FindAnyObjectByType<InventoryView>();
        if (view == null)
        {
            Debug.LogError("InventoryView�� ã�� �� �����ϴ�!");
            return;
        }

        model = new InventoryModel();
        view.Initialize(CloseInventory); // �ݱ� ��ư �ݹ�
        isOpen = false;
    }

    void Update()
    {
        // I Ű�� �κ��丮 ���
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
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
        if (view == null) return;

        isOpen = false;
        view.Show(false);
    }

    /// <summary>
    /// ������ �߰� (ID, ������, ������ ���)
    /// </summary>
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

        model.AddItem(item);

        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    /// <summary>
    /// ���� InventoryItem ��ü�� �κ��丮�� �߰� (uniqueId ����)
    /// </summary>
    public void AddExistingItem(InventoryItem item)
    {
        if (item == null)
            return;

        model.AddItem(item);

        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }


    // �κ��丮 ���� �� �̵�
    private void OnItemDropped(int fromIndex, int toIndex)
    {
        model.SwapItem(fromIndex, toIndex);
        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    // �κ��丮 �� ���â
    private void OnItemEquipped(int index)
    {
        if (index < 0 || index >= model.Items.Count) return;
        var item = model.Items[index];
        Debug.Log($"OnItemEquipped - {index}�� ���� ������ {item.data.name} ���� �õ�");

        var equipPresenter = FindAnyObjectByType<EquipmentPresenter>();
        equipPresenter?.HandleEquipItem(item, index);
    }

    // ������ ����
    private void OnItemRemoved(int index)
    {
        if (index < 0 || index >= model.Items.Count) return;
        var item = model.Items[index];
        model.RemoveItem(item.uniqueId);
        Debug.Log("OnItemRemoved");

        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }
}
