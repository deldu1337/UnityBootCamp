using UnityEngine;

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
        isOpen = false;
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquipment();
    }

    private void ToggleEquipment()
    {
        if (view == null) return;

        isOpen = !isOpen;
        view.Show(isOpen);
        uiCamera.gameObject.SetActive(isOpen);
    }

    private void CloseEquipment()
    {
        if (view == null) return;

        isOpen = false;
        view.Show(false);
    }
}
