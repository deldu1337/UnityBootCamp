using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] private GameObject equipmentUI; // ��� UI ��ü
    [SerializeField] private Button exitButton;      // ���â �ݱ� ��ư

    [Header("��� ����")]
    [SerializeField] private Button headSlot;
    [SerializeField] private Button rShoulderSlot;
    [SerializeField] private Button lShoulderSlot;
    [SerializeField] private Button gemSlot;
    [SerializeField] private Button weaponSlot;
    [SerializeField] private Button shieldSlot;

    public RectTransform RootRect
    {
        get
        {
            if (!equipmentUI) equipmentUI = GameObject.Find("EquipmentUI");
            return equipmentUI ? equipmentUI.GetComponent<RectTransform>() : null;
        }
    }

    /// <summary>�ʱ�ȭ: ���� ����, ���� ��ư ����</summary>
    public void Initialize(Action onExit, Action<string, InventoryItem> onEquipDropped)
    {
        if (equipmentUI == null)
            equipmentUI = GameObject.Find("EquipmentUI");

        if (equipmentUI != null)
        {
            exitButton = equipmentUI.GetComponentInChildren<Button>();
            exitButton.onClick.AddListener(() => onExit?.Invoke());
        }

        // ButtonPanel �� ��ư�� ������� ��������
        headSlot = GameObject.Find("ButtonPanel").transform.GetChild(0).GetComponentInChildren<Button>();
        rShoulderSlot = GameObject.Find("ButtonPanel").transform.GetChild(1).GetComponentInChildren<Button>();
        lShoulderSlot = GameObject.Find("ButtonPanel").transform.GetChild(2).GetComponentInChildren<Button>();
        gemSlot = GameObject.Find("ButtonPanel").transform.GetChild(3).GetComponentInChildren<Button>();
        weaponSlot = GameObject.Find("ButtonPanel").transform.GetChild(4).GetComponentInChildren<Button>();
        shieldSlot = GameObject.Find("ButtonPanel").transform.GetChild(5).GetComponentInChildren<Button>();

        Show(false);
    }

    /// <summary>���â Ȱ��/��Ȱ��</summary>
    public void Show(bool show)
    {
        equipmentUI?.SetActive(show);
        if (exitButton != null)
            exitButton.transform.SetAsLastSibling();
    }

    /// <summary>���� ��ư�� ������ ��� �̺�Ʈ ����</summary>
    private void SetupSlot(Button button, string slotType, Action<string, InventoryItem> onEquipDropped)
    {
        if (button == null) return;
        var slotView = button.GetComponent<EquipmentSlotView>();
        if (slotView == null)
            slotView = button.gameObject.AddComponent<EquipmentSlotView>();

        slotView.slotType = slotType;
        slotView.onItemDropped = onEquipDropped;
    }

    /// <summary>UI ����: ���Կ� ������ ������ ǥ�� �� Ŭ�� �̺�Ʈ ���</summary>
    public void UpdateEquipmentUI(IReadOnlyList<EquipmentSlot> slots, Action<string> onSlotClicked)
    {
        foreach (var slot in slots)
        {
            Button btn = GetSlotButton(slot.slotType);
            if (btn == null) continue;

            if (slot.equipped == null || string.IsNullOrEmpty(slot.equipped.iconPath))
            {
                btn.gameObject.SetActive(false); // ���� ��Ȱ��ȭ
            }
            else
            {
                btn.gameObject.SetActive(true);

                // ������ ����
                var image = btn.GetComponent<Image>();
                var icon = Resources.Load<Sprite>(slot.equipped.iconPath);
                if (image != null) image.sprite = icon;

                // �⺻ onClick ���� (��Ŭ�� ���� ����)
                btn.onClick.RemoveAllListeners();

                // ���� �� ����
                var slotView = btn.GetComponent<EquipmentSlotView>();
                if (slotView == null) slotView = btn.gameObject.AddComponent<EquipmentSlotView>();
                slotView.slotType = slot.slotType;
                slotView.onItemDropped = null;

                // DraggableItemView ����
                var draggable = btn.GetComponent<DraggableItemView>();
                if (draggable == null)
                    draggable = btn.gameObject.AddComponent<DraggableItemView>();

                draggable.Initialize(
                    slot.equipped,
                    ItemOrigin.Equipment,
                    dropCallback: null,
                    removeCallback: null,
                    equipCallback: null,
                    unequipCallback: (slotType, origin) =>
                    {
                        Debug.Log($"[EquipmentView] ��Ŭ�� ����: {slotType}, {origin}");
                        onSlotClicked?.Invoke(slotType);
                    }
                );

                // Hover tooltip ����
                var hover = btn.GetComponent<ItemHoverTooltip>();
                if (hover == null) hover = btn.gameObject.AddComponent<ItemHoverTooltip>();
                hover.SetItem(slot.equipped);
                hover.SetContext(ItemOrigin.Equipment);   // �� �߰�: ���â ���ؽ�Ʈ

                // PointerClick �̺�Ʈ ���� ��� (��Ŭ���� ó��)
                var trigger = btn.GetComponent<UnityEngine.EventSystems.EventTrigger>();
                if (trigger == null) trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                trigger.triggers.Clear();

                var entry = new UnityEngine.EventSystems.EventTrigger.Entry
                {
                    eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick
                };
                entry.callback.AddListener((data) =>
                {
                    var ev = (UnityEngine.EventSystems.PointerEventData)data;
                    if (ev.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right)
                    {
                        Debug.Log($"��Ŭ�� �� {slot.slotType} ����");
                        onSlotClicked?.Invoke(slot.slotType);
                    }
                    // ��Ŭ���� ����
                });
                trigger.triggers.Add(entry);
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

    /// <summary>���� ������ ����</summary>
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
