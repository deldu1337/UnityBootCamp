using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EquipmentSlot
{
    public string slotType;          // 슬롯 타입 (예: "head", "rshoulder", "weapon")
    public InventoryItem equipped;   // 장착된 아이템 (없으면 null)
}

[Serializable]
public class EquipmentData
{
    public List<EquipmentSlot> slots = new List<EquipmentSlot>(); // 슬롯 리스트
}

public class EquipmentModel
{
    private string filePath; // JSON 저장 경로
    private EquipmentData data; // 장비 데이터 저장

    public IReadOnlyList<EquipmentSlot> Slots => data.slots; // 외부 접근용 슬롯 읽기 전용

    public EquipmentModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerEquipment.json");
        Load();

        // 슬롯이 비어있으면 기본 슬롯 구성
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

    /// <summary>아이템 장착</summary>
    public void EquipItem(string slotType, InventoryItem item)
    {
        var slot = data.slots.Find(s => s.slotType == slotType);
        if (slot != null)
        {
            slot.equipped = item;
            Save();
            Debug.Log($"{slotType} 슬롯에 {item.data.name} 장착됨");
        }
        else
        {
            Debug.LogWarning($"EquipItem 실패: {slotType} 슬롯을 찾을 수 없음");
        }
    }

    /// <summary>아이템 해제</summary>
    public void UnequipItem(string slotType)
    {
        var slot = data.slots.Find(s => s.slotType == slotType);
        if (slot != null)
        {
            Debug.Log($"{slotType} 슬롯에서 {slot.equipped?.data?.name ?? "없음"} 해제");
            slot.equipped = null;
            Save();
        }
        else
        {
            Debug.LogWarning($"UnequipItem 실패: {slotType} 슬롯을 찾을 수 없음");
        }
    }

    /// <summary>인덱스로 슬롯 해제</summary>
    public void Unequip(int index)
    {
        if (index < 0 || index >= data.slots.Count) return;

        var slot = data.slots[index];
        Debug.Log($"{slot.slotType} 슬롯에서 {slot.equipped?.data?.name ?? "없음"} 해제");
        slot.equipped = null;
        Save();
    }

    /// <summary>슬롯 조회</summary>
    public EquipmentSlot GetSlot(string slotType)
    {
        return data.slots.Find(s => s.slotType == slotType);
    }

    /// <summary>장비 데이터 불러오기</summary>
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

    /// <summary>장비 데이터 저장</summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"장비 데이터 저장됨 → {filePath}");
    }
}
