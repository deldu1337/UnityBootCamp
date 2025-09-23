using UnityEngine;

/// <summary>
/// �κ��丮�� ���â�� �����ϰ�, ������ �̵�/����/���� ���� UI�� �� ������Ʈ�� �����ϴ� ��Ʈ�ѷ�
/// </summary>
public class InventoryController : MonoBehaviour
{
    // ��
    public InventoryModel inventory;    // �÷��̾� �κ��丮 ������
    public EquipmentModel equipment;    // ��� ���� ������

    // ��
    public InventoryView inventoryView; // �κ��丮 UI
    public EquipmentView equipmentView; // ���â UI

    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// �κ��丮�� ���â UI ��ü ����
    /// </summary>
    private void RefreshUI()
    {
        // �κ��丮 UI
        inventoryView.UpdateInventoryUI(
            inventory.Items,
            OnItemDropped,   // �巡�� �� ��ġ ����
            OnItemRemoved,   // �ܺη� �巡�� �� ����
            OnEquipRequest   // ������ ����
        );

        // ���â UI
        equipmentView.UpdateEquipmentUI(
            equipment.Slots,
            OnUnequipRequest // ���� Ŭ�� �� ������ ����
        );
    }

    /// <summary>
    /// �κ��丮 ������ ��ġ ���� (�巡�� �� ���)
    /// </summary>
    private void OnItemDropped(string fromId, string toId)
    {
        inventory.ReorderByUniqueId(fromId, toId);
        RefreshUI();
    }

    /// <summary>
    /// �κ��丮 ������ ����
    /// </summary>
    private void OnItemRemoved(string uniqueId)
    {
        inventory.RemoveById(uniqueId);
        RefreshUI();
    }

    /// <summary>
    /// �κ��丮���� ���â���� ������ ����
    /// </summary>
    private void OnEquipRequest(string uniqueId)
    {
        var item = inventory.GetItemById(uniqueId);
        if (item == null) return;

        var slotType = item.data.type;

        // �ش� ���Կ� ������ ����
        equipment.EquipItem(slotType, item);
        inventory.RemoveById(uniqueId);
        
        Debug.Log($"����: {item.data.name}");
        RefreshUI();
    }

    /// <summary>
    /// ���â���� ������ ���� �� �κ��丮�� �̵�
    /// </summary>
    private void OnUnequipRequest(string slotType)
    {
        var slot = equipment.GetSlot(slotType);
        if (slot == null || slot.equipped == null) return;

        var item = slot.equipped;

        if (inventory.Add(item))
        {
            equipment.UnequipItem(slotType);
            Debug.Log($"����: {item.data.name}");
        }

        RefreshUI();
    }
}

