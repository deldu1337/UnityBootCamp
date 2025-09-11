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
            Debug.LogError("EquipmentView�� ã�� �� �����ϴ�! ���� EquipmentView�� �ִ��� Ȯ���ϼ���.");
            return;
        }

        playerCombatStats = gameObject.GetComponent<PlayerCombatStats>();

        // UICharacter ���̾� ī�޶� �ڵ� �Ҵ�
        if (uiCamera == null)
        {
            int uiLayer = LayerMask.NameToLayer("UICharacter");
            if (uiLayer == -1)
            {
                Debug.LogError("UICharacter ���̾ �������� �ʽ��ϴ�! ���̾ �߰��ϼ���.");
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
                    Debug.LogError("UICharacter ���̾ ���� ī�޶� ã�� �� �����ϴ�! ī�޶� �߰��ϼ���.");
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
            // ĳ���� ���� ��ǥ �������� ��(Forward) ���⿡�� ���� �Ÿ� ����
            Vector3 offset = targetCharacter.forward * 2.2f + Vector3.up * 1.5f;
            uiCamera.transform.position = targetCharacter.position + offset;

            // �׻� ĳ������ �߽� �ٶ󺸱�
            uiCamera.transform.LookAt(targetCharacter.position + Vector3.up * 1.0f);
        }
    }

    private void HandleEquipFromInventory(string slotType, InventoryItem item)
    {
        Debug.Log($"�κ��丮 ������ {item.data.name} �� {slotType} ���� �õ�");

        // �𵨿� �ݿ�
        model.EquipItem(slotType, item);

        // ������ ���� ó��
        if (!string.IsNullOrEmpty(item.prefabPath))
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
                Handle(prefab, slotType);
        }

        // ���� ����
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

        // ���� Ÿ�� �������� ó��
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
                    Debug.LogWarning("ĳ���� ������(bone_HandR)�� ã�� �� ����");
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
                    Debug.LogWarning("ĳ���� �Ӹ�(bone_Head)�� ã�� �� ����");
                    return;
                }
                offset = new Vector3(-0.12f, 0.035f, 0);
                break;

            default:
                Debug.LogWarning($"�������� �ʴ� ���� Ÿ��: {slot}");
                return;
        }

        // ���� ��� ����
        for (int i = body.childCount - 1; i >= 0; i--)
        {
            Transform child = body.GetChild(i);
            // �̸��� ���� �Ǵ� Clone ���� ���� üũ
            if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                Destroy(child.gameObject);
        }

        // �� ��� ����
        GameObject instance = Instantiate(prefab, body);
        instance.transform.SetAsLastSibling();
        instance.transform.localPosition = offset;
        instance.transform.localRotation = Quaternion.identity;
    }


    /// <summary>
    /// JSON�� ���� �����Ͱ� �ִ� ���Ը� ĳ���Ϳ� ������ ����
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
                    Debug.LogWarning($"�������� ã�� �� ����: {slot.equipped.prefabPath}");
                    continue;
                }
                Handle(prefab, slot.slotType);
            }
        }
        // ��� ���� �ջ�
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);
    }

    public void HandleEquipItem(InventoryItem item, int slotIndex)
    {
        Debug.Log($"��� ���� ó��: {slotIndex}�� ���� �� {item.data.name}");
        Debug.Log(item.data.type);

        model.EquipItem(item.data.type, item);

        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            Handle(prefab, item.data.type);
        }
        // ���� ��
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);

        RefreshEquipmentUI();
    }


    public void HandleUnequipItem(string slotType)
    {
        model.UnequipItem(slotType);

        // ���� ��
        if (playerCombatStats != null)
            playerCombatStats.RecalculateStats(model.Slots);

        // ���� ��
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
