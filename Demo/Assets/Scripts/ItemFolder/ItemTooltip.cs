//using System;
//using UnityEngine;
//using UnityEngine.UI;

//public class ItemTooltip : MonoBehaviour
//{
//    public static ItemTooltip Instance;

//    [Header("UI")]
//    [SerializeField] private GameObject tooltipPanel;  // Panel (Image+Button ����)
//    [SerializeField] private Text tooltipText;

//    [Header("Layout")]
//    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.1f, 0f);

//    // (����) �ʱ� �� ������ ���⼭�� �ϰ� �ʹٸ� Color32 ���
//    [Header("Colors (optional init)")]
//    [SerializeField] private Color32 normalColor = new Color32(0, 0, 0, 225);
//    [SerializeField] private Color32 hoverColor = new Color32(3, 62, 113, 255);

//    private Transform followTarget;
//    private Action onClick;
//    private Button panelButton;
//    private UIHoverColor hover; // �� �гο� �پ��ִ� ������Ʈ

//    void Awake()
//    {
//        Instance = this;

//        hover = tooltipPanel.GetComponent<UIHoverColor>();
//        if (hover == null) hover = tooltipPanel.AddComponent<UIHoverColor>();
//        hover.SetColors(normalColor, hoverColor); // ����: �ʱ� �� ����

//        panelButton = tooltipPanel.GetComponent<Button>();
//        if (panelButton == null) panelButton = tooltipPanel.AddComponent<Button>();
//        panelButton.onClick.AddListener(() => onClick?.Invoke());

//        Hide();
//    }

//    void Update()
//    {
//        if (!tooltipPanel.activeSelf || followTarget == null) return;
//        Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position + worldOffset);
//        tooltipPanel.transform.position = screenPos;
//    }

//    public void ShowFor(Transform target, string info, Action onClick)
//    {
//        followTarget = target;
//        tooltipText.text = info;
//        this.onClick = onClick;

//        // �г� ����� �⺻������
//        var img = tooltipPanel.GetComponent<Image>();
//        if (img) img.color = normalColor;

//        tooltipPanel.SetActive(true);
//    }

//    public void Hide()
//    {
//        tooltipPanel.SetActive(false);
//        followTarget = null;
//        onClick = null;
//    }
//}

using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    [Header("UI")]
    [SerializeField] private GameObject tooltipPanel;      // �� ��ü or ������
    [SerializeField] private Text tooltipText;

    [Header("Optional")]
    [SerializeField] private Canvas uiCanvas;              // ������ �ڵ� Ž��
    [SerializeField] private GameObject tooltipPanelPrefab;// �г��� ���ٸ� �ν��Ͻ� ������(����)

    [Header("Layout")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.1f, 0f);

    [Header("Colors")]
    [SerializeField] private Color32 normalColor = new Color32(0, 0, 0, 225);
    [SerializeField] private Color32 hoverColor = new Color32(3, 62, 113, 255);

    private Transform followTarget;
    private Action onClick;
    private Button panelButton;
    private UIHoverColor hover;

    void Awake()
    {
        Instance = this;
        EnsureCanvas();
        EnsurePanel();        // �ʿ��� ������Ʈ ���ε�����
        Hide();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        // tooltipPanel�� �ı��Ǿ��ų�(null ����) ���� �غ� �� ������ �н�
        if (!tooltipPanel || !tooltipPanel.activeSelf || followTarget == null) return;

        var cam = Camera.main;
        if (!cam) return;

        Vector3 screenPos = cam.WorldToScreenPoint(followTarget.position + worldOffset);
        tooltipPanel.transform.position = screenPos;
    }

    public void ShowFor(Transform target, string info, Action onClick)
    {
        if (!EnsurePanel())
        {
            Debug.LogWarning("[ItemTooltip] tooltipPanel�� ���� ǥ���� �� �����ϴ�.");
            return;
        }

        followTarget = target;
        tooltipText.text = info;
        this.onClick = onClick;

        // ���� �ʱ�ȭ
        var img = tooltipPanel.GetComponent<Image>();
        if (img) img.color = normalColor;

        tooltipPanel.SetActive(true);
        // �ʱ� ��ġ�� �� �� ���
        if (Camera.main)
            tooltipPanel.transform.position = Camera.main.WorldToScreenPoint(followTarget.position + worldOffset);
    }

    public void Hide()
    {
        if (tooltipPanel) tooltipPanel.SetActive(false);
        followTarget = null;
        onClick = null;
    }

    // ================== helpers ==================

    // �ı�/���Ҵ� ��� Ŀ���ϴ� ���� ����
    private bool EnsurePanel()
    {
        // �̹� ��ȿ�ϸ� OK
        if (tooltipPanel) return true;

        // 1) ������ �̸����� ã�ƺ���(�ɼ�: �ʿ信 �°� ����)
        var found = GameObject.Find("TooltipPanel");
        if (found) tooltipPanel = found;

        // 2) �������� ������ ĵ���� �Ʒ��� �ν��Ͻ� ����
        if (!tooltipPanel && tooltipPanelPrefab && EnsureCanvas())
        {
            tooltipPanel = Instantiate(tooltipPanelPrefab, uiCanvas.transform);
            tooltipPanel.name = "TooltipPanel";
        }

        if (!tooltipPanel) return false;

        // ������Ʈ ����ε�
        hover = tooltipPanel.GetComponent<UIHoverColor>();
        if (!hover) hover = tooltipPanel.AddComponent<UIHoverColor>();
        hover.SetColors(normalColor, hoverColor);

        panelButton = tooltipPanel.GetComponent<Button>();
        if (!panelButton) panelButton = tooltipPanel.AddComponent<Button>();
        panelButton.onClick.RemoveAllListeners();
        panelButton.onClick.AddListener(() => onClick?.Invoke());

        // �ؽ�Ʈ �ڵ� ���ε�(�ʿ��ϸ� ���� Drag&Drop)
        if (!tooltipText) tooltipText = tooltipPanel.GetComponentInChildren<Text>(true);

        return true;
    }

    private bool EnsureCanvas()
    {
        if (uiCanvas) return true;

        uiCanvas = FindAnyObjectByType<Canvas>();
        if (!uiCanvas)
        {
            Debug.LogWarning("[ItemTooltip] Canvas�� ���� tooltip�� ����/ǥ���� �� �����ϴ�.");
            return false;
        }
        return true;
    }
}
