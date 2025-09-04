using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public string uniqueId; // 아이템 고유 ID (UUID)
    public int id;          // DataManager에서 관리하는 아이템 ID
    public ItemData data;   // 아이템 데이터 (이름, 공격력 등)
    public Sprite icon;     // 인벤토리 UI에서 사용할 아이콘
}

[Serializable]
public class Inventory
{
    public List<InventoryItem> items = new List<InventoryItem>();

    // List → Dictionary 변환 (빠른 검색용)
    public Dictionary<string, InventoryItem> ToDictionary()
    {
        var dict = new Dictionary<string, InventoryItem>();
        foreach (var item in items)
            dict[item.uniqueId] = item;
        return dict;
    }

    // Dictionary → List 변환 (저장용)
    public void FromDictionary(Dictionary<string, InventoryItem> dict)
    {
        items.Clear();
        foreach (var kvp in dict)
            items.Add(kvp.Value);
    }
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // 인벤토리 UI Panel
    private Button[] inventoryButtons;                  // 인벤토리 슬롯 버튼 배열
    private Transform buttonContainer;                  // 버튼들이 들어있는 부모 Transform

    private bool isOpen;                                // 인벤토리 열림 상태
    private DataManager dataManager;                    // 아이템 데이터를 관리하는 싱글톤
    private string inventoryFilePath;                   // 인벤토리 저장 경로
    private Inventory inventory;                        // 인벤토리 데이터
    private Dictionary<string, InventoryItem> inventoryDict = new Dictionary<string, InventoryItem>(); // 검색용 Dictionary

    void Start()
    {
        // Panel이 할당되지 않았다면 씬에서 검색
        if (inventoryPanel == null)
            inventoryPanel = GameObject.Find("InventoryPanel");

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false); // 처음엔 비활성화

            // Scroll View → Viewport → Content 순서로 버튼 컨테이너 가져오기
            buttonContainer = inventoryPanel.transform.GetChild(0).GetChild(0);

            // 버튼 배열 가져오기
            inventoryButtons = buttonContainer.GetComponentsInChildren<Button>(true);
        }

        isOpen = false;

        // DataManager에서 아이템 데이터 불러오기
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // 저장된 인벤토리 불러오기
        LoadInventory();
    }

    void Update()
    {
        // I 키 입력 시 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(isOpen);

                // 인벤토리 열릴 때 UI 갱신
                if (isOpen)
                    RefreshInventoryUI();
            }
        }
    }

    // 인벤토리 UI 갱신
    public void RefreshInventoryUI()
    {
        // 1) 모든 버튼 비활성화
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        // 2) 인벤토리 아이템 순회
        int index = 0;
        foreach (var kvp in inventoryDict)
        {
            if (index >= inventoryButtons.Length)
                break; // 슬롯 초과 방지

            var item = kvp.Value;

            // 버튼 활성화
            var button = inventoryButtons[index];
            button.gameObject.SetActive(true);

            // 버튼 이미지 변경
            var image = button.GetComponent<Image>();
            if (image != null && item.icon != null)
                image.sprite = item.icon; // 저장된 아이콘 사용

            // 드래그 가능하게 Component 추가
            DraggableItem draggable = button.GetComponent<DraggableItem>();
            if (draggable == null)
            {
                draggable = button.gameObject.AddComponent<DraggableItem>();
                draggable.playerInventory = this; // PlayerInventory 참조 전달
            }

            index++;
        }
    }

    public void SwapInventoryData(int indexA, int indexB)
    {
        // inventoryDict → List로 변환
        List<InventoryItem> itemList = new List<InventoryItem>(inventoryDict.Values);

        if (indexA < 0 || indexA >= itemList.Count || indexB < 0 || indexB >= itemList.Count)
            return;

        // 순서 교환
        InventoryItem temp = itemList[indexA];
        itemList[indexA] = itemList[indexB];
        itemList[indexB] = temp;

        // Dictionary 갱신
        inventoryDict.Clear();
        foreach (var item in itemList)
            inventoryDict[item.uniqueId] = item;

        // 저장
        SaveInventory();
    }

    // 인벤토리 파일에서 데이터 불러오기
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
            // 파일 없으면 새 인벤토리 생성 후 저장
            inventory = new Inventory();
            inventoryDict = new Dictionary<string, InventoryItem>();
            SaveInventory();
        }
    }

    // 인벤토리에 아이템 추가
    public void AddItemToInventory(int id, Sprite icon)
    {
        // DataManager에 아이템 없으면 경고
        if (!dataManager.dicItemDatas.ContainsKey(id))
        {
            Debug.LogWarning($"아이템 ID {id} 가 DataManager에 없음!");
            return;
        }

        string uniqueId = Guid.NewGuid().ToString(); // UUID 생성

        var newItem = new InventoryItem
        {
            uniqueId = uniqueId,
            id = id,
            data = dataManager.dicItemDatas[id],
            icon = icon
        };

        // Dictionary에 추가
        inventoryDict.Add(uniqueId, newItem);
        Debug.Log($"아이템 추가됨: {newItem.data.name} (uniqueId: {uniqueId})");

        // 파일 저장
        SaveInventory();

        // 인벤토리 UI가 열려있으면 바로 갱신
        if (isOpen)
            RefreshInventoryUI();
    }

    // 인벤토리 파일 저장
    private void SaveInventory()
    {
        inventory.FromDictionary(inventoryDict); // Dictionary → List
        string json = JsonUtility.ToJson(inventory, true); // JSON 변환
        File.WriteAllText(inventoryFilePath, json);       // 파일 쓰기
        Debug.Log($"인벤토리 저장됨 → {inventoryFilePath}");
    }
}