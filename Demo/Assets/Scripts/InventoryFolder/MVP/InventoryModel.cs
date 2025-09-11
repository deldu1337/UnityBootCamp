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
    public string prefabPath;      // JSON에서 불러오는 필드
    public GameObject prefab => Resources.Load<GameObject>(prefabPath); // 런타임에 로드
}

[Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();
}

public class InventoryModel
{
    private Dictionary<string, InventoryItem> itemDict = new Dictionary<string, InventoryItem>();
    private string filePath;

    public IReadOnlyList<InventoryItem> Items => new List<InventoryItem>(itemDict.Values);

    public InventoryModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
        Load();
    }

    public void AddItem(InventoryItem item)
    {
        itemDict[item.uniqueId] = item;
        Save();
    }

    public void SwapItems(int indexA, int indexB)
    {
        var list = new List<InventoryItem>(itemDict.Values);
        if (indexA == indexB || indexA < 0 || indexB < 0 ||
            indexA >= list.Count || indexB >= list.Count)
            return;

        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;

        // Dictionary 동기화
        itemDict.Clear();
        foreach (var item in list)
            itemDict[item.uniqueId] = item;

        Save();
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<InventoryData>(json);
            itemDict.Clear();
            foreach (var item in data.items)
                itemDict[item.uniqueId] = item;
        }
        else
        {
            itemDict.Clear();
            Save();
        }
    }

    public void Save()
    {
        var data = new InventoryData { items = new List<InventoryItem>(itemDict.Values) };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"인벤토리 저장됨 → {filePath}");
    }

    public void RemoveItem(string uniqueId)
    {
        if (itemDict.ContainsKey(uniqueId))
        {
            Debug.Log("RemoveItem");
            itemDict.Remove(uniqueId);
            Save();
        }
    }

}
