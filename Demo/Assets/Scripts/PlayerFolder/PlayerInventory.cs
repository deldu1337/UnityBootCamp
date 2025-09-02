using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Panel ��ü ���� ��õ
    private bool isOpen;

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        isOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen; // ���� ���
            if (inventoryPanel != null)
                inventoryPanel.SetActive(isOpen);
        }
    }
}