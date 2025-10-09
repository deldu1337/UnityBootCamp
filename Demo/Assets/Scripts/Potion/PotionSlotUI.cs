using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionSlotUI : MonoBehaviour, IDropHandler
{
    [Tooltip("0~3 (Ű 1~4�� ����)")]
    public int index;

    [Header("UI")]
    public Image icon;                 // �ڽĿ� �ִ� �̹���(��: "1", "2", "3", "4")
    public GameObject emptyOverlay;    // �� ������ �� ���̴� ���(����)

    // ���� ���Կ� ���ε��� �κ��丮 �������� uniqueId
    public string boundUniqueId;

    private Text qtyText; // �� �߰�

    // �ܺο��� �������� �ڵ����� ����� �����ϰ� ������ ȣ��
    public void AutoWireIconByChildName(string childName)
    {
        if (icon) return;
        var t = transform.Find(childName);
        icon = t ? t.GetComponent<Image>() : GetComponentInChildren<Image>(true);
        if (icon)
        {
            icon.raycastTarget = false;
            icon.enabled = false; // �� �ʱ⿣ ��Ȱ��
        }
    }

    //public void Clear()
    //{
    //    boundUniqueId = null;

    //    if (icon)
    //    {
    //        icon.sprite = null;
    //        icon.enabled = false; // �� �� ĭ: �̹��� ��
    //    }

    //    if (emptyOverlay) emptyOverlay.SetActive(true);
    //}

    //public void Set(InventoryItem item, Sprite s)
    //{
    //    boundUniqueId = item.uniqueId;

    //    if (icon)
    //    {
    //        icon.sprite = s;
    //        icon.enabled = s != null; // �� ������ ���� ���� ��
    //    }

    //    if (emptyOverlay) emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
    //}
    public void Clear()
    {
        boundUniqueId = null;

        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        SetQty(0); // �� ����
        if (emptyOverlay) emptyOverlay.SetActive(true);
    }

    // �� ���� ���� ����
    public void Set(InventoryItem item, Sprite s, int quantity)
    {
        boundUniqueId = item.uniqueId;

        if (icon)
        {
            icon.sprite = s;
            icon.enabled = s != null;
        }

        SetQty(quantity); // ��
        if (emptyOverlay) emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
    }

    public void SetBySave(string uniqueId, Sprite s, int quantity)
    {
        boundUniqueId = uniqueId;
        if (icon) { icon.sprite = s; icon.enabled = (s != null); }
        SetQty(quantity); // ��
        if (emptyOverlay) emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
    }

    public void SetBySave(string uniqueId, Sprite s) // �� ���� �ñ״�ó�� ���̴� �� ������ ����
    {
        SetBySave(uniqueId, s, 1);
    }

    public void SetQty(int q)
    {
        if (qtyText == null) qtyText = EnsureQtyLabel(transform);
        if (qtyText == null) return;

        if (q >= 1) // 1���� ǥ�� �� �ص� �Ǹ� �̷���, �ʿ��ϸ� >=1 ��
        {
            qtyText.text = q.ToString();
            qtyText.enabled = true;
        }
        else
        {
            qtyText.text = "";
            qtyText.enabled = false;
        }
    }

    private Text EnsureQtyLabel(Transform parent)
    {
        var t = parent.Find("Qty") as RectTransform;
        if (t == null)
        {
            var go = new GameObject("Qty", typeof(RectTransform));
            t = go.GetComponent<RectTransform>();
            t.SetParent(parent, false);
            t.anchorMin = new Vector2(1, 0);
            t.anchorMax = new Vector2(1, 0);
            t.pivot = new Vector2(1, 0);
            t.anchoredPosition = new Vector2(-10, 10);
            t.sizeDelta = new Vector2(60, 24);

            var txt = go.AddComponent<Text>();
            txt.alignment = TextAnchor.LowerRight;
            Font f = null;
            try { f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
            if (f == null) { try { f = Font.CreateDynamicFontFromOSFont("Arial", 22); } catch { } }
            txt.font = f;
            txt.fontSize = 22;
            txt.raycastTarget = false;

            var outline = go.AddComponent<Outline>();
            outline.effectDistance = new Vector2(1, -1);
            outline.useGraphicAlpha = true;

            qtyText = txt;
        }
        return t.GetComponent<Text>();
    }


    public bool IsEmpty => string.IsNullOrEmpty(boundUniqueId);

    // === �κ��丮���� �巡�� ��� ���� (���Ǹ� ���) ===
    public void OnDrop(PointerEventData eventData)
    {
        var drag = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<DraggableItemView>() : null;
        if (drag == null || drag.Item == null) return;

        var item = drag.Item;
        if (item.data == null || !string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
        {
            // ���Ǹ� ���
            return;
        }

        // ������ �ε�
        Sprite s = null;
        if (!string.IsNullOrEmpty(item.iconPath))
            s = Resources.Load<Sprite>(item.iconPath);

        PotionQuickBar.Instance.Assign(index, item, s);

        // �巡�� �ð����� ���� �ڸ��� ����(�κ��丮������ �巡�׵ǹǷ� UI�� �״��)
        drag.SnapBackToOriginal();
    }

    // PotionSlotUI.cs ���ο�
    public RectTransform GetRect() => GetComponent<RectTransform>();
    public Camera GetCanvasCamera() => GetComponentInParent<Canvas>()?.worldCamera;

    public void RefreshEmptyOverlay()
    {
        if (emptyOverlay)
            emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
        if (icon) icon.enabled = !string.IsNullOrEmpty(boundUniqueId) && icon.sprite != null;
    }

    //public void SetBySave(string uniqueId, Sprite s)
    //{
    //    boundUniqueId = uniqueId;
    //    if (icon) { icon.sprite = s; icon.enabled = (s != null); }
    //    if (emptyOverlay) emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
    //}
}
