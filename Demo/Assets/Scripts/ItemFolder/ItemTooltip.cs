using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;
    [SerializeField] private GameObject tooltipPanel; // 패널
    [SerializeField] private Text tooltipText;
    private Vector2 padding = new Vector2(15f, 5f); // 좌우·상하 여백

    void Awake()
    {
        Instance = this;
        Hide();
    }

    void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            // 마우스 따라다니도록
            Vector3 mousePos = Input.mousePosition;
            tooltipPanel.transform.position = mousePos + new Vector3(30f, 8f, 0f);
        }
    }

    public void Show(string info)
    {
        tooltipText.text = info;

        // 텍스트의 실제 크기 가져오기 (위아래 여백 최소화)
        Vector2 preferredSize = new Vector2(
            tooltipText.preferredWidth,
            tooltipText.preferredHeight
        );

        // 패널 크기 = 텍스트 크기 + 패딩
        RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = preferredSize + padding;

        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
