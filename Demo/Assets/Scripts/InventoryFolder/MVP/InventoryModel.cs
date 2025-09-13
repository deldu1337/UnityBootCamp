using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string uniqueId;
    public int id;
    public ItemData data;
    public string iconPath;
    public string prefabPath;
    public GameObject prefab => Resources.Load<GameObject>(prefabPath);
}

[Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();
}

public class InventoryModel
{
    private List<InventoryItem> items = new();  // 순서 보장
    private string filePath;

    public IReadOnlyList<InventoryItem> Items => items;

    public InventoryModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
        Load();
    }

    public InventoryItem GetItemById(string uniqueId)
    {
        return items.Find(i => i.uniqueId == uniqueId);
    }

    public void AddItem(InventoryItem item)
    {
        if (!items.Exists(i => i.uniqueId == item.uniqueId))
        {
            items.Add(item);
            Save();
        }
    }

    public void RemoveById(string uniqueId)
    {
        items.RemoveAll(i => i.uniqueId == uniqueId);
        Save();
    }

    /// <summary>
    /// uniqueId 기반으로 아이템 순서를 재배치하고 JSON에 저장
    /// </summary>
    public void ReorderByUniqueId(string fromId, string toId)
    {
        int fromIndex = items.FindIndex(i => i.uniqueId == fromId);
        if (fromIndex < 0) return;

        int toIndex = string.IsNullOrEmpty(toId) ? items.Count : items.FindIndex(i => i.uniqueId == toId);
        if (toIndex < 0) toIndex = items.Count - 1;

        var item = items[fromIndex];
        items.RemoveAt(fromIndex);

        // fromIndex < toIndex였으면 toIndex를 -1 해줘야 정상적인 위치
        if (toIndex > fromIndex)
            toIndex--;

        items.Insert(toIndex, item);
        Save();
    }

    public bool Add(InventoryItem item)
    {
        if (item == null || items.Exists(i => i.uniqueId == item.uniqueId)) return false;
        items.Add(item);
        Save();
        return true;
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<InventoryData>(json);
            items = data?.items ?? new List<InventoryItem>();
        }
        else
        {
            items = new List<InventoryItem>();
            Save();
        }
    }

    public void Save()
    {
        var data = new InventoryData { items = items };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"[InventoryModel] JSON 저장됨: {filePath}");
    }
}
