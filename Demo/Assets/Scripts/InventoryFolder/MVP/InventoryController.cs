using System.Linq;
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

    /// <summary>
    /// ���� �� UI �ʱ�ȭ
    /// </summary>
    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// �κ��丮/���â UI ����
    /// </summary>
    private void RefreshUI()
    {
        // �κ��丮 UI ����
        inventoryView.UpdateInventoryUI(
            inventory.Items,           // ������ ����Ʈ
            OnItemDropped,             // �巡�׾ص�� �̺�Ʈ
            OnItemRemoved,             // �κ��丮 �ܺ� ���� �̺�Ʈ
            (idx) => OnEquipRequest(idx, ItemOrigin.Inventory) // ���� ��û �̺�Ʈ
        );

        // ���â UI ����
        equipmentView.UpdateEquipmentUI(
            equipment.Slots,                   // ��� ���� ����Ʈ
            slotType => OnUnequipRequestBySlot(slotType) // ���� Ÿ�� ��� ��� ���� �̺�Ʈ
        );
    }

    /// <summary>
    /// �κ��丮 ������ ��ġ ���� ó�� (�巡�׾ص��)
    /// </summary>
    /// <param name="from">�̵� �� �ε���</param>
    /// <param name="to">�̵� �� �ε���</param>
    private void OnItemDropped(int from, int to)
    {
        inventory.SwapItem(from, to); // �𵨿��� ������ ��ġ ��ü
        RefreshUI();                  // UI ����
    }

    /// <summary>
    /// �κ��丮 ������ ���� ó�� (�κ��丮 �ܺη� �巡��)
    /// </summary>
    /// <param name="index">������ ������ �ε���</param>
    private void OnItemRemoved(int index)
    {
        inventory.RemoveAt(index); // �𵨿��� ������ ����
        RefreshUI();               // UI ����
    }

    /// <summary>
    /// ���â���� ���� Ÿ�� ��� ��� ���� ó��
    /// </summary>
    /// <param name="slotType">������ ��� ���� Ÿ��</param>
    private void OnUnequipRequestBySlot(string slotType)
    {
        // ���� Ÿ�Կ� �ش��ϴ� �ε��� ã��
        int index = equipment.Slots.ToList().FindIndex(s => s.slotType == slotType);
        if (index < 0) return;

        var item = equipment.Slots[index].equipped;
        if (item == null) return;

        // �κ��丮�� ������ ������ ��� ���� �� �κ��丮�� �߰�
        if (inventory.Add(item))
        {
            equipment.Unequip(index); // �𵨿��� ��� ����
            Debug.Log($"����: {item.data.name}");
        }
        RefreshUI(); // UI ����
    }

    /// <summary>
    /// �κ��丮���� ���â���� ���� ��û ó��
    /// </summary>
    /// <param name="index">�κ��丮 ������ �ε���</param>
    /// <param name="origin">������ ��ó (Inventory/Equipment)</param>
    private void OnEquipRequest(int index, ItemOrigin origin)
    {
        if (origin != ItemOrigin.Inventory) return;

        var item = inventory.Items[index];
        if (item == null) return;

        var slotType = item.data.type;

        // ��� �𵨿� �����ϰ� �κ��丮���� ����
        equipment.EquipItem(slotType, item);
        inventory.RemoveAt(index);
        Debug.Log($"����: {item.data.name}");

        RefreshUI(); // UI ����
    }

    /// <summary>
    /// ���â���� �κ��丮�� ��� ���� ��û ó��
    /// </summary>
    /// <param name="index">��� ���� �ε���</param>
    /// <param name="origin">������ ��ó (Inventory/Equipment)</param>
    private void OnUnequipRequest(int index, ItemOrigin origin)
    {
        if (origin != ItemOrigin.Equipment) return;

        var item = equipment.Slots[index].equipped;
        if (item == null) return;

        // �κ��丮�� ������ ������ ��� ���� �� �κ��丮�� �߰�
        if (inventory.Add(item))
        {
            equipment.Unequip(index); // �𵨿��� ��� ����
            Debug.Log($"����: {item.data.name}");
        }
        RefreshUI(); // UI ����
    }
}
