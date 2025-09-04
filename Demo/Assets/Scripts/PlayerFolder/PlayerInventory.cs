using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public string uniqueId; // 고유 ID (UUID)
    public int id;      // DataManager 아이템 ID
    public ItemData data;   // 아이템 데이터
    public Sprite icon;      // 인벤토리 UI용 아이콘
}

[Serializable]
public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();

    // Dictionary 변환
    public Dictionary<string, InventoryItem> ToDictionary()
    {
        var dict = new Dictionary<string, InventoryItem>();
        foreach (var item in items)
            dict[item.uniqueId] = item;
        return dict;
    }

    // Dictionary에서 List로 변환
    public void FromDictionary(Dictionary<string, InventoryItem> dict)
    {
        items.Clear();
        foreach (var kvp in dict)
            items.Add(kvp.Value);
    }
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Panel 전체 연결 추천
    private Button[] inventoryButtons;
    private Transform buttonContainer; // 20개 버튼이 들어있는 부모

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
            // 버튼 컨테이너 가져오기 (Scroll View → Viewport → Content)
            buttonContainer = inventoryPanel.transform.GetChild(0).GetChild(0);
            inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        }

        isOpen = false;

        // 아이템 name 불러오기
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
            isOpen = !isOpen; // 상태 토글
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isOpen);
                if (isOpen)
                    RefreshInventoryUI(); // 인벤토리 열릴 때 UI 갱신
            }
        }
    }

    private void RefreshInventoryUI()
    {
        // 1) 모든 버튼 비활성화
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        // 2) 인벤토리 아이템 순회 → 버튼 활성화 & 아이콘 적용
        int index = 0;
        foreach (var kvp in inventoryDict)
        {
            if (index >= inventoryButtons.Length)
                break; // 버튼 개수 초과 방지

            var item = kvp.Value;

            // 버튼 활성화
            var button = inventoryButtons[index];
            button.gameObject.SetActive(true);

            //아이콘 가져오기(프리팹의 SpriteRenderer에서)
            var image = button.GetComponent<Image>();
            if (image != null && item.icon != null)
                image.sprite = item.icon; // or item.icon (저장된 아이콘)

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

            // Dictionary로 변환
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
            Debug.LogWarning($"아이템 ID {id} 가 DataManager에 없음!");
            return;
        }

        string uniqueId = Guid.NewGuid().ToString(); // 고유 ID 생성

        var newItem = new InventoryItem
        {
            uniqueId = uniqueId,
            id = id,
            data = dataManager.dicItemDatas[id],
            icon = icon
        };

        inventoryDict.Add(uniqueId, newItem);
        Debug.Log($"아이템 추가됨: {newItem.data.name} (uniqueId: {uniqueId})");
        SaveInventory();

        // 인벤토리 UI가 열려 있으면 바로 갱신
        if (isOpen)
            RefreshInventoryUI();
    }

    private void SaveInventory()
    {
        inventory.FromDictionary(inventoryDict);
        string json = JsonUtility.ToJson(inventory, true);
        File.WriteAllText(inventoryFilePath, json);
        Debug.Log($"인벤토리 저장됨 → {inventoryFilePath}");
    }
}