using System;
using UnityEngine;

/// <summary>
/// 인벤토리 UI와 모델을 연결하고 입력/상호작용 처리
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
            Debug.LogError("InventoryView를 찾을 수 없습니다!");
            return;
        }

        model = new InventoryModel();
        view.Initialize(CloseInventory); // 닫기 버튼 콜백
        isOpen = false;
    }

    void Update()
    {
        // I 키로 인벤토리 토글
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
    /// 아이템 추가 (ID, 아이콘, 프리팹 경로)
    /// </summary>
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
            prefabPath = prefabPath
        };

        model.AddItem(item);

        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    /// <summary>
    /// 기존 InventoryItem 객체를 인벤토리에 추가 (uniqueId 유지)
    /// </summary>
    public void AddExistingItem(InventoryItem item)
    {
        if (item == null)
            return;

        model.AddItem(item);

        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }


    // 인벤토리 슬롯 간 이동
    private void OnItemDropped(int fromIndex, int toIndex)
    {
        model.SwapItem(fromIndex, toIndex);
        if (isOpen)
            view.UpdateInventoryUI(model.Items, OnItemDropped, OnItemRemoved, OnItemEquipped);
    }

    // 인벤토리 → 장비창
    private void OnItemEquipped(int index)
    {
        if (index < 0 || index >= model.Items.Count) return;
        var item = model.Items[index];
        Debug.Log($"OnItemEquipped - {index}번 슬롯 아이템 {item.data.name} 장착 시도");

        var equipPresenter = FindAnyObjectByType<EquipmentPresenter>();
        equipPresenter?.HandleEquipItem(item, index);
    }

    // 아이템 제거
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
