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
    public string iconPath; // 아이콘 리소스 경로
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
    public GameObject InventoryPanel => inventoryPanel;

    private Button ExitButton;

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
            // ExitButton 찾기
            ExitButton = inventoryPanel.transform.GetComponentInChildren<Button>();

            // 버튼 클릭 이벤트 등록
            ExitButton.onClick.AddListener(() =>
            {
                CloseInventory();
            });

            inventoryPanel.SetActive(false); // 처음엔 비활성화

            buttonContainer = inventoryPanel.transform.Find("InventoryUI/TextPanel");
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

    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
            Debug.Log("인벤토리 닫힘");
        }
        else
        {
            Debug.LogWarning("InventoryPanel이 할당되지 않았습니다.");
        }
    }


    // 인벤토리 UI 갱신
    public void RefreshInventoryUI()
    {
        // 모든 버튼 비활성화
        foreach (var btn in inventoryButtons)
            btn.gameObject.SetActive(false);

        // 인벤토리 List 기반 UI
        for (int i = 0; i < inventory.items.Count && i < inventoryButtons.Length; i++)
        {
            var item = inventory.items[i];
            var button = inventoryButtons[i];
            button.gameObject.SetActive(true);

            // 아이콘 적용
            var image = button.GetComponent<Image>();
            if (image != null && !string.IsNullOrEmpty(item.iconPath))
            {
                Sprite icon = Resources.Load<Sprite>(item.iconPath);
                if (icon != null)
                    image.sprite = icon;
            }

            // 드래그 컴포넌트 연결
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

        // Dictionary 동기화
        inventoryDict.Clear();
        foreach (var item in inventory.items)
            inventoryDict[item.uniqueId] = item;

        SaveInventory();
        RefreshInventoryUI();
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
        string iconPath = "Icons/" + icon.name; // 예: Resources/Icons 폴더 기준

        var newItem = new InventoryItem
        {
            uniqueId = uniqueId,
            id = id,
            data = dataManager.dicItemDatas[id],
            iconPath = iconPath
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