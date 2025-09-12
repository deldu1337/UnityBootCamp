using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI; // 장비 UI 전체
    [SerializeField] private Button exitButton;      // 장비창 닫기 버튼

    [Header("장비 슬롯")]
    [SerializeField] private Button headSlot;
    [SerializeField] private Button rShoulderSlot;
    [SerializeField] private Button lShoulderSlot;
    [SerializeField] private Button gemSlot;
    [SerializeField] private Button weaponSlot;
    [SerializeField] private Button shieldSlot;

    /// <summary>초기화: 슬롯 연결, 종료 버튼 연결</summary>
    public void Initialize(Action onExit, Action<string, InventoryItem> onEquipDropped)
    {
        if (equipmentUI == null)
            equipmentUI = GameObject.Find("EquipmentUI");

        if (equipmentUI != null)
        {
            exitButton = equipmentUI.GetComponentInChildren<Button>();
            exitButton.onClick.AddListener(() => onExit?.Invoke());
        }

        // ButtonPanel 내 버튼들 순서대로 가져오기
        headSlot = GameObject.Find("ButtonPanel").transform.GetChild(0).GetComponentInChildren<Button>();
        rShoulderSlot = GameObject.Find("ButtonPanel").transform.GetChild(1).GetComponentInChildren<Button>();
        lShoulderSlot = GameObject.Find("ButtonPanel").transform.GetChild(2).GetComponentInChildren<Button>();
        gemSlot = GameObject.Find("ButtonPanel").transform.GetChild(3).GetComponentInChildren<Button>();
        weaponSlot = GameObject.Find("ButtonPanel").transform.GetChild(4).GetComponentInChildren<Button>();
        shieldSlot = GameObject.Find("ButtonPanel").transform.GetChild(5).GetComponentInChildren<Button>();

        Show(false);
    }

    /// <summary>장비창 활성/비활성</summary>
    public void Show(bool show)
    {
        equipmentUI?.SetActive(show);
        if (exitButton != null)
            exitButton.transform.SetAsLastSibling();
    }

    /// <summary>슬롯 버튼에 아이템 드롭 이벤트 연결</summary>
    private void SetupSlot(Button button, string slotType, Action<string, InventoryItem> onEquipDropped)
    {
        if (button == null) return;
        var slotView = button.GetComponent<EquipmentSlotView>();
        if (slotView == null)
            slotView = button.gameObject.AddComponent<EquipmentSlotView>();

        slotView.slotType = slotType;
        slotView.onItemDropped = onEquipDropped;
    }

    /// <summary>UI 갱신: 슬롯에 장착된 아이템 표시 및 클릭 이벤트 등록</summary>
    public void UpdateEquipmentUI(IReadOnlyList<EquipmentSlot> slots, Action<string> onSlotClicked)
    {
        foreach (var slot in slots)
        {
            Button btn = GetSlotButton(slot.slotType);
            if (btn == null) continue;

            if (slot.equipped == null || string.IsNullOrEmpty(slot.equipped.iconPath))
            {
                btn.gameObject.SetActive(false); // 슬롯 비활성화
            }
            else
            {
                btn.gameObject.SetActive(true);
                var image = btn.GetComponent<Image>();
                var icon = Resources.Load<Sprite>(slot.equipped.iconPath);
                if (image != null) image.sprite = icon;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => onSlotClicked?.Invoke(slot.slotType));
            }
        }
    }

    private Button GetSlotButton(string slotType)
    {
        return slotType switch
        {
            "head" => headSlot,
            "rshoulder" => rShoulderSlot,
            "lshoulder" => lShoulderSlot,
            "gem" => gemSlot,
            "weapon" => weaponSlot,
            "shield" => shieldSlot,
            _ => null
        };
    }

    /// <summary>슬롯 아이콘 설정</summary>
    public void SetEquipmentIcon(Sprite icon, string slotType)
    {
        var button = GetSlotButton(slotType);
        if (button != null)
        {
            var image = button.GetComponent<Image>();
            image.sprite = icon;
        }
    }
}
