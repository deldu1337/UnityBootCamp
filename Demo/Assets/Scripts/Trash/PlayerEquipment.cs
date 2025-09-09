using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI; // 인벤토리 UI Panel

    private Button ExitButton;

    private bool isOpen;                                // 인벤토리 열림 상태

    void Start()
    {
        // Panel이 할당되지 않았다면 씬에서 검색
        if (equipmentUI == null)
            equipmentUI = GameObject.Find("EquipmentUI");

        if (equipmentUI != null)
        {
            // ExitButton 찾기
            ExitButton = equipmentUI.GetComponentInChildren<Button>();

            // 버튼 클릭 이벤트 등록
            ExitButton.onClick.AddListener(() =>
            {
                CloseEquipment();
            });

            equipmentUI.SetActive(false); // 처음엔 비활성화
        }

        isOpen = false;
    }

    void Update()
    {
        // I 키 입력 시 인벤토리 토글
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            if (equipmentUI != null)
            {
                equipmentUI.SetActive(isOpen);
            }
        }
    }

    public void CloseEquipment()
    {
        if (equipmentUI != null)
        {
            equipmentUI.SetActive(false);
            isOpen = false;
            Debug.Log("장비창 닫힘");
        }
        else
        {
            Debug.LogWarning("equipmentUI가 할당되지 않았습니다.");
        }
    }
}