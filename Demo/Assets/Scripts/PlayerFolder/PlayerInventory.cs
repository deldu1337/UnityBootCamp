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
    public string iconPath; // ������ ���ҽ� ���
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
    public GameObject InventoryPanel => inventoryPanel;

    private Button ExitButton;

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
            // ExitButton ã��
            ExitButton = inventoryPanel.transform.GetComponentInChildren<Button>();

            // ��ư Ŭ�� �̺�Ʈ ���
            ExitButton.onClick.AddListener(() =>
            {
                CloseInventory();
            });

            inventoryPanel.SetActive(false); // ó���� ��Ȱ��ȭ

            buttonContainer = inventoryPanel.transform.Find("InventoryUI/TextPanel");
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

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
            Debug.Log("�κ��丮 ����");
        }
        else
        {
            Debug.LogWarning("InventoryPanel�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }


    // �κ��丮 UI ����
    public void RefreshInventoryUI()
    {
        // ��� ��ư ��Ȱ��ȭ
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        // �κ��丮 List ��� UI
        for (int i = 0; i < inventory.items.Count && i < inventoryButtons.Length; i++)
        {
            var item = inventory.items[i];
            var button = inventoryButtons[i];
            button.gameObject.SetActive(true);

            // ������ ����
            var image = button.GetComponent<Image>();
            if (image != null && !string.IsNullOrEmpty(item.iconPath))
            {
                Sprite icon = Resources.Load<Sprite>(item.iconPath);
                if (icon != null)
                    image.sprite = icon;
            }

            // �巡�� ������Ʈ ����
            DraggableItem draggable = button.GetComponent<DraggableItem>();
            if (draggable == null)
                draggable = button.gameObject.AddComponent<DraggableItem>();

            draggable.playerInventory = this;
        }
    }

    public void SwapInventoryData(int indexA, int indexB)
    {
        if (indexA == indexB || indexA < 0 || indexB < 0 ||
            indexA >= inventory.items.Count || indexB >= inventory.items.Count)
            return;

        var temp = inventory.items[indexA];
        inventory.items[indexA] = inventory.items[indexB];
        inventory.items[indexB] = temp;

        // Dictionary ����ȭ
        inventoryDict.Clear();
        foreach (var item in inventory.items)
            inventoryDict[item.uniqueId] = item;

        SaveInventory();
        RefreshInventoryUI();
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
        string iconPath = "Icons/" + icon.name; // ��: Resources/Icons ���� ����

        var newItem = new InventoryItem
        {
            uniqueId = uniqueId,
            id = id,
            data = dataManager.dicItemDatas[id],
            iconPath = iconPath
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