using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Refs")]
    public Image icon;                    // 슬롯 아이콘 이미지
    public SkillCooldownUI cooldownUI;    // 이미 쓰고 있는 쿨다운 UI 컴포넌트 (필요하면 연결)
    public int index;                     // 퀵슬롯 인덱스 (0~8)

    [Header("Runtime")]
    public string SkillId { get; private set; }
    private Transform dragParent;         // 드래그 중 임시 부모
    private Canvas rootCanvas;
    private Image ghost;                  // 드래그 미리보기(고스트)
    private Sprite currentIcon;

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (icon != null) icon.enabled = false;
    }

    public void SetSkill(string id, Sprite sp)
    {
        SkillId = id;
        currentIcon = sp;
        if (icon != null)
        {
            icon.sprite = sp;
            icon.enabled = !string.IsNullOrEmpty(id);
        }
    }

    public (string id, Sprite sprite) GetData() => (SkillId, currentIcon);
    public void ApplyData((string id, Sprite sprite) data) => SetSkill(data.id, data.sprite);

    // ============ Drag ============
    public void OnBeginDrag(PointerEventData e)
    {
        if (string.IsNullOrEmpty(SkillId) || icon == null) return;

        ghost = new GameObject("Ghost", typeof(Image)).GetComponent<Image>();
        ghost.raycastTarget = false;
        ghost.transform.SetParent(rootCanvas.transform, false);
        ghost.rectTransform.sizeDelta = icon.rectTransform.rect.size;
        ghost.sprite = icon.sprite;
        ghost.color = new Color(1, 1, 1, 0.8f);

        UpdateGhostPosition(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (ghost) UpdateGhostPosition(e);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (ghost) Destroy(ghost.gameObject);
        ghost = null;
    }

    void UpdateGhostPosition(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, e.position, rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera, out var local);
        ghost.rectTransform.anchoredPosition = local;
    }

    // ============ Drop ============
    public void OnDrop(PointerEventData e)
    {
        // 드래그 시작한 곳이 SkillSlotUI 또는 SkillBookItemDraggable 인지 검사
        if (e.pointerDrag == null) return;

        var fromSlot = e.pointerDrag.GetComponent<SkillSlotUI>();
        if (fromSlot != null && fromSlot != this)
        {
            // 슬롯 ↔ 슬롯 스왑
            GetComponentInParent<SkillQuickBar>().Swap(fromSlot.index, index);
            return;
        }

        var bookItem = e.pointerDrag.GetComponent<SkillBookItemDraggable>();
        if (bookItem != null && bookItem.Unlocked)
        {
            // 스킬북 → 슬롯 할당
            SetSkill(bookItem.SkillId, bookItem.IconSprite);
            return;
        }
    }
}
