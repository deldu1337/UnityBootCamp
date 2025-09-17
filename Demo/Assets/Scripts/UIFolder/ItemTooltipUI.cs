using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private Canvas canvas;            // ���� Canvas
    [SerializeField] private RectTransform root;       // �� �г��� RectTransform
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text typeText;            // ����
    [SerializeField] private Text statsText;
    //[SerializeField] private Image iconImage;          // ����

    [Header("Layout")]
    [SerializeField] private Vector2 screenOffset = new Vector2(16, -16);

    void Awake()
    {
        Instance = this;
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (root == null) root = transform as RectTransform;
        gameObject.SetActive(false);
    }

    public void Show(InventoryItem item, Vector2 screenPos)
    {
        if (item == null || item.data == null) return;

        nameText.text = item.data.name;
        levelText.text = $"�䱸 ����: {Mathf.Max(1, item.data.level)}";
        if (typeText) typeText.text = $"�з�: {item.data.type}";
        statsText.text = BuildStats(item);

        //if (iconImage)
        //{
        //    iconImage.enabled = !string.IsNullOrEmpty(item.iconPath);
        //    iconImage.sprite = !string.IsNullOrEmpty(item.iconPath) ? Resources.Load<Sprite>(item.iconPath) : null;
        //}

        // ���̾ƿ� ���� ���� �� ��ġ ���
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        UpdatePosition(screenPos);

        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // ���� ����
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        if (canvas == null) return;

        Vector2 sp = screenPos + screenOffset;
        RectTransform canvasRect = canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, sp, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out var local);

        // ȭ�� ������ �� ������ Ŭ����
        Vector2 size = root.sizeDelta;
        Vector2 canvasSize = canvasRect.rect.size;

        Vector2 clamped = new Vector2(
            Mathf.Clamp(local.x, -canvasSize.x * 0.5f + size.x * 0.5f, canvasSize.x * 0.5f - size.x * 0.5f),
            Mathf.Clamp(local.y, -canvasSize.y * 0.5f + size.y * 0.5f, canvasSize.y * 0.5f - size.y * 0.5f)
        );

        root.anchoredPosition = clamped;
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
        Add("���ݷ�", d.atk);
        Add("����", d.def);
        Add("��ø", d.dex);
        Add("����", d.As);
        Add("ġȮ", d.cc);
        Add("ġ��", d.cd);

        if (sb.Length == 0) sb.Append("�߰� �ɷ�ġ ����");
        return sb.ToString();
    }
}
