using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EquipmentSlot
{
    public string slotType;          // ���� Ÿ�� (��: "head", "rshoulder", "weapon")
    public InventoryItem equipped;   // ������ ������ (������ null)
}

[Serializable]
public class EquipmentData
{
    public List<EquipmentSlot> slots = new List<EquipmentSlot>(); // ���� ����Ʈ
}

public class EquipmentModel
{
    private string filePath; // JSON ���� ���
    private EquipmentData data; // ��� ������ ����

    public IReadOnlyList<EquipmentSlot> Slots => data.slots; // �ܺ� ���ٿ� ���� �б� ����

    public EquipmentModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerEquipment.json");
        Load();

        // ������ ��������� �⺻ ���� ����
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

    /// <summary>������ ����</summary>
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

    /// <summary>������ ����</summary>
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

    /// <summary>�ε����� ���� ����</summary>
    public void Unequip(int index)
    {
        if (index < 0 || index >= data.slots.Count) return;

        var slot = data.slots[index];
        Debug.Log($"{slot.slotType} ���Կ��� {slot.equipped?.data?.name ?? "����"} ����");
        slot.equipped = null;
        Save();
    }

    /// <summary>���� ��ȸ</summary>
    public EquipmentSlot GetSlot(string slotType)
    {
        return data.slots.Find(s => s.slotType == slotType);
    }

    /// <summary>��� ������ �ҷ�����</summary>
    public void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<EquipmentData>(json);
        }
        else
        {
            data = new EquipmentData();
            Save();
        }
    }

    /// <summary>��� ������ ����</summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"��� ������ ����� �� {filePath}");
    }
}
