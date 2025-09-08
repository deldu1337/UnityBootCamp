using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI; // �κ��丮 UI Panel

    private Button ExitButton;

    private bool isOpen;                                // �κ��丮 ���� ����

    void Start()
    {
        // Panel�� �Ҵ���� �ʾҴٸ� ������ �˻�
        if (equipmentUI == null)
            equipmentUI = GameObject.Find("EquipmentUI");

        if (equipmentUI != null)
        {
            // ExitButton ã��
            ExitButton = equipmentUI.GetComponentInChildren<Button>();

            // ��ư Ŭ�� �̺�Ʈ ���
            ExitButton.onClick.AddListener(() =>
            {
                CloseEquipment();
            });

            equipmentUI.SetActive(false); // ó���� ��Ȱ��ȭ
        }

        isOpen = false;
    }

    void Update()
    {
        // I Ű �Է� �� �κ��丮 ���
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
            Debug.Log("���â ����");
        }
        else
        {
            Debug.LogWarning("equipmentUI�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}