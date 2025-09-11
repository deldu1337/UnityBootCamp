using UnityEngine;
using UnityEngine.UI;

public class EquipmentPresenter : MonoBehaviour
{
    private EquipmentModel model;
    private EquipmentView view;

    [SerializeField] private Camera uiCamera;
    [SerializeField] private Transform targetCharacter;

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

        view.Initialize(CloseEquipment);
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

    private void ToggleEquipment()
    {
        if (view == null) return;

        isOpen = !isOpen;
        view.Show(isOpen);
        uiCamera.gameObject.SetActive(isOpen);

        if (isOpen)
            RefreshEquipmentUI();
    }

    public void HandleEquipItem(InventoryItem item, int slotIndex)
    {
        Debug.Log($"��� ���� ó��: {slotIndex}�� ���� �� {item.data.name}");

        model.EquipItem(item.data.type, item);

        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
            {
                // ������ ã��
                Transform hand = null;
                foreach (var t in targetCharacter.GetComponentsInChildren<Transform>())
                {
                    if (t.name == "bone_HandR")
                    {
                        hand = t;
                        break;
                    }
                }

                if (hand == null)
                {
                    Debug.LogWarning("ĳ���� ������(bone_HandR)�� ã�� �� ����");
                    return;
                }
                if (hand != null)
                {
                    // ���� ���� ����: ���� ���⸸ �����ϴ� ���, ��� ���� ���� ����
                    for (int i = hand.childCount - 1; i >= 0; i--)
                    {
                        Transform child = hand.GetChild(i);
                        // �̸��� ���� �Ǵ� Clone ���� ���� üũ
                        if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                            Destroy(child.gameObject);
                    }

                    // �� ���� ���� (������ ��ġ��)
                    GameObject weaponInstance = Instantiate(prefab, hand);
                    weaponInstance.transform.SetAsLastSibling(); // ���� ������ �ڽ�����
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                Debug.LogWarning($"�������� ã�� �� ����: {item.prefabPath}");
            }
        }

        RefreshEquipmentUI();
    }



    public void HandleUnequipItem(string slotType)
    {
        model.UnequipItem(slotType);
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
