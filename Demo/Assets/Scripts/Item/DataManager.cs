//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class ItemDataArray
//{
//    public ItemData[] items; // ItemData ��ü �迭
//}

//public class DataManager : MonoBehaviour
//{
//    public static DataManager Instance { get; private set; }
//    public Dictionary<int, ItemData> dicItemDatas;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        LoadDatas();
//    }

//    public void LoadDatas()
//    {
//        TextAsset textAsset = Resources.Load<TextAsset>("Datas/itemData");
//        if (textAsset == null)
//        {
//            Debug.LogError("Resources/Datas/itemData.json ������ Ȯ���ϼ���!");
//            return;
//        }

//        var json = textAsset.text;
//        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);

//        if (wrapper == null || wrapper.items == null)
//        {
//            Debug.LogError("JSON �Ľ� ����! JSON ���� Ȯ�� �ʿ�.");
//            return;
//        }

//        dicItemDatas = new Dictionary<int, ItemData>();
//        foreach (var data in wrapper.items)
//            dicItemDatas[data.id] = data;
//    }
//}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataArray { public ItemData[] items; }

[Serializable]
public class ItemStatRange { public float min; public float max; }

[Serializable]
public class ItemRangeEntry
{
    public int id;
    public ItemStatRange hp;
    public ItemStatRange mp;
    public ItemStatRange atk;
    public ItemStatRange def;
    public ItemStatRange dex;
    public ItemStatRange As;
    public ItemStatRange cc;
    public ItemStatRange cd;
}

[Serializable]
public class ItemRangeArray { public ItemRangeEntry[] items; }

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public Dictionary<int, ItemData> dicItemDatas;

    // �� �߰�: �����ۺ� ���� ���� ��
    // id -> (stat -> range)
    private readonly Dictionary<int, Dictionary<string, ItemStatRange>> _ranges
        = new Dictionary<int, Dictionary<string, ItemStatRange>>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadDatas();
        LoadRanges(); // �� �߰�
    }

    public void LoadDatas()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Datas/itemData");
        if (textAsset == null) { Debug.LogError("Resources/Datas/itemData.json ������ Ȯ���ϼ���!"); return; }

        var json = textAsset.text;
        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);
        if (wrapper == null || wrapper.items == null) { Debug.LogError("JSON �Ľ� ����! JSON ���� Ȯ�� �ʿ�."); return; }

        dicItemDatas = new Dictionary<int, ItemData>();
        foreach (var data in wrapper.items) dicItemDatas[data.id] = data;
    }

    // �� ���� JSON �δ� (��: Resources/Datas/itemRanges.json)
    private void LoadRanges()
    {
        _ranges.Clear();

        var ta = Resources.Load<TextAsset>("Datas/itemRanges");
        if (ta == null)
        {
            Debug.LogWarning("[DataManager] itemRanges.json �� ã�� ���߽��ϴ�. (���� �Ѹ� ��Ȱ��)");
            return;
        }

        var wrapper = JsonUtility.FromJson<ItemRangeArray>(ta.text);
        if (wrapper?.items == null) return;

        foreach (var e in wrapper.items)
        {
            var map = new Dictionary<string, ItemStatRange>();
            if (e.hp != null) map["hp"] = e.hp;
            if (e.mp != null) map["mp"] = e.mp;
            if (e.atk != null) map["atk"] = e.atk;
            if (e.def != null) map["def"] = e.def;
            if (e.dex != null) map["dex"] = e.dex;
            if (e.As != null) map["As"] = e.As;
            if (e.cc != null) map["cc"] = e.cc;
            if (e.cd != null) map["cd"] = e.cd;

            _ranges[e.id] = map;
        }
    }

    // �� �ܺο��� ���� ��û
    public bool TryGetRange(int itemId, string stat, out ItemStatRange range)
    {
        range = null;
        if (_ranges.TryGetValue(itemId, out var map))
            return map.TryGetValue(stat, out range);
        return false;
    }
}
