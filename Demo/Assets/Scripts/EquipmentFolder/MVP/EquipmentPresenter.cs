using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

/// <summary>
/// ��� UI�� ĳ���� ���� �����ϰ� ����/���� �� ���� ����� ����ϴ� Presenter
/// MVC ���Ͽ��� Presenter ����
/// </summary>
public class EquipmentPresenter : MonoBehaviour
{
    // �𵨰� ��
    private EquipmentModel model;    // ��� ������ ��
    private EquipmentView view;      // UI ��
    private InventoryPresenter inventoryPresenter; // �κ��丮 Presenter

    // UI ī�޶� �� ĳ����
    [SerializeField] private Camera uiCamera;               // ��� UI ���� ī�޶�
    [SerializeField] private Transform targetCharacter;     // ��� ǥ���� ĳ���� Ʈ������
    [SerializeField] private PlayerCombatStats playerCombatStats; // ĳ���� ���� ����

    private bool isOpen; // ���â ���� ����

    /// <summary>
    /// �ʱ�ȭ: �� ����, �� ã��, UIī�޶� �ڵ� �Ҵ�, �ʱ� ���� ������ �ε�
    /// </summary>
    void Start()
    {
        model = new EquipmentModel();
        view = FindAnyObjectByType<EquipmentView>();
        if (view == null)
        {
            Debug.LogError("EquipmentView�� ã�� �� �����ϴ�! ���� EquipmentView�� �ִ��� Ȯ���ϼ���.");
            return;
        }

        // �κ��丮 Presenter �ڵ� �Ҵ�
        if (inventoryPresenter == null)
        {
            inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
            if (inventoryPresenter == null)
            {
                Debug.LogWarning("InventoryPresenter�� ã�� �� �����ϴ�. �κ��丮 ����� ���� �������� ���� �� �ֽ��ϴ�.");
            }
        }

        playerCombatStats = gameObject.GetComponent<PlayerCombatStats>();

        // UICharacter ���̾� ī�޶� �ڵ� �Ҵ�
        if (uiCamera == null)
        {
            int uiLayer = LayerMask.NameToLayer("UICharacter");
            if (uiLayer == -1)
                Debug.LogError("UICharacter ���̾ �������� �ʽ��ϴ�! ���̾ �߰��ϼ���.");
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
                    Debug.LogError("UICharacter ���̾ ���� ī�޶� ã�� �� �����ϴ�! ī�޶� �߰��ϼ���.");
            }
        }

        // �� �ʱ�ȭ (�ݱ� ��ư�� �巡�� �� ��� �ݹ� ����)
        view.Initialize(CloseEquipment, HandleEquipFromInventory);

        // JSON�� ����� ��� �ҷ��� ĳ���Ϳ� ����
        InitializeEquippedItemsFromJson();

        // UI ����
        RefreshEquipmentUI();
        isOpen = false;
    }

    /// <summary>���â ����/�ݱ� ����Ű ó��</summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquipment();
    }

    /// <summary>
    /// UI ī�޶� ĳ���� �������� �̵���Ű�� �׻� ĳ���� �ٶ󺸰� ��
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
    /// �κ��丮���� �巡���Ͽ� ���� �� ó��
    /// </summary>
    /// <param name="slotType">���� ���� Ÿ��</param>
    /// <param name="item">������ ������</param>

    private void HandleEquipFromInventory(string slotType, InventoryItem item)
    {
        Debug.Log($"�κ��丮 ������ {item.data.name} �� {slotType} ���� �õ�");

        var slot = model.GetSlot(slotType);

        // ���� �������� �κ��丮�� ��������
        if (slot != null && slot.equipped != null)
        {
            InventoryItem oldItem = slot.equipped;
            if (inventoryPresenter != null)
            {
                inventoryPresenter.AddExistingItem(oldItem);
            }
        }

        // ���ο� ������ ����
        model.EquipItem(slotType, item);

        if (!string.IsNullOrEmpty(item.prefabPath))
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
                Handle(prefab, slotType);
        }

        // ���� �� UI ����
        playerCombatStats?.RecalculateStats(model.Slots);
        RefreshEquipmentUI();
    }


    /// <summary>���â ����/�ݱ� ���</summary>
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
    /// ĳ���� ���Կ� ������ ���� ó��
    /// </summary>
    /// <param name="prefab">������ ������ ������</param>
    /// <param name="slot">���� Ÿ��</param>
    private void Handle(GameObject prefab, string slot)
    {
        Transform body = null;
        Vector3 offset = Vector3.zero;

        // ���� Ÿ�Կ� ���� �� ã�� �� ������ ����
        switch (slot)
        {
            case "weapon":
                body = FindChildByName(targetCharacter, "bone_HandR");
                if (body == null) { Debug.LogWarning("ĳ���� ������(bone_HandR)�� ã�� �� ����"); return; }
                break;

            case "head":
                body = FindChildByName(targetCharacter, "bone_Head");
                if (body == null) { Debug.LogWarning("ĳ���� �Ӹ�(bone_Head)�� ã�� �� ����"); return; }
                offset = new Vector3(-0.12f, 0.035f, 0);
                break;

            case "shield":
                body = FindChildByName(targetCharacter, "bone_HandL");
                if (body == null) { Debug.LogWarning("ĳ���� �޼�(bone_HandL)�� ã�� �� ����"); return; }
                offset = new Vector3(0, 0, -0.05f);
                break;

            case "lshoulder":
                body = FindChildByName(targetCharacter, "bone_ShoulderL");
                if (body == null) { Debug.LogWarning("ĳ���� ���� ���(bone_ShoulderL)�� ã�� �� ����"); return; }
                offset = new Vector3(0, 0, -0.2f);
                break;

            case "rshoulder":
                body = FindChildByName(targetCharacter, "bone_ShoulderR");
                if (body == null) { Debug.LogWarning("ĳ���� ������ ���(bone_ShoulderR)�� ã�� �� ����"); return; }
                offset = new Vector3(0, 0, 0.2f);
                break;

            default:
                Debug.LogWarning($"�������� �ʴ� ���� Ÿ��: {slot}");
                return;
        }

        // ���� ��� ����
        for (int i = body.childCount - 1; i >= 0; i--)
        {
            Transform child = body.GetChild(i);
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }

        // �� ��� ����
        GameObject instance = Instantiate(prefab, body);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = offset;
        instance.transform.localRotation = Quaternion.identity;
    }

    /// <summary>JSON�� ����� ��� �����͸� ������� ĳ���Ϳ� ����</summary>
    private void InitializeEquippedItemsFromJson()
    {
        foreach (var slot in model.Slots)
        {
            if (slot.equipped != null && !string.IsNullOrEmpty(slot.equipped.prefabPath))
            {
                GameObject prefab = Resources.Load<GameObject>(slot.equipped.prefabPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"�������� ã�� �� ����: {slot.equipped.prefabPath}");
                    continue;
                }
                Handle(prefab, slot.slotType);
            }
        }
        playerCombatStats?.RecalculateStats(model.Slots);
    }

    /// <summary>�ε��� �Ǵ� �������� ���� ó��</summary>
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

        // ���� ��� ������ �κ��丮�� �ǵ�����
        if (slot != null && slot.equipped != null)
        {
            InventoryItem oldItem = slot.equipped;
            if (inventoryPresenter != null)
            {
                inventoryPresenter.AddExistingItem(oldItem);
            }
        }

        // ���ο� ������ ����
        model.EquipItem(slotType, item);

        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            Handle(prefab, slotType);
        }

        // ���� �� UI ����
        playerCombatStats?.RecalculateStats(model.Slots);
        RefreshEquipmentUI();
    }



    /// <summary>���� Ÿ�� �������� ĳ���� Transform ��ȯ</summary>
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
                Debug.LogWarning($"�������� �ʴ� ���� Ÿ��: {slotType}");
                return null;
        }
    }

    /// <summary>��������� �̸����� Transform ã��</summary>
    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>())
            if (t.name == name)
                return t;
        return null;
    }

    /// <summary>���� ��� ���� ó��</summary>
    public void HandleUnequipItem(string slotType)
    {
        var slot = model.GetSlot(slotType);
        if (slot == null || slot.equipped == null) return;

        InventoryItem unequippedItem = slot.equipped;

        // �𵨿��� ���� ����
        model.UnequipItem(slotType);

        // ĳ���� ��� ����
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

        // �κ��丮�� ������ ������ �߰�
        if (inventoryPresenter != null)
        {
            inventoryPresenter.AddExistingItem(unequippedItem);
        }


        playerCombatStats?.RecalculateStats(model.Slots);
        RefreshEquipmentUI();
    }

    /// <summary>UI ����</summary>
    private void RefreshEquipmentUI()
    {
        if (view != null && model != null)
            view.UpdateEquipmentUI(model.Slots, HandleUnequipItem);
    }

    /// <summary>���â �ݱ�</summary>
    private void CloseEquipment()
    {
        if (view == null) return;
        isOpen = false;
        view.Show(false);
    }
}
