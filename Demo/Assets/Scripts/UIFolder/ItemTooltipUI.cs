using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private Canvas canvas;           // �⺻ ���� Canvas (��� OK)
    [SerializeField] private RectTransform root;      // = InfoItem RectTransform
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text tierText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text statsText;

    [Header("Layout")]
    [SerializeField] private Vector2 screenOffset = new Vector2(16, -16);
    [SerializeField] private float gapFromIcon = 10f;
    [SerializeField] private float minHeight = 0f;    // �ʿ� �� �ּ� ����(�ɼ�)

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

        // ���� ������ ��ġ�� �⺻��
        root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0f, 0.5f);

        // �ؽ�Ʈ ���� ���� ���
        if (nameText) nameText.color = Color.white;
        if (levelText) levelText.color = Color.white;
        if (typeText) typeText.color = Color.white;
        if (statsText) statsText.color = new Color32(100, 96, 219, 255);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // ������ġ: �� ������Ʈ�� ��Ȱ��ȭ�� �� �׻� ���� ����
        Hide();
    }

    // ����: ���콺 ��ġ ����
    public void Show(InventoryItem item, Vector2 screenPos)
    {
        if (item == null || item.data == null) return;

        nameText.text = item.data.name;
        nameText.color = GetTierColor(item.data.tier);

        tierText.text = $"���: {item.data.tier}";
        tierText.color = GetTierColor(item.data.tier);

        int required = Mathf.Max(1, item.data.level);
        levelText.text = $"�䱸 ����: {required}";

        var ps = PlayerStatsManager.Instance;
        if (ps != null && ps.Data != null && ps.Data.Level < required)
            levelText.color = Color.red;
        else
            levelText.color = Color.white;

        if (typeText) typeText.text = $"�з�: {item.data.type}";
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
            case "head": return "�Ӹ�";
            case "rshoulder": return "������ ���";
            case "lshoulder": return "���� ���";
            case "weapon": return "����";
            case "shield": return "����";
            case "gem": return "��";
            case "potion": return "����";
            // �ʿ��� Ÿ�Ե� ��� �߰�
            default: return type; // ������ ������ ���� �� �״��
        }
    }


    // Ÿ�� RectTransform(������) ���� ���� (���� �켱)
    public void ShowNextTo(InventoryItem item, RectTransform target, ItemHoverTooltip owner)
    {
        if (item == null || item.data == null || target == null) return;

        nameText.text = item.data.name;
        nameText.color = GetTierColor(item.data.tier);

        tierText.text = $"���: {item.data.tier}";
        tierText.color = GetTierColor(item.data.tier);

        int required = Mathf.Max(1, item.data.level);
        levelText.text = $"�䱸 ����: {required}";

        var ps = PlayerStatsManager.Instance;
        if (ps != null && ps.Data != null)
            Debug.Log($"[Tooltip] Player Level = {ps.Data.Level}, Required = {required}");

        if (ps != null && ps.Data != null && ps.Data.Level < required)
            levelText.color = Color.red;
        else
            levelText.color = Color.white;

        if (typeText) typeText.text = $"����: {GetTypeDisplayName(item.data.type)}";
        statsText.text = BuildStats(item);

        ForceResizeToContent();           // ���� ��������

        // Ÿ���� ���� Canvas�� ���� �̵� (��ǥ�� ����)
        var targetCanvas = target.GetComponentInParent<Canvas>();
        if (targetCanvas == null) return;

        if (root.parent != targetCanvas.transform)
        {
            originalParent = root.parent; // ���Ϳ�
            root.SetParent(targetCanvas.transform, worldPositionStays: false);
        }

        var cam = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
        var parentRect = root.parent as RectTransform; // = targetCanvas rect

        // ������ ���� �ڳ�
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Vector3 worldRightMid = (corners[2] + corners[3]) * 0.5f; // TR~BR
        Vector3 worldLeftMid = (corners[0] + corners[1]) * 0.5f; // BL~TL

        // �θ�(TargetCanvas) ���� ���� ��ǥ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, RectTransformUtility.WorldToScreenPoint(cam, worldRightMid), cam, out var parentLocalRight);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, RectTransformUtility.WorldToScreenPoint(cam, worldLeftMid), cam, out var parentLocalLeft);

        // ��ġ ����: ���� �켱, ������ ������, �� �� �����ϸ� �� ���� ��
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

        // �ǹ�/��ǥ ��ġ
        root.pivot = placeLeft ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);

        Vector2 desired = placeLeft
            ? parentLocalLeft - new Vector2(gapFromIcon, 0f)    // ������ ����
            : parentLocalRight + new Vector2(gapFromIcon, 0f);   // ������ ������

        root.anchoredPosition = ClampInsideParent(desired, parentRect);

        currentOwner = owner;
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // �� ��
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

    // ���� ��������: �ڽ� �ؽ�Ʈ preferredHeight + VLG spacing/padding�� �ջ�
    private void ForceResizeToContent()
    {
        var vlg = root.GetComponent<VerticalLayoutGroup>();
        float spacing = vlg ? vlg.spacing : 0f;
        var padding = vlg ? vlg.padding : new RectOffset();

        // ���̾ƿ� ���� (�ؽ�Ʈ ���� �ݿ�)
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);

        int enabledCount = 0;
        float totalHeights = 0f;

        // �� �ؽ�Ʈ�� preferredHeight �ջ� (��Ȱ��/���� 0)
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

        // ���� ����
        root.sizeDelta = new Vector2(root.sizeDelta.x, newHeight);

        // ���� ���� �� �� �� ���� ������(�幰�� ù ������ �и� ����)
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
        Vector2 size = root.rect.size; // ���� ���� ũ��
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
        float predictedLeftEdge = parentLocalLeft.x - gapFromIcon - size.x; // pivot.x=1 ����
        return predictedLeftEdge >= r.xMin;
    }

    private bool HasRoomOnRight(Vector2 parentLocalRight, RectTransform parentRect)
    {
        Vector2 size = root.rect.size;
        Rect r = parentRect.rect;
        float predictedRightEdge = parentLocalRight.x + gapFromIcon + size.x; // pivot.x=0 ����
        return predictedRightEdge <= r.xMax;
    }

    // ���� �ʺ� �����ϱ� �� ���� ���� �Ÿ�(����� ����, ������ ����)
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
        Add("������", d.atk);
        Add("����", d.def);
        Add("DEX", d.dex);
        Add("���� �ӵ�", d.As);
        Add("ġ��Ÿ Ȯ��", d.cc);
        Add("ġ��Ÿ ���ط�", d.cd);

        if (sb.Length == 0) sb.Append("�߰� �ɷ�ġ ����");
        return sb.ToString();
    }

    private static Color GetTierColor(string tier)
    {
        if (string.IsNullOrEmpty(tier)) return Color.white;

        switch (tier.Trim().ToLower())
        {
            case "common": return Color.white;                         // ���
            case "uncommon": return new Color32(50, 205, 50, 255);     // ���λ� (LightGreen)
            case "rare": return new Color32(255, 128, 0, 255);                           // ������
            case "unique": return new Color32(170, 0, 255, 255);       // ����� (���� ��)
            case "legendary": return new Color32(255, 215, 0, 255);       // ��Ȳ��
            default: return Color.white;
        }
    }

}