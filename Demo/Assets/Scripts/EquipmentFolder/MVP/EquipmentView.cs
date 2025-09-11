using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private Button exitButton;

    [Header("장비 슬롯")]
    [SerializeField] private Button headSlot; // 예시: 무기 슬롯 하나
    [SerializeField] private Button rShoulderSlot;  // 예시: 방어구 슬롯
    [SerializeField] private Button lShoulderSlot; // 예시: 무기 슬롯 하나
    [SerializeField] private Button gemSlot;  // 예시: 방어구 슬롯
    [SerializeField] private Button weaponSlot; // 예시: 무기 슬롯 하나
    [SerializeField] private Button shieldSlot; // 예시: 무기 슬롯 하나

    public void Initialize(Action onExit, Action<string, InventoryItem> onEquipDropped)
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

        headSlot = GameObject.Find("ButtonPanel").transform.GetChild(0).GetComponentInChildren<Button>();
        rShoulderSlot = GameObject.Find("ButtonPanel").transform.GetChild(1).GetComponentInChildren<Button>();
        lShoulderSlot = GameObject.Find("ButtonPanel").transform.GetChild(2).GetComponentInChildren<Button>();
        gemSlot = GameObject.Find("ButtonPanel").transform.GetChild(3).GetComponentInChildren<Button>();
        weaponSlot = GameObject.Find("ButtonPanel").transform.GetChild(4).GetComponentInChildren<Button>();
        shieldSlot = GameObject.Find("ButtonPanel").transform.GetChild(5).GetComponentInChildren<Button>();

        Show(false);
    }

    public void Show(bool show)
    {
        equipmentUI?.SetActive(show);
        if (exitButton != null)
            exitButton.transform.SetAsLastSibling();
    }

    private void SetupSlot(Button button, string slotType, Action<string, InventoryItem> onEquipDropped)
    {
        if (button == null) return;
        var slotView = button.GetComponent<EquipmentSlotView>();
        if (slotView == null)
            slotView = button.gameObject.AddComponent<EquipmentSlotView>();

        slotView.slotType = slotType;
        slotView.onItemDropped = onEquipDropped;
    }

    public void UpdateEquipmentUI(IReadOnlyList<EquipmentSlot> slots, Action<string> onSlotClicked)
    {
        foreach (var slot in slots)
        {
            Button btn = GetSlotButton(slot.slotType);
            if (btn == null)
                continue;

            if (slot.equipped == null || string.IsNullOrEmpty(slot.equipped.iconPath))
            {
                // 아이템 없으면 버튼 비활성화
                btn.gameObject.SetActive(false);
            }
            else
            {
                // 아이템 있으면 버튼 활성화 및 아이콘 적용
                btn.gameObject.SetActive(true);
                var image = btn.GetComponent<Image>();
                var icon = Resources.Load<Sprite>(slot.equipped.iconPath);
                if (image != null)
                    image.sprite = icon;

                // 클릭 이벤트 연결
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

    public void SetEquipmentIcon(Sprite icon, string slotType)
    {
        if (slotType == "head" && headSlot != null)
        {
            // 아이콘 적용
            var image = headSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "rshoulder" && rShoulderSlot != null)
        {
            // 아이콘 적용
            var image = rShoulderSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "lshoulder" && lShoulderSlot != null)
        {
            // 아이콘 적용
            var image = lShoulderSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "gem" && gemSlot != null)
        {
            // 아이콘 적용
            var image = gemSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "weapon" && weaponSlot != null)
        {
            var image = weaponSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "shield" && shieldSlot != null)
        {
            var image = shieldSlot.GetComponent<Image>();
            image.sprite = icon;
        }
    }
}
