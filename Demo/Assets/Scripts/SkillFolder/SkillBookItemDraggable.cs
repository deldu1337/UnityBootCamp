using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillBookItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Refs")]
    public Image icon;
    public GameObject lockOverlay;    // 잠금일 때 보이게

    [Header("Runtime")]
    public string SkillId { get; private set; }
    public int UnlockLevel { get; private set; }
    public bool Unlocked { get; private set; }
    public Sprite IconSprite { get; private set; }

    private Canvas rootCanvas;
    private Image ghost;

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void Setup(string id, Sprite sp, int unlockLv, bool unlocked)
    {
        SkillId = id;
        UnlockLevel = unlockLv;
        IconSprite = sp;
        if (icon) { icon.sprite = sp; icon.enabled = sp != null; }
        SetUnlocked(unlocked);
    }

    public void SetUnlocked(bool unlocked)
    {
        Unlocked = unlocked;
        if (lockOverlay) lockOverlay.SetActive(!unlocked);
    }

    // ============ Drag ============
    public void OnBeginDrag(PointerEventData e)
    {
        if (!Unlocked || icon == null || icon.sprite == null) return;

        ghost = new GameObject("Ghost", typeof(Image)).GetComponent<Image>();
        ghost.raycastTarget = false;
        ghost.transform.SetParent(rootCanvas.transform, false);
        ghost.rectTransform.sizeDelta = icon.rectTransform.rect.size;
        ghost.sprite = icon.sprite;
        ghost.color = new Color(1, 1, 1, 0.8f);

        UpdateGhost(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (ghost) UpdateGhost(e);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (ghost) Destroy(ghost.gameObject);
        ghost = null;
    }

    void UpdateGhost(PointerEventData e)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, e.position, rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera, out var local);
        ghost.rectTransform.anchoredPosition = local;
    }
}
