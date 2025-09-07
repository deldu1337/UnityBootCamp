using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public string uniqueId; // ������ ���� ID (UUID)
    public int id;          // DataManager���� �����ϴ� ������ ID
    public ItemData data;   // ������ ������ (�̸�, ���ݷ� ��)
    public Sprite icon;     // �κ��丮 UI���� ����� ������
}

[Serializable]
public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();

    // List �� Dictionary ��ȯ (���� �˻���)
    public Dictionary<string, InventoryItem> ToDictionary()
    {
        var dict = new Dictionary<string, InventoryItem>();
        foreach (var item in items)
            dict[item.uniqueId] = item;
        return dict;
    }

    // Dictionary �� List ��ȯ (�����)
    public void FromDictionary(Dictionary<string, InventoryItem> dict)
    {
        items.Clear();
        foreach (var kvp in dict)
            items.Add(kvp.Value);
    }
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // �κ��丮 UI Panel
    private Button[] inventoryButtons;                  // �κ��丮 ���� ��ư �迭
    private Transform buttonContainer;                  // ��ư���� ����ִ� �θ� Transform

    private bool isOpen;                                // �κ��丮 ���� ����
    private DataManager dataManager;                    // ������ �����͸� �����ϴ� �̱���
    private string inventoryFilePath;                   // �κ��丮 ���� ���
    private Inventory inventory;                        // �κ��丮 ������
    private Dictionary<string, InventoryItem> inventoryDict = new Dictionary<string, InventoryItem>(); // �˻��� Dictionary

    void Start()
    {
        // Panel�� �Ҵ���� �ʾҴٸ� ������ �˻�
        if (inventoryPanel == null)
            inventoryPanel = GameObject.Find("InventoryPanel");

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // ó���� ��Ȱ��ȭ

            // Scroll View �� Viewport �� Content ������ ��ư �����̳� ��������
            buttonContainer = inventoryPanel.transform.GetChild(0).GetChild(0);

            // ��ư �迭 ��������
            inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        }

        isOpen = false;

        // DataManager���� ������ ������ �ҷ�����
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // ����� �κ��丮 �ҷ�����
        LoadInventory();
    }

    void Update()
    {
        // I Ű �Է� �� �κ��丮 ���
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isOpen);

                // �κ��丮 ���� �� UI ����
                if (isOpen)
                    RefreshInventoryUI();
            }
        }
    }

    // �κ��丮 UI ����
    public void RefreshInventoryUI()
    {
        // 1) ��� ��ư ��Ȱ��ȭ
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        // 2) �κ��丮 ������ ��ȸ
        int index = 0;
        foreach (var kvp in inventoryDict)
        {
            if (index >= inventoryButtons.Length)
                break; // ���� �ʰ� ����

            var item = kvp.Value;

            // ��ư Ȱ��ȭ
            var button = inventoryButtons[index];
            button.gameObject.SetActive(true);

            // ��ư �̹��� ����
            var image = button.GetComponent<Image>();
            if (image != null && item.icon != null)
                image.sprite = item.icon; // ����� ������ ���

            // �巡�� �����ϰ� Component �߰�
            DraggableItem draggable = button.GetComponent<DraggableItem>();
            if (draggable == null)
            {
                draggable = button.gameObject.AddComponent<DraggableItem>();
                draggable.playerInventory = this; // PlayerInventory ���� ����
            }

            index++;
        }
    }

    public void SwapInventoryData(int indexA, int indexB)
    {
        // inventoryDict �� List�� ��ȯ
        List<InventoryItem> itemList = new List<InventoryItem>(inventoryDict.Values);

        if (indexA < 0 || indexA >= itemList.Count || indexB < 0 || indexB >= itemList.Count)
            return;

        // ���� ��ȯ
        InventoryItem temp = itemList[indexA];
        itemList[indexA] = itemList[indexB];
        itemList[indexB] = temp;

        // Dictionary ����
        inventoryDict.Clear();
        foreach (var item in itemList)
            inventoryDict[item.uniqueId] = item;

        // ����
        SaveInventory();
    }

    // �κ��丮 ���Ͽ��� ������ �ҷ�����
    public void LoadInventory()
    {
        inventoryFilePath = Path.Combine(Application.persistentDataPath, "playerInventory.json");

        if (File.Exists(inventoryFilePath))
        {
            string json = File.ReadAllText(inventoryFilePath);
            inventory = JsonUtility.FromJson<Inventory>(json);

            // Dictionary�� ��ȯ
            inventoryDict = inventory.ToDictionary();
        }
        else
        {
            // ���� ������ �� �κ��丮 ���� �� ����
            inventory = new Inventory();
            inventoryDict = new Dictionary<string, InventoryItem>();
            SaveInventory();
        }
    }

    // �κ��丮�� ������ �߰�
    public void AddItemToInventory(int id, Sprite icon)
    {
        // DataManager�� ������ ������ ���
        if (!dataManager.dicItemDatas.ContainsKey(id))
        {
            Debug.LogWarning($"������ ID {id} �� DataManager�� ����!");
            return;
        }

        string uniqueId = Guid.NewGuid().ToString(); // UUID ����

        var newItem = new InventoryItem
        {
            uniqueId = uniqueId,
            id = id,
            data = dataManager.dicItemDatas[id],
            icon = icon
        };

        // Dictionary�� �߰�
        inventoryDict.Add(uniqueId, newItem);
        Debug.Log($"������ �߰���: {newItem.data.name} (uniqueId: {uniqueId})");

        // ���� ����
        SaveInventory();

        // �κ��丮 UI�� ���������� �ٷ� ����
        if (isOpen)
            RefreshInventoryUI();
    }

    // �κ��丮 ���� ����
    private void SaveInventory()
    {
        inventory.FromDictionary(inventoryDict); // Dictionary �� List
        string json = JsonUtility.ToJson(inventory, true); // JSON ��ȯ
        File.WriteAllText(inventoryFilePath, json);       // ���� ����
        Debug.Log($"�κ��丮 ����� �� {inventoryFilePath}");
    }
}