using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using TextAsset = UnityEngine.TextAsset;

[Serializable]
public class ItemDataArray
{
    public ItemData[] items;
}

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public Dictionary<int, ItemData> dicItemDatas;
    private DataManager() { }

    public static DataManager GetInstance()
    {
        if(DataManager.instance == null)
            DataManager.instance = new DataManager();
        return DataManager.instance;
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

        // �迭 JSON�̸� ���۷� ���α�
        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);
        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("JSON �Ľ� ����! JSON ���� Ȯ�� �ʿ�.");
            return;
        }
        dicItemDatas = new Dictionary<int, ItemData>();

        foreach (var data in wrapper.items)
        {
            dicItemDatas[data.id] = data;
            Debug.LogFormat($"{data.id}, {data.name}, {data.atk}");
        }
    }
}
