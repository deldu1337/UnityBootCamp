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
    [SerializeField] private PlayerStatsManager playerStats; // ĳ���� ���� ����

    void Start()
    {
        model = new EquipmentModel();
        view = FindAnyObjectByType<EquipmentView>();
        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        playerStats = GetComponent<PlayerStatsManager>();

        // UI �ʱ�ȭ
        if (view != null)
            view.Initialize(CloseEquipment, HandleEquipFromInventory);

        // UI ī�޶� �ڵ� ����
        SetupUICamera();

        // JSON���� ����� ��� �ҷ��� ĳ���Ϳ� ����
        InitializeEquippedItemsFromJson();

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

    /// <summary>������ ���� (uniqueId ���)</summary>
    public void HandleEquipItem(InventoryItem item)
    {
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
        playerStats?.RecalculateStats(model.Slots);
        SaveLoadManager.SavePlayerData(playerStats.Data);
    }

    /// <summary>JSON ����� ��� ������ ����</summary>
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

    /// <summary>��� ������ ĳ���� ���� ����</summary>
    private void AttachPrefabToCharacter(GameObject prefab, string slotType)
    {
        Transform bone = GetSlotTransform(slotType);
        if (bone == null) return;

        // ���� ������Ʈ ����
        for (int i = bone.childCount - 1; i >= 0; i--)
        {
            Transform child = bone.GetChild(i);
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }

        // �� ������ ����
        GameObject instance = Instantiate(prefab, bone);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = GetSlotOffset(slotType);
        instance.transform.localRotation = Quaternion.identity;
    }

    /// <summary>��� ������ ĳ���� ������ ����</summary>
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

    /// <summary>���� �� ã��</summary>
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
