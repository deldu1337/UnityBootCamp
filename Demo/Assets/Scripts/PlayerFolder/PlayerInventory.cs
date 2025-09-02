using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // Panel 전체 연결 추천
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
            isOpen = !isOpen; // 상태 토글
            if (inventoryPanel != null)
                inventoryPanel.SetActive(isOpen);
        }
    }
}