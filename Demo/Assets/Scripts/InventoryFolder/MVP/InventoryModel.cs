using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 인벤토리 아이템 데이터
/// </summary>
[Serializable]
public class InventoryItem
{
    public string uniqueId;          // 고유 ID (중복 방지)
    public int id;                   // 아이템 기본 ID (DataManager 참조)
    public ItemData data;            // 아이템 메타 데이터
    public string iconPath;          // 아이콘 리소스 경로
    public string prefabPath;        // 프리팹 리소스 경로 (JSON 저장용)
    public GameObject prefab => Resources.Load<GameObject>(prefabPath); // 런타임 로드
}

/// <summary>
/// 인벤토리 전체 데이터를 JSON 직렬화용으로 감싸는 클래스
/// </summary>
[Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();
}

/// <summary>
/// 인벤토리 모델 (데이터 관리, 저장/로드, CRUD)
/// </summary>
public class InventoryModel
{
    private Dictionary<string, InventoryItem> itemDict = new Dictionary<string, InventoryItem>(); // uniqueId 기반
    private string filePath;

    // 읽기 전용 아이템 리스트
    public IReadOnlyList<InventoryItem> Items => new List<InventoryItem>(itemDict.Values);

    public InventoryModel()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");
        Load(); // 생성 시 JSON에서 불러오기
    }

    /// <summary>
    /// 아이템 추가 또는 덮어쓰기
    /// </summary>
    public void AddItem(InventoryItem item)
    {
        if (!itemDict.ContainsKey(item.uniqueId))
            itemDict[item.uniqueId] = item;
        Save();
    }

    /// <summary>
    /// 인덱스 기준 아이템 위치 교체
    /// </summary>
    public void SwapItem(int indexA, int indexB)
    {
        var list = new List<InventoryItem>(itemDict.Values);
        if (indexA == indexB || indexA < 0 || indexB < 0 || indexA >= list.Count || indexB >= list.Count)
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

    /// <summary>
    /// 인덱스 기반 아이템 삭제
    /// </summary>
    public void RemoveAt(int index)
    {
        var list = new List<InventoryItem>(itemDict.Values);
        if (index < 0 || index >= list.Count) return;

        var item = list[index];
        RemoveItem(item.uniqueId);
    }

    /// <summary>
    /// 중복 방지하며 아이템 추가
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
    /// JSON에서 인벤토리 로드
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
            Save(); // 파일 없으면 초기화 후 생성
        }
    }

    /// <summary>
    /// JSON으로 인벤토리 저장
    /// </summary>
    public void Save()
    {
        var data = new InventoryData { items = new List<InventoryItem>(itemDict.Values) };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"인벤토리 저장됨 → {filePath}");
    }

    /// <summary>
    /// uniqueId 기반 삭제
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
