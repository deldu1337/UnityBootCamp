using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private Button exitButton;

    public void Initialize(Action onExit)
    {
        if (equipmentUI == null)
            equipmentUI = GameObject.Find("EquipmentUI");

        if (equipmentUI != null)
        {
            // ExitButton 찾기
            exitButton = equipmentUI.GetComponentInChildren<Button>();
            Debug.Log(exitButton.name);

            // 버튼 클릭 이벤트 등록
            exitButton.onClick.AddListener(() => onExit?.Invoke());
        }

        Show(false);
    }

    public void Show(bool show)
    {
        equipmentUI?.SetActive(show);
        if (exitButton != null)
            exitButton.transform.SetAsLastSibling();
    }
}
