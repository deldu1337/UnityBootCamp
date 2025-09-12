using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// �κ��丮 ������ ������
/// </summary>
[Serializable]
public class InventoryItem
{
    public string uniqueId;          // ���� ID (�ߺ� ����)
    public int id;                   // ������ �⺻ ID (DataManager ����)
    public ItemData data;            // ������ ��Ÿ ������
    public string iconPath;          // ������ ���ҽ� ���
    public string prefabPath;        // ������ ���ҽ� ��� (JSON �����)
    public GameObject prefab => Resources.Load<GameObject>(prefabPath); // ��Ÿ�� �ε�
}

/// <summary>
/// �κ��丮 ��ü �����͸� JSON ����ȭ������ ���δ� Ŭ����
/// </summary>
[Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();
}

/// <summary>
/// �κ��丮 �� (������ ����, ����/�ε�, CRUD)
/// </summary>
public class InventoryModel
{
    private Dictionary<string, InventoryItem> itemDict = new Dictionary<string, InventoryItem>(); // uniqueId ���
    private string filePath;

    // �б� ���� ������ ����Ʈ
    public IReadOnlyList<InventoryItem> Items => new List<InventoryItem>(itemDict.Values);

    public InventoryModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
        Load(); // ���� �� JSON���� �ҷ�����
    }

    /// <summary>
    /// ������ �߰� �Ǵ� �����
    /// </summary>
    public void AddItem(InventoryItem item)
    {
        if (!itemDict.ContainsKey(item.uniqueId))
            itemDict[item.uniqueId] = item;
        Save();
    }

    /// <summary>
    /// �ε��� ���� ������ ��ġ ��ü
    /// </summary>
    public void SwapItem(int indexA, int indexB)
    {
        var list = new List<InventoryItem>(itemDict.Values);
        if (indexA == indexB || indexA < 0 || indexB < 0 || indexA >= list.Count || indexB >= list.Count)
            return;

        var temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;

        // Dictionary ����ȭ
        itemDict.Clear();
        foreach (var item in list)
            itemDict[item.uniqueId] = item;

        Save();
    }

    /// <summary>
    /// �ε��� ��� ������ ����
    /// </summary>
    public void RemoveAt(int index)
    {
        var list = new List<InventoryItem>(itemDict.Values);
        if (index < 0 || index >= list.Count) return;

        var item = list[index];
        RemoveItem(item.uniqueId);
    }

    /// <summary>
    /// �ߺ� �����ϸ� ������ �߰�
    /// </summary>
    public bool Add(InventoryItem item)
    {
        if (item == null) return false;
        if (itemDict.ContainsKey(item.uniqueId)) return false;

        itemDict[item.uniqueId] = item;
        Save();
        return true;
    }

    /// <summary>
    /// JSON���� �κ��丮 �ε�
    /// </summary>
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
            Save(); // ���� ������ �ʱ�ȭ �� ����
        }
    }

    /// <summary>
    /// JSON���� �κ��丮 ����
    /// </summary>
    public void Save()
    {
        var data = new InventoryData { items = new List<InventoryItem>(itemDict.Values) };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"�κ��丮 ����� �� {filePath}");
    }

    /// <summary>
    /// uniqueId ��� ����
    /// </summary>
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
