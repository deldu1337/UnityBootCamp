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
        Debug.Log($"장비 착용 처리: {slotIndex}번 슬롯 → {item.data.name}");

        model.EquipItem(item.data.type, item);

        if (!string.IsNullOrEmpty(item.prefabPath) && targetCharacter != null)
        {
            GameObject prefab = Resources.Load<GameObject>(item.prefabPath);
            if (prefab != null)
            {
                // 오른손 찾기
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
                    Debug.LogWarning("캐릭터 오른손(bone_HandR)을 찾을 수 없음");
                    return;
                }
                if (hand != null)
                {
                    // 기존 무기 제거: 숫자 무기만 삭제하는 대신, 모든 기존 무기 제거
                    for (int i = hand.childCount - 1; i >= 0; i--)
                    {
                        Transform child = hand.GetChild(i);
                        // 이름이 숫자 또는 Clone 포함 여부 체크
                        if (int.TryParse(child.name.Replace("(Clone)", ""), out _))
                            Destroy(child.gameObject);
                    }

                    // 새 무기 장착 (마지막 위치에)
                    GameObject weaponInstance = Instantiate(prefab, hand);
                    weaponInstance.transform.SetAsLastSibling(); // 가장 마지막 자식으로
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                Debug.LogWarning($"프리팹을 찾을 수 없음: {item.prefabPath}");
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
