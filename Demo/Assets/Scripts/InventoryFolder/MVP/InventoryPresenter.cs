using System;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour
{
    private InventoryModel model;
    private InventoryView view;
    private bool isOpen;

    void Start()
    {
        view = FindObjectOfType<InventoryView>();
        if (view == null)
        {
            Debug.LogError("InventoryView를 찾을 수 없습니다!");
            return;
        }

        model = new InventoryModel();
        view.Initialize(CloseInventory);
        isOpen = false;
    }

    void Update()
    {
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

    public void AddItem(int id, Sprite icon, string prefabPath)
    {
        var dataManager = DataManager.Instance;
        if (!dataManager.dicItemDatas.ContainsKey(id))
        {
            Debug.LogWarning($"아이템 ID {id}가 DataManager에 없음!");
            return;
        }

        var item = new InventoryItem
        {
            uniqueId = Guid.NewGuid().ToString(),
            id = id,
            data = dataManager.dicItemDatas[id],
            iconPath = "Icons/" + icon.name,
            prefabPath = prefabPath // Resources 경로 문자열
        };

        model.AddItem(item);

        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    private void OnItemDropped(int fromIndex, int toIndex)
    {
        model.SwapItems(fromIndex, toIndex);
        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    private void OnItemEquipped(int index)
    {
        if (index < 0 || index >= model.Items.Count) return;
        var item = model.Items[index];
        Debug.Log($"OnItemEquipped - {index}번 슬롯 아이템 {item.data.name} 장착 시도");

        // EquipmentPresenter 연결 → 실제 장착 처리
        var equipPresenter = FindObjectOfType<EquipmentPresenter>();
        equipPresenter?.HandleEquipItem(item, index);
    }

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
