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
        SaveIfCleaned();      // ← 클린업된 내용 즉시 반영
    }

    public InventoryItem GetItemById(string uniqueId)
    {
        return items.Find(i => i.uniqueId == uniqueId);
    }

    public void AddItem(InventoryItem item)
    {
        if (InventoryGuards.IsInvalid(item))
        {
            Debug.LogWarning("[InventoryModel] 무효 아이템 추가 시도 → 무시");
            return;
        }
        if (items.Exists(i => i.uniqueId == item.uniqueId))
        {
            Debug.LogWarning($"[InventoryModel] 중복 uniqueId 추가 시도({item.uniqueId}) → 무시");
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
    /// uniqueId 기반으로 아이템 순서를 재배치하고 JSON에 저장
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
            Debug.LogWarning("[InventoryModel] 재배치 중 무효 아이템 발견 → 제거");
            items.RemoveAt(fromIndex);
            Save();
            return;
        }
        items.RemoveAt(fromIndex);

        // fromIndex < toIndex였으면 toIndex를 -1 해줘야 정상적인 위치
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
        // 통합 서비스 사용
        var data = SaveLoadService.LoadInventoryOrNew();
        items = data.items ?? new List<InventoryItem>();

        // 로드시 무효 아이템 정리
        int before = items.Count;
        items.RemoveAll(InventoryGuards.IsInvalid);
        int after = items.Count;
        if (before != after)
            Debug.LogWarning($"[InventoryModel] 로드시 무효 아이템 {before - after}개 정리함");
    }

    public void Save()
    {
        // 저장 전에도 무효 아이템 정리(이중 안전망)
        items.RemoveAll(InventoryGuards.IsInvalid);

        var data = new InventoryData { items = items };
        SaveLoadService.SaveInventory(data);
    }

    // 생성자에서 호출하는 “정리되었으면 한번 더 저장” 헬퍼
    private void SaveIfCleaned()
    {
        // 파일과 메모리의 아이템 수가 다르면 정리된 것으로 판단
        if (!File.Exists(filePath)) { Save(); return; }

        try
        {
            string json = File.ReadAllText(filePath);
            var onDisk = JsonUtility.FromJson<InventoryData>(json)?.items ?? new List<InventoryItem>();
            int diskCount = onDisk.Count;
            int memCount = items.Count;
            if (diskCount != memCount)
            {
                Debug.LogWarning($"[InventoryModel] 초기 로드 시 정리 반영: {diskCount} → {memCount}");
                Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[InventoryModel] SaveIfCleaned 중 파일 비교 실패 → Save 강행: {e}");
            Save();
        }
    }
}
