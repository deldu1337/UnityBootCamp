using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 UI와 캐릭터 모델을 연결하고 장착/해제 및 스탯 계산을 담당하는 Presenter
/// </summary>
public class EquipmentPresenter : MonoBehaviour
{
    private EquipmentModel model;                // 장비 데이터 모델
    private EquipmentView view;                  // 장비 UI
    private InventoryPresenter inventoryPresenter; // 인벤토리 Presenter
    private bool isOpen = false;                 // 장비창 열림 상태

    [SerializeField] private Camera uiCamera;           // 장비 UI 전용 카메라
    [SerializeField] private Transform targetCharacter; // 캐릭터 모델
    [SerializeField] private PlayerStatsManager playerStats; // 캐릭터 스탯 계산용

    void Start()
    {
        model = new EquipmentModel();
        view = FindAnyObjectByType<EquipmentView>();
        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        playerStats = GetComponent<PlayerStatsManager>();

        // UI 초기화
        if (view != null)
            view.Initialize(CloseEquipment, HandleEquipFromInventory);

        // UI 카메라 자동 세팅
        SetupUICamera();

        // JSON에서 저장된 장비 불러와 캐릭터에 장착
        InitializeEquippedItemsFromJson();

        RefreshEquipmentUI();
    }

    public IReadOnlyList<EquipmentSlot> GetEquipmentSlots()
    {
        return model?.Slots; // EquipmentModel에서 슬롯 리스트 가져오기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquipment();
    }

    void LateUpdate()
    {
        // UI 카메라가 캐릭터를 바라보도록 유지
        if (uiCamera != null && targetCharacter != null)
        {
            Vector3 offset = targetCharacter.forward * 2.2f + Vector3.up * 1.5f;
            uiCamera.transform.position = targetCharacter.position + offset;
            uiCamera.transform.LookAt(targetCharacter.position + Vector3.up * 1.0f);
        }
    }

    /// <summary>UI 카메라 자동 세팅</summary>
    private void SetupUICamera()
    {
        if (uiCamera == null)
        {
            int uiLayer = LayerMask.NameToLayer("UICharacter");
            if (uiLayer != -1)
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
            }
        }
    }

    /// <summary>장비창 열기/닫기</summary>
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

    /// <summary>아이템 장착 (uniqueId 기반)</summary>
    public void HandleEquipItem(InventoryItem item)
    {
        string slotType = item.data.type;
        var slot = model.GetSlot(slotType);

        // 기존 장비를 인벤토리에 추가
        if (slot?.equipped != null)
        {
            inventoryPresenter?.AddExistingItem(slot.equipped);
        }

        // 장비 데이터 교체
        model.EquipItem(slotType, item);

        // 인벤토리에서 해당 아이템 제거
        inventoryPresenter?.RemoveItemFromInventory(item.uniqueId);

        // 캐릭터 모델 프리팹 장착
        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            var prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
                AttachPrefabToCharacter(prefab, slotType);
        }

        // UI & 스탯 갱신
        ApplyStatsAndSave();
        inventoryPresenter?.Refresh();
        RefreshEquipmentUI();
    }



    /// <summary>드래그앤드롭으로 장비 장착</summary>
    private void HandleEquipFromInventory(string slotType, InventoryItem item)
    {
        HandleEquipItem(item);
    }

    /// <summary>아이템 해제 (uniqueId 기반)</summary>
    public void HandleUnequipItem(string slotType)
    {
        var slot = model.GetSlot(slotType);
        if (slot?.equipped == null) return;

        var item = slot.equipped;

        // 인벤토리로 반환
        inventoryPresenter?.AddExistingItem(item);

        // 장비 데이터 업데이트
        model.UnequipItem(slotType);

        // 캐릭터 모델에서 프리팹 제거
        RemovePrefabFromCharacter(slotType);

        inventoryPresenter?.Refresh();
        ApplyStatsAndSave();
        RefreshEquipmentUI();
    }

    // 장비 변경 시 즉시 스탯 갱신 + 저장
    private void ApplyStatsAndSave()
    {
        playerStats?.RecalculateStats(model.Slots);
        SaveLoadManager.SavePlayerData(playerStats.Data);
    }

    /// <summary>JSON 저장된 장비 데이터 복원</summary>
    private void InitializeEquippedItemsFromJson()
    {
        foreach (var slot in model.Slots)
        {
            if (slot.equipped != null && !string.IsNullOrEmpty(slot.equipped.prefabPath))
            {
                GameObject prefab = Resources.Load<GameObject>(slot.equipped.prefabPath);
                if (prefab != null)
                    AttachPrefabToCharacter(prefab, slot.slotType);
            }
        }
        playerStats?.RecalculateStats(model.Slots);
    }

    /// <summary>장비 프리팹 캐릭터 본에 장착</summary>
    private void AttachPrefabToCharacter(GameObject prefab, string slotType)
    {
        Transform bone = GetSlotTransform(slotType);
        if (bone == null) return;

        // 기존 오브젝트 제거
        for (int i = bone.childCount - 1; i >= 0; i--)
        {
            Transform child = bone.GetChild(i);
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }

        // 새 프리팹 장착
        GameObject instance = Instantiate(prefab, bone);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = GetSlotOffset(slotType);
        instance.transform.localRotation = Quaternion.identity;
    }

    /// <summary>장비 프리팹 캐릭터 본에서 제거</summary>
    private void RemovePrefabFromCharacter(string slotType)
    {
        Transform bone = GetSlotTransform(slotType);
        if (bone == null) return;

        for (int i = bone.childCount - 1; i >= 0; i--)
        {
            Transform child = bone.GetChild(i);
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }
    }

    /// <summary>슬롯 본 찾기</summary>
    private Transform GetSlotTransform(string slotType)
    {
        string boneName = slotType switch
        {
            "weapon" => "bone_HandR",
            "shield" => "bone_HandL",
            "head" => "bone_Head",
            "lshoulder" => "bone_ShoulderL",
            "rshoulder" => "bone_ShoulderR",
            _ => null
        };

        if (boneName == null) return null;

        foreach (Transform t in targetCharacter.GetComponentsInChildren<Transform>())
            if (t.name == boneName)
                return t;

        Debug.LogWarning($"{slotType} 슬롯에 해당하는 본({boneName})을 찾을 수 없습니다.");
        return null;
    }

    /// <summary>슬롯별 오프셋</summary>
    private Vector3 GetSlotOffset(string slotType)
    {
        return slotType switch
        {
            "head" => new Vector3(-0.12f, 0.035f, 0),
            "shield" => new Vector3(0, 0, -0.05f),
            "lshoulder" => new Vector3(0, 0, -0.2f),
            "rshoulder" => new Vector3(0, 0, 0.2f),
            _ => Vector3.zero
        };
    }

    /// <summary>장비 UI 갱신</summary>
    private void RefreshEquipmentUI()
    {
        if (view != null && model != null)
            view.UpdateEquipmentUI(model.Slots, HandleUnequipItem);
    }

    /// <summary>장비창 닫기</summary>
    private void CloseEquipment()
    {
        view.Show(false);
        isOpen = false;
    }
}
