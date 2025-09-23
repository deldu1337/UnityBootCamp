using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDataArray
{
    public ItemData[] items; // ItemData ��ü �迭
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
            Debug.LogError("Resources/Datas/itemData.json ������ Ȯ���ϼ���!");
            return;
        }

        var json = textAsset.text;
        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);

        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("JSON �Ľ� ����! JSON ���� Ȯ�� �ʿ�.");
            return;
        }

        dicItemDatas = new Dictionary<int, ItemData>();
        foreach (var data in wrapper.items)
            dicItemDatas[data.id] = data;
    }
}