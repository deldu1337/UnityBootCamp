using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private Canvas canvas;           // 기본 상위 Canvas (없어도 OK)
    [SerializeField] private RectTransform root;      // = InfoItem RectTransform
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text tierText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text statsText;

    [Header("Layout")]
    [SerializeField] private Vector2 screenOffset = new Vector2(16, -16);
    [SerializeField] private float gapFromIcon = 10f;
    [SerializeField] private float minHeight = 0f;    // 필요 시 최소 높이(옵션)

    //[SerializeField] private PlayerStatsManager playerStats;

    private Transform originalParent;
    private ItemHoverTooltip currentOwner;

    void Awake()
    {
        //if (playerStats == null)
        //    playerStats = FindAnyObjectByType<PlayerStatsManager>();

        Instance = this;
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (root == null) root = transform as RectTransform;

        // 예측 가능한 배치용 기본값
        root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0f, 0.5f);

        // 텍스트 색상 전부 흰색
        if (nameText) nameText.color = Color.white;
        if (levelText) levelText.color = Color.white;
        if (typeText) typeText.color = Color.white;
        if (statsText) statsText.color = new Color32(100, 96, 219, 255);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // 안전장치: 이 오브젝트가 비활성화될 때 항상 툴팁 숨김
        Hide();
    }

    // 기존: 마우스 위치 기준
    public void Show(InventoryItem item, Vector2 screenPos)
    {
        if (item == null || item.data == null) return;

        nameText.text = item.data.name;
        nameText.color = GetTierColor(item.data.tier);

        tierText.text = $"등급: {item.data.tier}";
        tierText.color = GetTierColor(item.data.tier);

        int required = Mathf.Max(1, item.data.level);
        levelText.text = $"요구 레벨: {required}";

        var ps = PlayerStatsManager.Instance;
        if (ps != null && ps.Data != null && ps.Data.Level < required)
            levelText.color = Color.red;
        else
            levelText.color = Color.white;

        if (typeText) typeText.text = $"분류: {item.data.type}";
        statsText.text = BuildStats(item);

        ForceResizeToContent();
        UpdatePosition(screenPos);

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    private string GetTypeDisplayName(string type)
    {
        switch (type.ToLower())
        {
            case "head": return "머리";
            case "rshoulder": return "오른쪽 어깨";
            case "lshoulder": return "왼쪽 어깨";
            case "weapon": return "무기";
            case "shield": return "방패";
            case "gem": return "젬";
            case "potion": return "포션";
            // 필요한 타입들 계속 추가
            default: return type; // 매핑이 없으면 원래 값 그대로
        }
    }


    // 타겟 RectTransform(아이콘) 옆에 고정 (왼쪽 우선)
    public void ShowNextTo(InventoryItem item, RectTransform target, ItemHoverTooltip owner)
    {
        if (item == null || item.data == null || target == null) return;

        nameText.text = item.data.name;
        nameText.color = GetTierColor(item.data.tier);

        tierText.text = $"등급: {item.data.tier}";
        tierText.color = GetTierColor(item.data.tier);

        int required = Mathf.Max(1, item.data.level);
        levelText.text = $"요구 레벨: {required}";

        var ps = PlayerStatsManager.Instance;
        if (ps != null && ps.Data != null)
            Debug.Log($"[Tooltip] Player Level = {ps.Data.Level}, Required = {required}");

        if (ps != null && ps.Data != null && ps.Data.Level < required)
            levelText.color = Color.red;
        else
            levelText.color = Color.white;

        if (typeText) typeText.text = $"부위: {GetTypeDisplayName(item.data.type)}";
        statsText.text = BuildStats(item);

        ForceResizeToContent();           // 동적 리사이즈

        // 타겟이 속한 Canvas로 툴팁 이동 (좌표계 통일)
        var targetCanvas = target.GetComponentInParent<Canvas>();
        if (targetCanvas == null) return;

        if (root.parent != targetCanvas.transform)
        {
            originalParent = root.parent; // 복귀용
            root.SetParent(targetCanvas.transform, worldPositionStays: false);
        }

        var cam = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
        var parentRect = root.parent as RectTransform; // = targetCanvas rect

        // 아이콘 월드 코너
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Vector3 worldRightMid = (corners[2] + corners[3]) * 0.5f; // TR~BR
        Vector3 worldLeftMid = (corners[0] + corners[1]) * 0.5f; // BL~TL

        // 부모(TargetCanvas) 기준 로컬 좌표
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, RectTransformUtility.WorldToScreenPoint(cam, worldRightMid), cam, out var parentLocalRight);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, RectTransformUtility.WorldToScreenPoint(cam, worldLeftMid), cam, out var parentLocalLeft);

        // 배치 결정: 왼쪽 우선, 없으면 오른쪽, 둘 다 부족하면 더 넓은 쪽
        bool canLeft = HasRoomOnLeft(parentLocalLeft, parentRect);
        bool canRight = HasRoomOnRight(parentLocalRight, parentRect);

        bool placeLeft;
        if (canLeft) placeLeft = true;
        else if (canRight) placeLeft = false;
        else
        {
            float leftSpace = AvailableSpaceLeft(parentLocalLeft, parentRect);
            float rightSpace = AvailableSpaceRight(parentLocalRight, parentRect);
            placeLeft = leftSpace >= rightSpace;
        }

        // 피벗/목표 위치
        root.pivot = placeLeft ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);

        Vector2 desired = placeLeft
            ? parentLocalLeft - new Vector2(gapFromIcon, 0f)    // 아이콘 왼쪽
            : parentLocalRight + new Vector2(gapFromIcon, 0f);   // 아이콘 오른쪽

        root.anchoredPosition = ClampInsideParent(desired, parentRect);

        currentOwner = owner;
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // 맨 위
    }

    public void Hide(ItemHoverTooltip requester = null)
    {
        if (requester != null && requester != currentOwner)
            return;

        gameObject.SetActive(false);
        currentOwner = null;
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        if (canvas == null) return;

        Vector2 sp = screenPos + screenOffset;
        RectTransform canvasRect = canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, sp, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out var local);

        root.anchoredPosition = ClampInsideCanvas(local, canvasRect);
    }

    // 동적 리사이즈: 자식 텍스트 preferredHeight + VLG spacing/padding을 합산
    private void ForceResizeToContent()
    {
        var vlg = root.GetComponent<VerticalLayoutGroup>();
        float spacing = vlg ? vlg.spacing : 0f;
        var padding = vlg ? vlg.padding : new RectOffset();

        // 레이아웃 갱신 (텍스트 내용 반영)
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);

        int enabledCount = 0;
        float totalHeights = 0f;

        // 각 텍스트의 preferredHeight 합산 (비활성/널은 0)
        if (nameText && nameText.gameObject.activeInHierarchy)
        {
            totalHeights += LayoutUtility.GetPreferredHeight(nameText.rectTransform);
            enabledCount++;
        }
        if (levelText && levelText.gameObject.activeInHierarchy)
        {
            totalHeights += LayoutUtility.GetPreferredHeight(levelText.rectTransform);
            enabledCount++;
        }
        if (typeText && typeText.gameObject.activeInHierarchy)
        {
            totalHeights += LayoutUtility.GetPreferredHeight(typeText.rectTransform);
            enabledCount++;
        }
        if (statsText && statsText.gameObject.activeInHierarchy)
        {
            totalHeights += LayoutUtility.GetPreferredHeight(statsText.rectTransform);
            enabledCount++;
        }

        float totalSpacing = Mathf.Max(0, enabledCount - 1) * spacing;
        float totalPadding = padding.top + padding.bottom;

        float newHeight = totalHeights + totalSpacing + totalPadding;
        if (minHeight > 0f) newHeight = Mathf.Max(newHeight, minHeight);

        // 실제 적용
        root.sizeDelta = new Vector2(root.sizeDelta.x, newHeight);

        // 적용 직후 한 번 더 강제 리빌드(드물게 첫 프레임 밀림 방지)
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    private Vector2 ClampInsideCanvas(Vector2 local, RectTransform canvasRect)
    {
        Vector2 size = root.sizeDelta;
        Vector2 canvasSize = canvasRect.rect.size;

        float leftLimit = -canvasSize.x * 0.5f + size.x * (1f - root.pivot.x);
        float rightLimit = canvasSize.x * 0.5f - size.x * (root.pivot.x);
        float bottomLimit = -canvasSize.y * 0.5f + size.y * (1f - root.pivot.y);
        float topLimit = canvasSize.y * 0.5f - size.y * (root.pivot.y);

        return new Vector2(
            Mathf.Clamp(local.x, leftLimit, rightLimit),
            Mathf.Clamp(local.y, bottomLimit, topLimit)
        );
    }

    private Vector2 ClampInsideParent(Vector2 local, RectTransform parentRect)
    {
        Vector2 size = root.rect.size; // 실제 렌더 크기
        Rect r = parentRect.rect;

        float leftLimit = r.xMin + size.x * (1f - root.pivot.x);
        float rightLimit = r.xMax - size.x * (root.pivot.x);
        float bottomLimit = r.yMin + size.y * (1f - root.pivot.y);
        float topLimit = r.yMax - size.y * (root.pivot.y);

        return new Vector2(
            Mathf.Clamp(local.x, leftLimit, rightLimit),
            Mathf.Clamp(local.y, bottomLimit, topLimit)
        );
    }

    private bool HasRoomOnLeft(Vector2 parentLocalLeft, RectTransform parentRect)
    {
        Vector2 size = root.rect.size;
        Rect r = parentRect.rect;
        float predictedLeftEdge = parentLocalLeft.x - gapFromIcon - size.x; // pivot.x=1 가정
        return predictedLeftEdge >= r.xMin;
    }

    private bool HasRoomOnRight(Vector2 parentLocalRight, RectTransform parentRect)
    {
        Vector2 size = root.rect.size;
        Rect r = parentRect.rect;
        float predictedRightEdge = parentLocalRight.x + gapFromIcon + size.x; // pivot.x=0 가정
        return predictedRightEdge <= r.xMax;
    }

    // 툴팁 너비를 제외하기 전 기준 가용 거리(양수면 여유, 음수면 부족)
    private float AvailableSpaceLeft(Vector2 parentLocalLeft, RectTransform parentRect)
    {
        Rect r = parentRect.rect;
        return parentLocalLeft.x - gapFromIcon - r.xMin;
    }

    private float AvailableSpaceRight(Vector2 parentLocalRight, RectTransform parentRect)
    {
        Rect r = parentRect.rect;
        return r.xMax - (parentLocalRight.x + gapFromIcon);
    }

    private string BuildStats(InventoryItem item)
    {
        var d = item.data;
        var sb = new StringBuilder();

        void Add(string label, float val)
        {
            if (Mathf.Abs(val) > 0.0001f) sb.AppendLine($"{label}  +{val}");
        }

        Add("HP", d.hp);
        Add("MP", d.mp);
        Add("데미지", d.atk);
        Add("방어력", d.def);
        Add("DEX", d.dex);
        Add("공격 속도", d.As);
        Add("치명타 확률", d.cc);
        Add("치명타 피해량", d.cd);

        if (sb.Length == 0) sb.Append("추가 능력치 없음");
        return sb.ToString();
    }

    private static Color GetTierColor(string tier)
    {
        if (string.IsNullOrEmpty(tier)) return Color.white;

        switch (tier.Trim().ToLower())
        {
            case "common": return Color.white;                         // 흰색
            case "uncommon": return new Color32(50, 205, 50, 255);     // 연두색 (LightGreen)
            case "rare": return new Color32(255, 128, 0, 255);                           // 빨간색
            case "unique": return new Color32(170, 0, 255, 255);       // 보라색 (퍼플 톤)
            case "legendary": return new Color32(255, 215, 0, 255);       // 주황색
            default: return Color.white;
        }
    }

}