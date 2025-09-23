using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataArray
{
    public ItemData[] items; // ItemData 객체 배열
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public Dictionary<int, ItemData> dicItemDatas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadDatas();
    }

    public void LoadDatas()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Datas/itemData");
        if (textAsset == null)
        {
            Debug.LogError("Resources/Datas/itemData.json 파일을 확인하세요!");
            return;
        }

        var json = textAsset.text;
        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);

        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("JSON 파싱 실패! JSON 형식 확인 필요.");
            return;
        }

        dicItemDatas = new Dictionary<int, ItemData>();
        foreach (var data in wrapper.items)
            dicItemDatas[data.id] = data;
    }
}