﻿using System.Collections;
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

    // 현재 플레이어 종족(카메라 간격 스위치에 사용)
    private string currentRace = "humanmale";

    void Start()
    {
        UIEscapeStack.GetOrCreate();

        // 종족 구해서 모델 생성 + currentRace 저장
        var ps = PlayerStatsManager.Instance;
        currentRace = (ps != null && ps.Data != null && !string.IsNullOrEmpty(ps.Data.Race))
                        ? ps.Data.Race
                        : "humanmale";

        model = new EquipmentModel(currentRace);

        view = FindAnyObjectByType<EquipmentView>();
        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();

        if (view != null)
            view.Initialize(CloseEquipment, HandleEquipFromInventory);

        SetupUICamera();
        InitializeEquippedItems();
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

        // 여기서 ESC로 닫지 않는다 (중앙 ESC 스택에서 처리)
        // if (Input.GetKeyDown(KeyCode.Escape)) CloseEquipment();
    }

    void LateUpdate()
    {
        // UI 카메라가 캐릭터를 바라보도록 유지
        if (uiCamera != null && targetCharacter != null)
        {
            // 종족별 전/후 거리만 스위치로 조절
            float dist = GetRaceCameraDistance(currentRace);
            float height = GetRaceLookAtHeight(currentRace);
            float lookY = GetRaceCameraHeight(currentRace);

            Vector3 offset = targetCharacter.forward * dist + Vector3.up * height; // 높이는 그대로 유지
            //Vector3 offset = targetCharacter.forward * 2.2f + Vector3.up * 1.5f;
            uiCamera.transform.position = targetCharacter.position + offset;
            uiCamera.transform.LookAt(targetCharacter.position + Vector3.up * lookY);
        }
    }

    /// <summary>종족별 카메라 전/후 거리</summary>
    private float GetRaceCameraDistance(string race)
    {
        // 비교 편의를 위해 소문자 통일
        string r = string.IsNullOrEmpty(race) ? "humanmale" : race.ToLowerInvariant();

        switch (r)
        {
            case "humanmale": return 2.2f;
            case "dwarfmale": return 2.0f;
            case "gnomemale": return 1.2f;
            case "nightelfmale": return 2.7f;
            case "orcmale": return 2.5f;
            case "trollmale": return 2.8f;
            case "goblinmale": return 1.7f;
            case "scourgefemale": return 2.0f;
            default: return 2.2f;
        }
    }

    /// <summary>종족별 시선(LookAt) 높이 — 필요 시 미세조정</summary>
    private float GetRaceLookAtHeight(string race)
    {
        string r = string.IsNullOrEmpty(race) ? "humanmale" : race.ToLowerInvariant();
        switch (r)
        {
            case "humanmale": return 1.4f;
            case "dwarfmale": return 1.3f;
            case "gnomemale": return 0.7f;
            case "nightelfmale": return 1.6f;
            case "orcmale": return 1.4f;
            case "trollmale": return 1.5f;
            case "goblinmale": return 1.0f;
            case "scourgefemale": return 1.2f;
            default: return 1.4f;
        }
    }

    /// <summary>종족별 카메라 높이</summary>
    private float GetRaceCameraHeight(string race)
    {
        string r = string.IsNullOrEmpty(race) ? "humanmale" : race.ToLowerInvariant();
        switch (r)
        {
            case "humanmale": return 1.0f;
            case "dwarfmale": return 0.7f;
            case "gnomemale": return 0.5f;
            case "nightelfmale": return 1.3f;
            case "orcmale": return 1.1f;
            case "trollmale": return 1.3f;
            case "goblinmale": return 0.6f;
            case "scourgefemale": return 0.9f;
            default: return 1.0f;
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
        if (uiCamera != null) uiCamera.gameObject.SetActive(isOpen);

        if (isOpen)
        {
            RefreshEquipmentUI();
            UIEscapeStack.Instance.Push(
                key: "equipment",
                close: CloseEquipment,
                isOpen: () => isOpen
            );
        }
        else
        {
            UIEscapeStack.Instance.Remove("equipment");
        }
    }

    private void CloseEquipment()
    {
        if (!isOpen) return;
        view.Show(false);
        if (uiCamera != null) uiCamera.gameObject.SetActive(false);
        isOpen = false;
        UIEscapeStack.Instance.Remove("equipment");
    }

    private int GetPlayerLevel()
    {
        var ps = PlayerStatsManager.Instance;
        return (ps != null && ps.Data != null) ? ps.Data.Level : 1;
    }

    /// <summary>아이템 장착 (uniqueId 기반)</summary>
    public void HandleEquipItem(InventoryItem item)
    {
        // === 레벨 제한 체크 ===
        int reqLevel = Mathf.Max(1, item.data.level);
        int curLevel = GetPlayerLevel();
        if (curLevel < reqLevel)
        {
            Debug.LogWarning($"[장착 실패] 요구 레벨 {reqLevel}, 현재 레벨 {curLevel} → '{item.data.name}' 장착 불가");
            // 필요하면 여기서 UI 토스트/사운드/버튼 흔들기 등 피드백 호출
            return;
        }

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
        var ps = PlayerStatsManager.Instance;
        if (ps != null)
        {
            ps.RecalculateStats(model.Slots);
            SaveLoadService.SavePlayerDataForRace(ps.Data.Race, ps.Data);
        }
    }

    /// <summary>JSON 저장된 장비 데이터 복원</summary>
    private void InitializeEquippedItems()
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
        var ps = PlayerStatsManager.Instance;
        if (ps != null) ps.RecalculateStats(model.Slots);
    }

    /// <summary>장비 프리팹 캐릭터 본에 장착</summary>
    private void AttachPrefabToCharacter(GameObject prefab, string slotType)
    {
        Transform bone = GetSlotTransform(slotType);
        if (bone == null) return;

        if (bone.childCount > 0)
        {
            Transform lastChild = bone.GetChild(bone.childCount - 1);
            Destroy(lastChild.gameObject);
        }

        // 새 프리팹 장착
        GameObject instance = Instantiate(prefab, bone);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = GetSlotOffset(slotType);
        instance.transform.localRotation = Quaternion.identity;

        // ── 여기부터 추가: 장비로 붙을 땐 줍기/툴팁/물리 비활성화 ──
        // 1) 장착 마커 부착
        if (instance.GetComponent<EquippedMarker>() == null)
            instance.AddComponent<EquippedMarker>();

        // 2) 근접 줍기/툴팁 스크립트 제거
        foreach (var pickup in instance.GetComponentsInChildren<ItemPickup>(true))
            Destroy(pickup);

        // (혹시 붙어있을 수 있는) 월드 툴팁 트리거류도 제거하고 싶다면:
        foreach (var hover in instance.GetComponentsInChildren<ItemHoverTooltip>(true))
            Destroy(hover);

        // 3) 물리 충돌/중력 제거 (캐릭터 본에 붙었을 땐 불필요)
        foreach (var col in instance.GetComponentsInChildren<Collider>(true))
            col.enabled = false;
        foreach (var rb in instance.GetComponentsInChildren<Rigidbody>(true))
            Destroy(rb);

        // 4) (선택) 레이어 분리해서 다른 시스템에서 무시되게
        // int equipLayer = LayerMask.NameToLayer("UICharacter");
        // if (equipLayer != -1) SetLayerRecursively(instance, equipLayer);
    }

    /// <summary>장비 프리팹 캐릭터 본에서 제거</summary>
    private void RemovePrefabFromCharacter(string slotType)
    {
        Transform bone = GetSlotTransform(slotType);
        if (bone == null) return;

        if (bone.childCount > 0)
        {
            Transform lastChild = bone.GetChild(bone.childCount - 1);
            Destroy(lastChild.gameObject);
        }
    }

    /// <summary>슬롯 본 찾기</summary>
    private Transform GetSlotTransform(string slotType)
    {
        string boneName = slotType switch
        {
            "weapon" => "HandR",
            "shield" => "HandL",
            "head" => "Head",
            "lshoulder" => "ShoulderL",
            "rshoulder" => "ShoulderR",
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
}
