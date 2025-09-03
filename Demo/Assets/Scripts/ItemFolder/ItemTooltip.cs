using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;
    [SerializeField] private GameObject tooltipPanel; // �г�
    [SerializeField] private Text tooltipText;
    private Vector2 padding = new Vector2(15f, 5f); // �¿졤���� ����

    void Awake()
    {
        Instance = this;
        Hide();
    }

    void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            // ���콺 ����ٴϵ���
            Vector3 mousePos = Input.mousePosition;
            tooltipPanel.transform.position = mousePos + new Vector3(30f, 8f, 0f);
        }
    }

    public void Show(string info)
    {
        tooltipText.text = info;

        // �ؽ�Ʈ�� ���� ũ�� �������� (���Ʒ� ���� �ּ�ȭ)
        Vector2 preferredSize = new Vector2(
            tooltipText.preferredWidth,
            tooltipText.preferredHeight
        );

        // �г� ũ�� = �ؽ�Ʈ ũ�� + �е�
        RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = preferredSize + padding;

        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
