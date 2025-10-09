//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class SkillBookItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
//{
//    [Header("Refs")]
//    public Image icon;                 // �ڽ� "Icon"
//    public GameObject lockOverlay;     // �ڽ� "LockOverlay"
//    [SerializeField] private Image bg; // �ڽ� "Bg" (����)

//    [Header("Runtime")]
//    public string SkillId { get; private set; }
//    public int UnlockLevel { get; private set; }
//    public bool Unlocked { get; private set; }
//    public Sprite IconSprite { get; private set; }

//    private Canvas rootCanvas;
//    private Image ghost;

//    void Awake()
//    {
//        rootCanvas = GetComponentInParent<Canvas>();

//        if (bg) bg.raycastTarget = false;

//        if (icon)
//        {
//            icon.preserveAspect = true;
//            icon.raycastTarget = false;
//            // ���⼭�� �� �̻� ������ ����
//            // icon.enabled = false;  �� ����
//        }
//    }

//    void OnEnable()
//    {
//        EnsureIconVisibility();
//    }

//    public void Setup(string id, Sprite sp, int unlockLv, bool unlocked)
//    {
//        SkillId = id;
//        UnlockLevel = unlockLv;
//        IconSprite = sp;
//        Unlocked = unlocked;

//        if (icon) icon.sprite = sp;
//        if (lockOverlay) lockOverlay.SetActive(!unlocked);

//        EnsureIconVisibility();
//    }

//    public void SetUnlocked(bool unlocked)
//    {
//        Unlocked = unlocked;
//        if (lockOverlay) lockOverlay.SetActive(!unlocked);
//        EnsureIconVisibility();
//    }

//    private void EnsureIconVisibility()
//    {
//        // ������ ��������Ʈ�� ������ �׻� ���̰�
//        if (icon)
//            icon.enabled = (IconSprite != null || icon.sprite != null);

//        // �ʿ��ϸ� ���� ���� ����(��: ��� �� ������)
//        // if (icon) icon.color = Unlocked ? Color.white : new Color(1,1,1,0.7f);
//    }

//    // ===== Drag =====
//    public void OnBeginDrag(PointerEventData e)
//    {
//        if (!Unlocked || icon == null || icon.sprite == null) return;

//        ghost = new GameObject("Ghost", typeof(Image)).GetComponent<Image>();
//        ghost.raycastTarget = false;
//        ghost.transform.SetParent(rootCanvas.transform, false);
//        ghost.rectTransform.sizeDelta = icon.rectTransform.rect.size;
//        ghost.sprite = icon.sprite;
//        ghost.color = new Color(1, 1, 1, 0.8f);

//        UpdateGhost(e);
//    }

//    public void OnDrag(PointerEventData e)
//    {
//        if (ghost) UpdateGhost(e);
//    }

//    public void OnEndDrag(PointerEventData e)
//    {
//        if (ghost) Destroy(ghost.gameObject);
//        ghost = null;
//    }

//    void UpdateGhost(PointerEventData e)
//    {
//        RectTransformUtility.ScreenPointToLocalPointInRectangle(
//            rootCanvas.transform as RectTransform,
//            e.position,
//            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
//            out var local
//        );
//        ghost.rectTransform.anchoredPosition = local;
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillBookItemDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Refs")]
    public Image icon;                 // �ڽ� "Icon"
    public GameObject lockOverlay;     // �ڽ� "LockOverlay"
    [SerializeField] private Image bg; // �ڽ� "Bg" (����)

    [Header("Runtime")]
    public string SkillId { get; private set; }
    public int UnlockLevel { get; private set; }
    public bool Unlocked { get; private set; }
    public Sprite IconSprite { get; private set; }

    private Canvas rootCanvas;
    private Image ghost;

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>(true);

        if (bg) bg.raycastTarget = false;

        if (icon)
        {
            icon.preserveAspect = true;
            icon.raycastTarget = false; // �巡�׽� ���콺 �� ����
        }
    }

    void OnEnable() => EnsureIconVisibility();

    public void Setup(string id, Sprite sp, int unlockLv, bool unlocked)
    {
        SkillId = id;
        UnlockLevel = unlockLv;
        IconSprite = sp;
        Unlocked = unlocked;

        if (icon) icon.sprite = sp;
        if (lockOverlay) lockOverlay.SetActive(!unlocked);

        EnsureIconVisibility();
    }

    public void SetUnlocked(bool unlocked)
    {
        Unlocked = unlocked;
        if (lockOverlay) lockOverlay.SetActive(!unlocked);
        EnsureIconVisibility();
    }

    private void EnsureIconVisibility()
    {
        // ������ ��������Ʈ�� ������ �׻� ���̰�
        if (icon)
            icon.enabled = (IconSprite != null || icon.sprite != null);
        // ��� ���¶�� �ణ�� ���ĸ� �� ���� ���� (���ϸ� �ּ� ����)
        // if (icon) icon.color = Unlocked ? Color.white : new Color(1, 1, 1, 0.7f);
    }

    // ===== Drag =====
    public void OnBeginDrag(PointerEventData e)
    {
        if (!Unlocked || icon == null || icon.sprite == null) return;
        if (!rootCanvas) rootCanvas = GetComponentInParent<Canvas>(true);
        if (!rootCanvas) return;

        ghost = new GameObject("Ghost", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        ghost.raycastTarget = false;
        ghost.transform.SetParent(rootCanvas.transform, false);
        ghost.sprite = icon.sprite;
        ghost.color = new Color(1, 1, 1, 0.85f);

        // ������ ũ�⿡ ���� ������
        var size = icon.rectTransform.rect.size;
        ghost.rectTransform.sizeDelta = size;

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

    private void UpdateGhost(PointerEventData e)
    {
        if (!rootCanvas || !ghost) return;

        Camera cam = null;
        if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = rootCanvas.worldCamera;
        else if (rootCanvas.renderMode == RenderMode.WorldSpace)
            cam = rootCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            e.position,
            cam,
            out var local
        );
        ghost.rectTransform.anchoredPosition = local;
    }
}
