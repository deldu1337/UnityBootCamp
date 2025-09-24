using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� UI�� ĳ���� ���� �����ϰ� ����/���� �� ���� ����� ����ϴ� Presenter
/// </summary>
public class EquipmentPresenter : MonoBehaviour
{
    private EquipmentModel model;                // ��� ������ ��
    private EquipmentView view;                  // ��� UI
    private InventoryPresenter inventoryPresenter; // �κ��丮 Presenter
    private bool isOpen = false;                 // ���â ���� ����

    [SerializeField] private Camera uiCamera;           // ��� UI ���� ī�޶�
    [SerializeField] private Transform targetCharacter; // ĳ���� ��
    //[SerializeField] private PlayerStatsManager playerStats; // ĳ���� ���� ����

    void Start()
    {
        model = new EquipmentModel();
        view = FindAnyObjectByType<EquipmentView>();
        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        //playerStats = GetComponent<PlayerStatsManager>();

        // UI �ʱ�ȭ
        if (view != null)
            view.Initialize(CloseEquipment, HandleEquipFromInventory);

        // UI ī�޶� �ڵ� ����
        SetupUICamera();

        // ����� ��� �ҷ��� ĳ���Ϳ� ����
        InitializeEquippedItems();

        RefreshEquipmentUI();
    }

    public IReadOnlyList<EquipmentSlot> GetEquipmentSlots()
    {
        return model?.Slots; // EquipmentModel���� ���� ����Ʈ ��������
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquipment();
        if(Input.GetKeyDown(KeyCode.Escape))
            CloseEquipment();
    }

    void LateUpdate()
    {
        // UI ī�޶� ĳ���͸� �ٶ󺸵��� ����
        if (uiCamera != null && targetCharacter != null)
        {
            Vector3 offset = targetCharacter.forward * 2.2f + Vector3.up * 1.5f;
            uiCamera.transform.position = targetCharacter.position + offset;
            uiCamera.transform.LookAt(targetCharacter.position + Vector3.up * 1.0f);
        }
    }

    /// <summary>UI ī�޶� �ڵ� ����</summary>
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

    /// <summary>���â ����/�ݱ�</summary>
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
    private int GetPlayerLevel()
    {
        var ps = PlayerStatsManager.Instance;
        return (ps != null && ps.Data != null) ? ps.Data.Level : 1;
    }

    /// <summary>������ ���� (uniqueId ���)</summary>
    public void HandleEquipItem(InventoryItem item)
    {
        // === ���� ���� üũ ===
        int reqLevel = Mathf.Max(1, item.data.level);
        int curLevel = GetPlayerLevel();
        if (curLevel < reqLevel)
        {
            Debug.LogWarning($"[���� ����] �䱸 ���� {reqLevel}, ���� ���� {curLevel} �� '{item.data.name}' ���� �Ұ�");
            // �ʿ��ϸ� ���⼭ UI �佺Ʈ/����/��ư ���� �� �ǵ�� ȣ��
            return;
        }

        string slotType = item.data.type;
        var slot = model.GetSlot(slotType);

        // ���� ��� �κ��丮�� �߰�
        if (slot?.equipped != null)
        {
            inventoryPresenter?.AddExistingItem(slot.equipped);
        }

        // ��� ������ ��ü
        model.EquipItem(slotType, item);

        // �κ��丮���� �ش� ������ ����
        inventoryPresenter?.RemoveItemFromInventory(item.uniqueId);

        // ĳ���� �� ������ ����
        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            var prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
                AttachPrefabToCharacter(prefab, slotType);
        }

        // UI & ���� ����
        ApplyStatsAndSave();
        inventoryPresenter?.Refresh();
        RefreshEquipmentUI();
    }

    /// <summary>�巡�׾ص������ ��� ����</summary>
    private void HandleEquipFromInventory(string slotType, InventoryItem item)
    {
        HandleEquipItem(item);
    }

    /// <summary>������ ���� (uniqueId ���)</summary>
    public void HandleUnequipItem(string slotType)
    {
        var slot = model.GetSlot(slotType);
        if (slot?.equipped == null) return;

        var item = slot.equipped;

        // �κ��丮�� ��ȯ
        inventoryPresenter?.AddExistingItem(item);

        // ��� ������ ������Ʈ
        model.UnequipItem(slotType);

        // ĳ���� �𵨿��� ������ ����
        RemovePrefabFromCharacter(slotType);

        inventoryPresenter?.Refresh();
        ApplyStatsAndSave();
        RefreshEquipmentUI();
    }

    // ��� ���� �� ��� ���� ���� + ����
    private void ApplyStatsAndSave()
    {
        var ps = PlayerStatsManager.Instance;
        if (ps != null)
        {
            ps.RecalculateStats(model.Slots);
            SaveLoadService.SavePlayerData(ps.Data);
        }
    }

    /// <summary>JSON ����� ��� ������ ����</summary>
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

    /// <summary>��� ������ ĳ���� ���� ����</summary>
    private void AttachPrefabToCharacter(GameObject prefab, string slotType)
    {
        Transform bone = GetSlotTransform(slotType);
        if (bone == null) return;

        if (bone.childCount > 0)
        {
            Transform lastChild = bone.GetChild(bone.childCount - 1);
            Destroy(lastChild.gameObject);
        }

        // �� ������ ����
        GameObject instance = Instantiate(prefab, bone);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = GetSlotOffset(slotType);
        instance.transform.localRotation = Quaternion.identity;

        // ���� ������� �߰�: ���� ���� �� �ݱ�/����/���� ��Ȱ��ȭ ����
        // 1) ���� ��Ŀ ����
        if (instance.GetComponent<EquippedMarker>() == null)
            instance.AddComponent<EquippedMarker>();

        // 2) ���� �ݱ�/���� ��ũ��Ʈ ����
        foreach (var pickup in instance.GetComponentsInChildren<ItemPickup>(true))
            Destroy(pickup);

        // (Ȥ�� �پ����� �� �ִ�) ���� ���� Ʈ���ŷ��� �����ϰ� �ʹٸ�:
        foreach (var hover in instance.GetComponentsInChildren<ItemHoverTooltip>(true))
            Destroy(hover);

        // 3) ���� �浹/�߷� ���� (ĳ���� ���� �پ��� �� ���ʿ�)
        foreach (var col in instance.GetComponentsInChildren<Collider>(true))
            col.enabled = false;
        foreach (var rb in instance.GetComponentsInChildren<Rigidbody>(true))
            Destroy(rb);

        // 4) (����) ���̾� �и��ؼ� �ٸ� �ý��ۿ��� ���õǰ�
        // int equipLayer = LayerMask.NameToLayer("UICharacter");
        // if (equipLayer != -1) SetLayerRecursively(instance, equipLayer);
    }

    /// <summary>��� ������ ĳ���� ������ ����</summary>
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

    /// <summary>���� �� ã��</summary>
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

        Debug.LogWarning($"{slotType} ���Կ� �ش��ϴ� ��({boneName})�� ã�� �� �����ϴ�.");
        return null;
    }

    /// <summary>���Ժ� ������</summary>
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

    /// <summary>��� UI ����</summary>
    private void RefreshEquipmentUI()
    {
        if (view != null && model != null)
            view.UpdateEquipmentUI(model.Slots, HandleUnequipItem);
    }

    /// <summary>���â �ݱ�</summary>
    private void CloseEquipment()
    {
        view.Show(false);
        isOpen = false;
    }
}
