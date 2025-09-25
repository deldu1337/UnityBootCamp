using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionSlotUI : MonoBehaviour, IDropHandler
{
    [Tooltip("0~3 (키 1~4에 매핑)")]
    public int index;

    [Header("UI")]
    public Image icon;                 // 자식에 있는 이미지(예: "1", "2", "3", "4")
    public GameObject emptyOverlay;    // 빈 슬롯일 때 보이는 배경(선택)

    // 현재 슬롯에 바인딩된 인벤토리 아이템의 uniqueId
    public string boundUniqueId;

    // 외부에서 아이콘을 자동으로 끌어다 연결하고 싶으면 호출
    public void AutoWireIconByChildName(string childName)
    {
        if (icon) return;
        var t = transform.Find(childName);
        icon = t ? t.GetComponent<Image>() : GetComponentInChildren<Image>(true);
        if (icon)
        {
            icon.raycastTarget = false;
            icon.enabled = false; // ← 초기엔 비활성
        }
    }

    public void Clear()
    {
        boundUniqueId = null;

        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false; // ← 빈 칸: 이미지 끔
        }

        if (emptyOverlay) emptyOverlay.SetActive(true);
    }

    public void Set(InventoryItem item, Sprite s)
    {
        boundUniqueId = item.uniqueId;

        if (icon)
        {
            icon.sprite = s;
            icon.enabled = s != null; // ← 아이콘 있을 때만 켬
        }

        if (emptyOverlay) emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
    }


    public bool IsEmpty => string.IsNullOrEmpty(boundUniqueId);

    // === 인벤토리에서 드래그 드롭 수신 (포션만 허용) ===
    public void OnDrop(PointerEventData eventData)
    {
        var drag = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<DraggableItemView>() : null;
        if (drag == null || drag.Item == null) return;

        var item = drag.Item;
        if (item.data == null || !string.Equals(item.data.type, "potion", StringComparison.OrdinalIgnoreCase))
        {
            // 포션만 허용
            return;
        }

        // 아이콘 로드
        Sprite s = null;
        if (!string.IsNullOrEmpty(item.iconPath))
            s = Resources.Load<Sprite>(item.iconPath);

        PotionQuickBar.Instance.Assign(index, item, s);

        // 드래그 시각물은 원래 자리로 복귀(인벤토리에서만 드래그되므로 UI는 그대로)
        drag.SnapBackToOriginal();
    }

    // PotionSlotUI.cs 내부에
    public RectTransform GetRect() => GetComponent<RectTransform>();
    public Camera GetCanvasCamera() => GetComponentInParent<Canvas>()?.worldCamera;

    public void RefreshEmptyOverlay()
    {
        if (emptyOverlay)
            emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
        if (icon) icon.enabled = !string.IsNullOrEmpty(boundUniqueId) && icon.sprite != null;
    }

    public void SetBySave(string uniqueId, Sprite s)
    {
        boundUniqueId = uniqueId;
        if (icon) { icon.sprite = s; icon.enabled = (s != null); }
        if (emptyOverlay) emptyOverlay.SetActive(string.IsNullOrEmpty(boundUniqueId));
    }
}
