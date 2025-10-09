using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentSlot
{
    public string slotType;
    public InventoryItem equipped;
}

[Serializable]
public class EquipmentData
{
    public List<EquipmentSlot> slots = new List<EquipmentSlot>();
}

public class EquipmentModel
{
    private readonly string race;   // �� �߰�: ����
    private EquipmentData data;

    public IReadOnlyList<EquipmentSlot> Slots => data.slots;

    public EquipmentModel(string race)
    {
        this.race = string.IsNullOrEmpty(race) ? "humanmale" : race;

        Load();

        if (data.slots.Count == 0)
        {
            data.slots.Add(new EquipmentSlot { slotType = "head", equipped = null });
            data.slots.Add(new EquipmentSlot { slotType = "rshoulder", equipped = null });
            data.slots.Add(new EquipmentSlot { slotType = "lshoulder", equipped = null });
            data.slots.Add(new EquipmentSlot { slotType = "gem", equipped = null });
            data.slots.Add(new EquipmentSlot { slotType = "weapon", equipped = null });
            data.slots.Add(new EquipmentSlot { slotType = "shield", equipped = null });
            Save();
        }
    }

    public void EquipItem(string slotType, InventoryItem item)
    {
        var slot = data.slots.Find(s => s.slotType == slotType);
        if (slot != null)
        {
            slot.equipped = item;
            Save();
            Debug.Log($"{slotType} ���Կ� {item.data.name} ������");
        }
        else
        {
            Debug.LogWarning($"EquipItem ����: {slotType} ������ ã�� �� ����");
        }
    }

    public void UnequipItem(string slotType)
    {
        var slot = data.slots.Find(s => s.slotType == slotType);
        if (slot != null)
        {
            Debug.Log($"{slotType} ���Կ��� {slot.equipped?.data?.name ?? "����"} ����");
            slot.equipped = null;
            Save();
        }
        else
        {
            Debug.LogWarning($"UnequipItem ����: {slotType} ������ ã�� �� ����");
        }
    }

    public void Unequip(int index)
    {
        if (index < 0 || index >= data.slots.Count) return;
        var slot = data.slots[index];
        Debug.Log($"{slot.slotType} ���Կ��� {slot.equipped?.data?.name ?? "����"} ����");
        slot.equipped = null;
        Save();
    }

    public EquipmentSlot GetSlot(string slotType) => data.slots.Find(s => s.slotType == slotType);

    public void Load()
    {
        data = SaveLoadService.LoadEquipmentForRaceOrNew(race);
    }

    public void Save()
    {
        SaveLoadService.SaveEquipmentForRace(race, data);
    }
}
