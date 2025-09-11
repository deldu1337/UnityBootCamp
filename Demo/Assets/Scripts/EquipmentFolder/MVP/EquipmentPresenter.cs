using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class EquipmentPresenter : MonoBehaviour
{
    private EquipmentModel model;
    private EquipmentView view;

    [SerializeField] private Camera uiCamera;
    [SerializeField] private Transform targetCharacter;
    [SerializeField] private PlayerCombatStats playerCombatStats;

    private bool isOpen;

    void Start()
    {
        model = new EquipmentModel();
        view = FindObjectOfType<EquipmentView>();
        if (view == null)
        {
            Debug.LogError("EquipmentView를 찾을 수 없습니다! 씬에 EquipmentView가 있는지 확인하세요.");
            return;
        }

        playerCombatStats = gameObject.GetComponent<PlayerCombatStats>();

        // UICharacter 레이어 카메라 자동 할당
        if (uiCamera == null)
        {
            int uiLayer = LayerMask.NameToLayer("UICharacter");
            if (uiLayer == -1)
            {
                Debug.LogError("UICharacter 레이어가 존재하지 않습니다! 레이어를 추가하세요.");
            }
            else
            {
                Camera[] cameras = GameObject.FindObjectsOfType<Camera>(true);
                foreach (Camera cam in cameras)
                {
                    if (cam.gameObject.layer == uiLayer)
                    {
                        uiCamera = cam;
                        break;
                    }
                }

                if (uiCamera == null)
                {
                    Debug.LogError("UICharacter 레이어를 가진 카메라를 찾을 수 없습니다! 카메라를 추가하세요.");
                }
            }
        }

        view.Initialize(CloseEquipment, HandleEquipFromInventory);
        InitializeEquippedItemsFromJson();
        RefreshEquipmentUI();
        isOpen = false;

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquipment();
    }

    void LateUpdate()
    {
        if (uiCamera != null && targetCharacter != null)
        {
            // 캐릭터 로컬 좌표 기준으로 앞(Forward) 방향에서 일정 거리 유지
            Vector3 offset = targetCharacter.forward * 2.2f + Vector3.up * 1.5f;
            uiCamera.transform.position = targetCharacter.position + offset;

            // 항상 캐릭터의 중심 바라보기
            uiCamera.transform.LookAt(targetCharacter.position + Vector3.up * 1.0f);
        }
    }

    private void HandleEquipFromInventory(string slotType, InventoryItem item)
    {
        Debug.Log($"인벤토리 아이템 {item.data.name} → {slotType} 장착 시도");

        // 모델에 반영
        model.EquipItem(slotType, item);

        // 프리팹 장착 처리
        if (!string.IsNullOrEmpty(item.prefabPath))
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
                Handle(prefab, slotType);
        }

        // 스탯 갱신
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);

        RefreshEquipmentUI();
    }

    private void ToggleEquipment()
    {
        if (view == null) return;

        isOpen = !isOpen;
        view.Show(isOpen);
        uiCamera.gameObject.SetActive(isOpen);

        if (isOpen)
            RefreshEquipmentUI();
    }

    private void Handle(GameObject prefab, string slot)
    {
        Transform body = null;
        Vector3 offset = Vector3.zero;

        // 슬롯 타입 기준으로 처리
        switch (slot)
        {
            case "weapon":
                foreach (var t in targetCharacter.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "bone_HandR")
                    {
                        body = t;
                        break;
                    }
                }
                if (body == null)
                {
                    Debug.LogWarning("캐릭터 오른손(bone_HandR)을 찾을 수 없음");
                    return;
                }
                break;

            case "head":
                foreach (var t in targetCharacter.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "bone_Head")
                    {
                        body = t;
                        break;
                    }
                }
                if (body == null)
                {
                    Debug.LogWarning("캐릭터 머리(bone_Head)를 찾을 수 없음");
                    return;
                }
                offset = new Vector3(-0.12f, 0.035f, 0);
                break;

            default:
                Debug.LogWarning($"지원되지 않는 슬롯 타입: {slot}");
                return;
        }

        // 기존 장비 제거
        for (int i = body.childCount - 1; i >= 0; i--)
        {
            Transform child = body.GetChild(i);
            // 이름이 숫자 또는 Clone 포함 여부 체크
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }

        // 새 장비 장착
        GameObject instance = Instantiate(prefab, body);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = offset;
        instance.transform.localRotation = Quaternion.identity;
    }


    /// <summary>
    /// JSON에 장착 데이터가 있는 슬롯만 캐릭터에 프리팹 장착
    /// </summary>
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
        // 장비 스탯 합산
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);
    }

    public void HandleEquipItem(InventoryItem item, int slotIndex)
    {
        Debug.Log($"장비 착용 처리: {slotIndex}번 슬롯 → {item.data.name}");
        Debug.Log(item.data.type);

        model.EquipItem(item.data.type, item);

        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            Handle(prefab, item.data.type);
        }
        // 장착 후
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);

        RefreshEquipmentUI();
    }


    public void HandleUnequipItem(string slotType)
    {
        model.UnequipItem(slotType);

        // 장착 후
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);

        // 해제 후
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);

        RefreshEquipmentUI();
    }

    private void RefreshEquipmentUI()
    {
        if (view != null && model != null)
        {
            view.UpdateEquipmentUI(model.Slots, HandleUnequipItem);
        }
    }

    private void CloseEquipment()
    {
        if (view == null) return;

        isOpen = false;
        view.Show(false);
    }
}
