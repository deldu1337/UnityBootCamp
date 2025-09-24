using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    [Header("UI")]
    [SerializeField] private GameObject tooltipPanel;  // Panel (Image+Button 포함)
    [SerializeField] private Text tooltipText;

    [Header("Layout")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.1f, 0f);

    // (선택) 초기 색 세팅을 여기서도 하고 싶다면 Color32 사용
    [Header("Colors (optional init)")]
    [SerializeField] private Color32 normalColor = new Color32(0, 0, 0, 225);
    [SerializeField] private Color32 hoverColor = new Color32(3, 62, 113, 255);

    private Transform followTarget;
    private Action onClick;
    private Button panelButton;
    private UIHoverColor hover; // ← 패널에 붙어있는 컴포넌트

    void Awake()
    {
        Instance = this;

        hover = tooltipPanel.GetComponent<UIHoverColor>();
        if (hover == null) hover = tooltipPanel.AddComponent<UIHoverColor>();
        hover.SetColors(normalColor, hoverColor); // 선택: 초기 색 지정

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

        // 패널 배경을 기본색으로
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

