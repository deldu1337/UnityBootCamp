using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private Button exitButton;

    [Header("��� ����")]
    [SerializeField] private Button headSlot; // ����: ���� ���� �ϳ�
    [SerializeField] private Button rShoulderSlot;  // ����: �� ����
    [SerializeField] private Button lShoulderSlot; // ����: ���� ���� �ϳ�
    [SerializeField] private Button gemSlot;  // ����: �� ����
    [SerializeField] private Button weaponSlot; // ����: ���� ���� �ϳ�
    [SerializeField] private Button shieldSlot; // ����: ���� ���� �ϳ�

    public void Initialize(Action onExit, Action<string, InventoryItem> onEquipDropped)
    {
        if (equipmentUI == null)
            equipmentUI = GameObject.Find("EquipmentUI");

        if (equipmentUI != null)
        {
            // ExitButton ã��
            exitButton = equipmentUI.GetComponentInChildren<Button>();
            Debug.Log(exitButton.name);

            // ��ư Ŭ�� �̺�Ʈ ���
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
                // ������ ������ ��ư ��Ȱ��ȭ
                btn.gameObject.SetActive(false);
            }
            else
            {
                // ������ ������ ��ư Ȱ��ȭ �� ������ ����
                btn.gameObject.SetActive(true);
                var image = btn.GetComponent<Image>();
                var icon = Resources.Load<Sprite>(slot.equipped.iconPath);
                if (image != null)
                    image.sprite = icon;

                // Ŭ�� �̺�Ʈ ����
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
            // ������ ����
            var image = headSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "rshoulder" && rShoulderSlot != null)
        {
            // ������ ����
            var image = rShoulderSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "lshoulder" && lShoulderSlot != null)
        {
            // ������ ����
            var image = lShoulderSlot.GetComponent<Image>();
            image.sprite = icon;
        }
        else if (slotType == "gem" && gemSlot != null)
        {
            // ������ ����
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
