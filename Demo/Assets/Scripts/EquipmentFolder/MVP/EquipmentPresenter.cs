using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

/// <summary>
/// 장비 UI와 캐릭터 모델을 연결하고 장착/해제 및 스탯 계산을 담당하는 Presenter
/// MVC 패턴에서 Presenter 역할
/// </summary>
public class EquipmentPresenter : MonoBehaviour
{
    // 모델과 뷰
    private EquipmentModel model;    // 장비 데이터 모델
    private EquipmentView view;      // UI 뷰
    private InventoryPresenter inventoryPresenter; // 인벤토리 Presenter

    // UI 카메라 및 캐릭터
    [SerializeField] private Camera uiCamera;               // 장비 UI 전용 카메라
    [SerializeField] private Transform targetCharacter;     // 장비를 표시할 캐릭터 트랜스폼
    [SerializeField] private PlayerCombatStats playerCombatStats; // 캐릭터 스탯 계산용

    private bool isOpen; // 장비창 열림 상태

    /// <summary>
    /// 초기화: 모델 생성, 뷰 찾기, UI카메라 자동 할당, 초기 장착 아이템 로드
    /// </summary>
    void Start()
    {
        model = new EquipmentModel();
        view = FindAnyObjectByType<EquipmentView>();
        if (view == null)
        {
            Debug.LogError("EquipmentView를 찾을 수 없습니다! 씬에 EquipmentView가 있는지 확인하세요.");
            return;
        }

        // 인벤토리 Presenter 자동 할당
        if (inventoryPresenter == null)
        {
            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
            if (inventoryPresenter == null)
            {
                Debug.LogWarning("InventoryPresenter를 찾을 수 없습니다. 인벤토리 기능이 정상 동작하지 않을 수 있습니다.");
            }
        }

        playerCombatStats = gameObject.GetComponent<PlayerCombatStats>();

        // UICharacter 레이어 카메라 자동 할당
        if (uiCamera == null)
        {
            int uiLayer = LayerMask.NameToLayer("UICharacter");
            if (uiLayer == -1)
                Debug.LogError("UICharacter 레이어가 존재하지 않습니다! 레이어를 추가하세요.");
            else
            {
                Camera[] cameras = GameObject.FindObjectsByType<Camera>(FindObjectsSortMode.None);
                foreach (Camera cam in cameras)
                {
                    if (cam.gameObject.layer == uiLayer)
                    {
                        uiCamera = cam;
                        break;
                    }
                }

                if (uiCamera == null)
                    Debug.LogError("UICharacter 레이어를 가진 카메라를 찾을 수 없습니다! 카메라를 추가하세요.");
            }
        }

        // 뷰 초기화 (닫기 버튼과 드래그 앤 드롭 콜백 연결)
        view.Initialize(CloseEquipment, HandleEquipFromInventory);

        // JSON에 저장된 장비 불러와 캐릭터에 장착
        InitializeEquippedItemsFromJson();

        // UI 갱신
        RefreshEquipmentUI();
        isOpen = false;
    }

    /// <summary>장비창 열기/닫기 단축키 처리</summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquipment();
    }

    /// <summary>
    /// UI 카메라를 캐릭터 앞쪽으로 이동시키고 항상 캐릭터 바라보게 함
    /// </summary>
    void LateUpdate()
    {
        if (uiCamera != null && targetCharacter != null)
        {
            Vector3 offset = targetCharacter.forward * 2.2f + Vector3.up * 1.5f;
            uiCamera.transform.position = targetCharacter.position + offset;
            uiCamera.transform.LookAt(targetCharacter.position + Vector3.up * 1.0f);
        }
    }

    /// <summary>
    /// 인벤토리에서 드래그하여 장착 시 처리
    /// </summary>
    /// <param name="slotType">장착 슬롯 타입</param>
    /// <param name="item">장착할 아이템</param>

    private void HandleEquipFromInventory(string slotType, InventoryItem item)
    {
        Debug.Log($"인벤토리 아이템 {item.data.name} → {slotType} 장착 시도");

        var slot = model.GetSlot(slotType);

        // 기존 아이템을 인벤토리로 돌려보냄
        if (slot != null && slot.equipped != null)
        {
            InventoryItem oldItem = slot.equipped;
            if (inventoryPresenter != null)
            {
                inventoryPresenter.AddExistingItem(oldItem);
            }
        }

        // 새로운 아이템 장착
        model.EquipItem(slotType, item);

        if (!string.IsNullOrEmpty(item.prefabPath))
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
                Handle(prefab, slotType);
        }

        // 스탯 및 UI 갱신
        playerCombatStats?.RecalculateStats(model.Slots);
        RefreshEquipmentUI();
    }


    /// <summary>장비창 열기/닫기 토글</summary>
    private void ToggleEquipment()
    {
        if (view == null) return;

        isOpen = !isOpen;
        view.Show(isOpen);
        if (uiCamera != null)
            uiCamera.gameObject.SetActive(isOpen);

        if (isOpen)
            RefreshEquipmentUI();
    }

    /// <summary>
    /// 캐릭터 슬롯에 프리팹 장착 처리
    /// </summary>
    /// <param name="prefab">장착할 아이템 프리팹</param>
    /// <param name="slot">슬롯 타입</param>
    private void Handle(GameObject prefab, string slot)
    {
        Transform body = null;
        Vector3 offset = Vector3.zero;

        // 슬롯 타입에 따른 본 찾기 및 오프셋 설정
        switch (slot)
        {
            case "weapon":
                body = FindChildByName(targetCharacter, "bone_HandR");
                if (body == null) { Debug.LogWarning("캐릭터 오른손(bone_HandR)을 찾을 수 없음"); return; }
                break;

            case "head":
                body = FindChildByName(targetCharacter, "bone_Head");
                if (body == null) { Debug.LogWarning("캐릭터 머리(bone_Head)를 찾을 수 없음"); return; }
                offset = new Vector3(-0.12f, 0.035f, 0);
                break;

            case "shield":
                body = FindChildByName(targetCharacter, "bone_HandL");
                if (body == null) { Debug.LogWarning("캐릭터 왼손(bone_HandL)를 찾을 수 없음"); return; }
                offset = new Vector3(0, 0, -0.05f);
                break;

            case "lshoulder":
                body = FindChildByName(targetCharacter, "bone_ShoulderL");
                if (body == null) { Debug.LogWarning("캐릭터 왼쪽 어깨(bone_ShoulderL)를 찾을 수 없음"); return; }
                offset = new Vector3(0, 0, -0.2f);
                break;

            case "rshoulder":
                body = FindChildByName(targetCharacter, "bone_ShoulderR");
                if (body == null) { Debug.LogWarning("캐릭터 오른쪽 어깨(bone_ShoulderR)를 찾을 수 없음"); return; }
                offset = new Vector3(0, 0, 0.2f);
                break;

            default:
                Debug.LogWarning($"지원되지 않는 슬롯 타입: {slot}");
                return;
        }

        // 기존 장비 제거
        for (int i = body.childCount - 1; i >= 0; i--)
        {
            Transform child = body.GetChild(i);
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }

        // 새 장비 장착
        GameObject instance = Instantiate(prefab, body);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = offset;
        instance.transform.localRotation = Quaternion.identity;
    }

    /// <summary>JSON에 저장된 장비 데이터를 기반으로 캐릭터에 장착</summary>
    private void InitializeEquippedItemsFromJson()
    {
        foreach (var slot in model.Slots)
        {
            if (slot.equipped != null && !string.IsNullOrEmpty(slot.equipped.prefabPath))
            {
                GameObject prefab = Resources.Load<GameObject>(slot.equipped.prefabPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"프리팹을 찾을 수 없음: {slot.equipped.prefabPath}");
                    continue;
                }
                Handle(prefab, slot.slotType);
            }
        }
        playerCombatStats?.RecalculateStats(model.Slots);
    }

    /// <summary>인덱스 또는 슬롯으로 장착 처리</summary>
    //public void HandleEquipItem(InventoryItem item, int slotIndex)
    //{
    //    model.EquipItem(item.data.type, item);

    //    if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
    //    {
    //        GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
    //        Handle(prefab, item.data.type);
    //    }

    //    playerCombatStats?.RecalculateStats(model.Slots);
    //    RefreshEquipmentUI();
    //}
    public void HandleEquipItem(InventoryItem item, int slotIndex)
    {
        string slotType = item.data.type;
        var slot = model.GetSlot(slotType);

        // 기존 장비가 있으면 인벤토리로 되돌려줌
        if (slot != null && slot.equipped != null)
        {
            InventoryItem oldItem = slot.equipped;
            if (inventoryPresenter != null)
            {
                inventoryPresenter.AddExistingItem(oldItem);
            }
        }

        // 새로운 아이템 장착
        model.EquipItem(slotType, item);

        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            Handle(prefab, slotType);
        }

        // 스탯 및 UI 갱신
        playerCombatStats?.RecalculateStats(model.Slots);
        RefreshEquipmentUI();
    }



    /// <summary>슬롯 타입 기준으로 캐릭터 Transform 반환</summary>
    private Transform GetSlotTransform(string slotType)
    {
        switch (slotType)
        {
            case "weapon": return FindChildByName(targetCharacter, "bone_HandR");
            case "shield": return FindChildByName(targetCharacter, "bone_HandL");
            case "head": return FindChildByName(targetCharacter, "bone_Head");
            case "lshoulder": return FindChildByName(targetCharacter, "bone_ShoulderL");
            case "rshoulder": return FindChildByName(targetCharacter, "bone_ShoulderR");
            default:
                Debug.LogWarning($"지원되지 않는 슬롯 타입: {slotType}");
                return null;
        }
    }

    /// <summary>재귀적으로 이름으로 Transform 찾기</summary>
    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>())
            if (t.name == name)
                return t;
        return null;
    }

    /// <summary>슬롯 장비 해제 처리</summary>
    public void HandleUnequipItem(string slotType)
    {
        var slot = model.GetSlot(slotType);
        if (slot == null || slot.equipped == null) return;

        InventoryItem unequippedItem = slot.equipped;

        // 모델에서 슬롯 해제
        model.UnequipItem(slotType);

        // 캐릭터 장비 제거
        Transform body = GetSlotTransform(slotType);
        if (body != null)
        {
            for (int i = body.childCount - 1; i >= 0; i--)
            {
                Transform child = body.GetChild(i);
                if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                    Destroy(child.gameObject);
            }
        }

        // 인벤토리에 동일한 아이템 추가
        if (inventoryPresenter != null)
        {
            inventoryPresenter.AddExistingItem(unequippedItem);
        }


        playerCombatStats?.RecalculateStats(model.Slots);
        RefreshEquipmentUI();
    }

    /// <summary>UI 갱신</summary>
    private void RefreshEquipmentUI()
    {
        if (view != null && model != null)
            view.UpdateEquipmentUI(model.Slots, HandleUnequipItem);
    }

    /// <summary>장비창 닫기</summary>
    private void CloseEquipment()
    {
        if (view == null) return;
        isOpen = false;
        view.Show(false);
    }
}
