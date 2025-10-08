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

    // ȹ�� �� Ȯ����(�Ѹ�) �ɷ�ġ (������ null ���)
    public RolledItemStats rolled;

    // �� �߰�: ����
    public bool stackable;       // ������ true
    public int quantity = 1;     // ���� ����
    public int maxStack = 99;    // �ִ� ����(���ϸ� ����)

    public GameObject prefab => Resources.Load<GameObject>(prefabPath);
}

[Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();
}

public class InventoryModel
{
    private readonly string race;
    private List<InventoryItem> items = new();  // ���� ����
    private string filePath;

    public IReadOnlyList<InventoryItem> Items => items;

    // �� ������ ������
    public InventoryModel(string race = "humanmale")
    {
        this.race = string.IsNullOrEmpty(race) ? "humanmale" : race;
        filePath = Path.Combine(Application.persistentDataPath, $"playerInventory_{this.race}.json");

        Load();
        SaveIfCleaned(); // �ε�� ������ ������ ������ �ݿ�
    }

    public InventoryItem GetItemById(string uniqueId)
        => items.Find(i => i.uniqueId == uniqueId);

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

        EnsureRolledShapeIfPresent(item);

        items.Add(item);
        Save();
    }

    public void RemoveById(string uniqueId)
    {
        items.RemoveAll(i => i.uniqueId == uniqueId);
        Save();
    }

    /// <summary>uniqueId ������� ������ ������ ���ġ�ϰ� JSON�� ����</summary>
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
        if (toIndex > fromIndex) toIndex--;
        items.Insert(toIndex, item);
        Save();
    }

    public bool Add(InventoryItem item)
    {
        if (InventoryGuards.IsInvalid(item)) return false;
        if (item == null || items.Exists(i => i.uniqueId == item.uniqueId)) return false;

        EnsureRolledShapeIfPresent(item);

        items.Add(item);
        Save();
        return true;
    }

    /// <summary>���� ����(id) �߰� �� ���� ��ġ��. �߰��ϸ� true.</summary>
    public bool TryStackPotion(int itemId, int addAmount)
    {
        var found = items.Find(i => i.data != null
                                 && i.data.type == "potion"
                                 && i.id == itemId
                                 && i.stackable);
        if (found == null) return false;

        int before = found.quantity;
        found.quantity = Mathf.Clamp(found.quantity + addAmount, 0, found.maxStack);
        Debug.Log($"[InventoryModel] ���� ����: id={itemId}, {before} �� {found.quantity}");
        Save();
        return true;
    }

    /// <summary>uniqueId�� ���� 1��(or n��) ���. 0 �Ǹ� ���� ����.</summary>
    public void ConsumePotionByUniqueId(string uniqueId, int count = 1)
    {
        var it = items.Find(i => i.uniqueId == uniqueId);
        if (it == null) return;

        if (it.data != null && it.data.type == "potion" && it.stackable)
        {
            it.quantity -= count;
            if (it.quantity <= 0)
            {
                items.Remove(it);
            }
            Save();
        }
        else
        {
            // �������� �ƴϸ� ����ó�� ����
            RemoveById(uniqueId);
        }
    }

    public void Load()
    {
        // ������ ���� �켱 + ���Ž� ���̱׷��̼�
        var data = SaveLoadService.LoadInventoryForRaceOrNew(race);
        items = data.items ?? new List<InventoryItem>();

        // �ε�� ��ȿ ������ ����
        int before = items.Count;
        items.RemoveAll(InventoryGuards.IsInvalid);
        int after = items.Count;
        if (before != after)
            Debug.LogWarning($"[InventoryModel] �ε�� ��ȿ ������ {before - after}�� ������");

        // rolled ����ü ���ռ� ����(������ null ���)
        for (int i = 0; i < items.Count; i++)
            EnsureRolledShapeIfPresent(items[i]);
    }

    public void Save()
    {
        // ���� ������ ��ȿ ������ ����(���� ������)
        items.RemoveAll(InventoryGuards.IsInvalid);

        var data = new InventoryData { items = items };
        SaveLoadService.SaveInventoryForRace(race, data);
    }

    private void SaveIfCleaned()
    {
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

    /// <summary>rolled�� �����ϴ� ��� ����ȭ ȣȯ(������ null ���) �� �⺻�� ����.</summary>
    private static void EnsureRolledShapeIfPresent(InventoryItem item)
    {
        if (item == null) return;
        if (item.rolled == null) return;

        void Fix(ref float v)
        {
            if (float.IsNaN(v) || float.IsInfinity(v)) v = 0f;
        }

        Fix(ref item.rolled.hp);
        Fix(ref item.rolled.mp);
        Fix(ref item.rolled.atk);
        Fix(ref item.rolled.def);
        Fix(ref item.rolled.dex);
        Fix(ref item.rolled.As);
        Fix(ref item.rolled.cc);
        Fix(ref item.rolled.cd);
    }
}
