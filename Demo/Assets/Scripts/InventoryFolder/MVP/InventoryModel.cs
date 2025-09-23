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
    private List<InventoryItem> items = new();  // ���� ����
    private string filePath;

    public IReadOnlyList<InventoryItem> Items => items;

    public InventoryModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
        Load();
        SaveIfCleaned();      // �� Ŭ������ ���� ��� �ݿ�
    }

    public InventoryItem GetItemById(string uniqueId)
    {
        return items.Find(i => i.uniqueId == uniqueId);
    }

    public void AddItem(InventoryItem item)
    {
        if (InventoryGuards.IsInvalid(item))
        {
            Debug.LogWarning("[InventoryModel] ��ȿ ������ �߰� �õ� �� ����");
            return;
        }
        if (items.Exists(i => i.uniqueId == item.uniqueId))
        {
            Debug.LogWarning($"[InventoryModel] �ߺ� uniqueId �߰� �õ�({item.uniqueId}) �� ����");
            return;
        }
        items.Add(item);
        Save();
    }

    public void RemoveById(string uniqueId)
    {
        items.RemoveAll(i => i.uniqueId == uniqueId);
        Save();
    }

    /// <summary>
    /// uniqueId ������� ������ ������ ���ġ�ϰ� JSON�� ����
    /// </summary>
    public void ReorderByUniqueId(string fromId, string toId)
    {
        int fromIndex = items.FindIndex(i => i.uniqueId == fromId);
        if (fromIndex < 0) return;

        int toIndex = string.IsNullOrEmpty(toId) ? items.Count : items.FindIndex(i => i.uniqueId == toId);
        if (toIndex < 0) toIndex = items.Count - 1;

        var item = items[fromIndex];
        if (InventoryGuards.IsInvalid(item))
        {
            Debug.LogWarning("[InventoryModel] ���ġ �� ��ȿ ������ �߰� �� ����");
            items.RemoveAt(fromIndex);
            Save();
            return;
        }
        items.RemoveAt(fromIndex);

        // fromIndex < toIndex������ toIndex�� -1 ����� �������� ��ġ
        if (toIndex > fromIndex)
            toIndex--;

        items.Insert(toIndex, item);
        Save();
    }

    public bool Add(InventoryItem item)
    {
        if (InventoryGuards.IsInvalid(item)) return false;
        if (item == null || items.Exists(i => i.uniqueId == item.uniqueId)) return false;
        items.Add(item);
        Save();
        return true;
    }

    public void Load()
    {
        // ���� ���� ���
        var data = SaveLoadService.LoadInventoryOrNew();
        items = data.items ?? new List<InventoryItem>();

        // �ε�� ��ȿ ������ ����
        int before = items.Count;
        items.RemoveAll(InventoryGuards.IsInvalid);
        int after = items.Count;
        if (before != after)
            Debug.LogWarning($"[InventoryModel] �ε�� ��ȿ ������ {before - after}�� ������");
    }

    public void Save()
    {
        // ���� ������ ��ȿ ������ ����(���� ������)
        items.RemoveAll(InventoryGuards.IsInvalid);

        var data = new InventoryData { items = items };
        SaveLoadService.SaveInventory(data);
    }

    // �����ڿ��� ȣ���ϴ� �������Ǿ����� �ѹ� �� ���塱 ����
    private void SaveIfCleaned()
    {
        // ���ϰ� �޸��� ������ ���� �ٸ��� ������ ������ �Ǵ�
        if (!File.Exists(filePath)) { Save(); return; }

        try
        {
            string json = File.ReadAllText(filePath);
            var onDisk = JsonUtility.FromJson<InventoryData>(json)?.items ?? new List<InventoryItem>();
            int diskCount = onDisk.Count;
            int memCount = items.Count;
            if (diskCount != memCount)
            {
                Debug.LogWarning($"[InventoryModel] �ʱ� �ε� �� ���� �ݿ�: {diskCount} �� {memCount}");
                Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[InventoryModel] SaveIfCleaned �� ���� �� ���� �� Save ����: {e}");
            Save();
        }
    }
}
