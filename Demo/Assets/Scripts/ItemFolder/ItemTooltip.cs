using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    [Header("UI")]
    [SerializeField] private GameObject tooltipPanel;  // Panel (Image+Button ����)
    [SerializeField] private Text tooltipText;

    [Header("Layout")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.1f, 0f);

    // (����) �ʱ� �� ������ ���⼭�� �ϰ� �ʹٸ� Color32 ���
    [Header("Colors (optional init)")]
    [SerializeField] private Color32 normalColor = new Color32(0, 0, 0, 225);
    [SerializeField] private Color32 hoverColor = new Color32(3, 62, 113, 255);

    private Transform followTarget;
    private Action onClick;
    private Button panelButton;
    private UIHoverColor hover; // �� �гο� �پ��ִ� ������Ʈ

    void Awake()
    {
        Instance = this;

        hover = tooltipPanel.GetComponent<UIHoverColor>();
        if (hover == null) hover = tooltipPanel.AddComponent<UIHoverColor>();
        hover.SetColors(normalColor, hoverColor); // ����: �ʱ� �� ����

        panelButton = tooltipPanel.GetComponent<Button>();
        if (panelButton == null) panelButton = tooltipPanel.AddComponent<Button>();
        panelButton.onClick.AddListener(() => onClick?.Invoke());

        Hide();
    }

    void Update()
    {
        if (!tooltipPanel.activeSelf || followTarget == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position + worldOffset);
        tooltipPanel.transform.position = screenPos;
    }

    public void ShowFor(Transform target, string info, Action onClick)
    {
        followTarget = target;
        tooltipText.text = info;
        this.onClick = onClick;

        // �г� ����� �⺻������
        var img = tooltipPanel.GetComponent<Image>();
        if (img) img.color = normalColor;

        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
        followTarget = null;
        onClick = null;
    }
}

