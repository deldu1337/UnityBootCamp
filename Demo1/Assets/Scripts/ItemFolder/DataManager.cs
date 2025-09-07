using System;
using System.Collections.Generic;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset; // TextAsset Ÿ�� ���

// JSON���� ������ �迭�� �б� ���� ���� Ŭ����
[Serializable]
public class ItemDataArray
{
    public ItemData[] items; // ItemData ��ü �迭
}

// ���� ��ü���� ������ �����͸� �����ϴ� �̱��� Ŭ����
public class DataManager : MonoBehaviour
{
    private static DataManager instance; // �̱��� �ν��Ͻ�
    public Dictionary<int, ItemData> dicItemDatas; // ID ������� ItemData�� ����
    private DataManager() { } // �ܺο��� ���� ���� (�̱���)

    // �̱��� �ν��Ͻ��� ��ȯ
    public static DataManager GetInstance()
    {
        if (DataManager.instance == null)
            DataManager.instance = new DataManager();
        return DataManager.instance;
    }

    // Resources ���� �� JSON ���Ͽ��� ������ �����͸� �ε�
    public void LoadDatas()
    {
        // Resources/Datas/itemData.json ���� �ε�
        TextAsset textAsset = Resources.Load<TextAsset>("Datas/itemData");
        if (textAsset == null)
        {
            Debug.LogError("Resources/Datas/itemData.json ������ Ȯ���ϼ���!");
            return;
        }
        var json = textAsset.text; // JSON ���ڿ� ����

        // JSON �迭�� ����(ItemDataArray)�� ���μ� �Ľ�
        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);
        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("JSON �Ľ� ����! JSON ���� Ȯ�� �ʿ�.");
            return;
        }

        // ID�� Ű��, ItemData�� ������ �ϴ� ��ųʸ� �ʱ�ȭ
        dicItemDatas = new Dictionary<int, ItemData>();

        // �� ������ �����͸� ��ųʸ��� �߰�
        foreach (var data in wrapper.items)
        {
            dicItemDatas[data.id] = data; // ������ ID �������� ����
            Debug.LogFormat($"{data.id}, {data.name}, {data.atk}"); // �ε� Ȯ�ο� �α�
        }
    }
}