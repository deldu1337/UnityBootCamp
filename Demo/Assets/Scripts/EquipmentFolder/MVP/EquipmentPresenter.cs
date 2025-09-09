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
            Debug.LogError("EquipmentView를 찾을 수 없습니다! 씬에 EquipmentView가 있는지 확인하세요.");
            return;
        }

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

        view.Initialize(CloseEquipment);
        isOpen = false;
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
