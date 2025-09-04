using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public string uniqueId; // ���� ID (UUID)
    public int id;      // DataManager ������ ID
    public ItemData data;   // ������ ������
    public Sprite icon;      // �κ��丮 UI�� ������
}

[Serializable]
public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();

    // Dictionary ��ȯ
    public Dictionary<string, InventoryItem> ToDictionary()
    {
        var dict = new Dictionary<string, InventoryItem>();
        foreach (var item in items)
            dict[item.uniqueId] = item;
        return dict;
    }

    // Dictionary���� List�� ��ȯ
    public void FromDictionary(Dictionary<string, InventoryItem> dict)
    {
        items.Clear();
        foreach (var kvp in dict)
            items.Add(kvp.Value);
    }
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Panel ��ü ���� ��õ
    private Button[] inventoryButtons;
    private Transform buttonContainer; // 20�� ��ư�� ����ִ� �θ�

    private bool isOpen;
    private DataManager dataManager;
    private string inventoryFilePath;
    private Inventory inventory;
    private Dictionary<string, InventoryItem> inventoryDict = new Dictionary<string, InventoryItem>();

    private void Start()
    {
        if (inventoryPanel == null)
            inventoryPanel = GameObject.Find("InventoryPanel");

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            // ��ư �����̳� �������� (Scroll View �� Viewport �� Content)
            buttonContainer = inventoryPanel.transform.GetChild(0).GetChild(0);
            inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        }

        isOpen = false;

        // ������ name �ҷ�����
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        LoadInventory();

        //Transform myChildren = inventoryPanel.transform.GetChild(0).GetChild(0);
        //Debug.Log(myChildren.name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen; // ���� ���
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isOpen);
                if (isOpen)
                    RefreshInventoryUI(); // �κ��丮 ���� �� UI ����
            }
        }
    }

    private void RefreshInventoryUI()
    {
        // 1) ��� ��ư ��Ȱ��ȭ
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        // 2) �κ��丮 ������ ��ȸ �� ��ư Ȱ��ȭ & ������ ����
        int index = 0;
        foreach (var kvp in inventoryDict)
        {
            if (index >= inventoryButtons.Length)
                break; // ��ư ���� �ʰ� ����

            var item = kvp.Value;

            // ��ư Ȱ��ȭ
            var button = inventoryButtons[index];
            button.gameObject.SetActive(true);

            //������ ��������(�������� SpriteRenderer����)
            var image = button.GetComponent<Image>();
            if (image != null && item.icon != null)
                image.sprite = item.icon; // or item.icon (����� ������)

            index++;
        }
    }

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
            inventory = new Inventory();
            inventoryDict = new Dictionary<string, InventoryItem>();
            SaveInventory();
        }
    }

    public void AddItemToInventory(int id, Sprite icon)
    {
        if (!dataManager.dicItemDatas.ContainsKey(id))
        {
            Debug.LogWarning($"������ ID {id} �� DataManager�� ����!");
            return;
        }

        string uniqueId = Guid.NewGuid().ToString(); // ���� ID ����

        var newItem = new InventoryItem
        {
            uniqueId = uniqueId,
            id = id,
            data = dataManager.dicItemDatas[id],
            icon = icon
        };

        inventoryDict.Add(uniqueId, newItem);
        Debug.Log($"������ �߰���: {newItem.data.name} (uniqueId: {uniqueId})");
        SaveInventory();

        // �κ��丮 UI�� ���� ������ �ٷ� ����
        if (isOpen)
            RefreshInventoryUI();
    }

    private void SaveInventory()
    {
        inventory.FromDictionary(inventoryDict);
        string json = JsonUtility.ToJson(inventory, true);
        File.WriteAllText(inventoryFilePath, json);
        Debug.Log($"�κ��丮 ����� �� {inventoryFilePath}");
    }
}