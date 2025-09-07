using UnityEngine;
using UnityEngine.UI;

// ������ ���� ���� ��ũ��Ʈ
public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;           // �̱��� �ν��Ͻ�

    [SerializeField] private GameObject tooltipPanel; // ���� ��ü �г�
    [SerializeField] private Text tooltipText;        // ������ ǥ�õ� �ؽ�Ʈ

    private Vector2 padding = new Vector2(15f, 5f);   // �гΰ� �ؽ�Ʈ ���� ���� (�¿�, ����)

    // Awake: �̱��� �ʱ�ȭ �� �г� ����
    void Awake()
    {
        Instance = this;  // �̱��� �ν��Ͻ� ����
        Hide();           // ���� �� ���� ����
    }

    // �� ������ ������Ʈ: ���콺 ����ٴϰ�
    void Update()
    {
        if (tooltipPanel.activeSelf) // ������ Ȱ��ȭ�Ǿ� ������
        {
            Vector3 mousePos = Input.mousePosition;           // ���� ���콺 ��ġ
            tooltipPanel.transform.position = mousePos + new Vector3(0f, 5f, 0f); // ���콺 ��ġ ���� ������ ����
        }
    }

    // ���� ǥ��
    public void Show(string info)
    {
        tooltipText.text = info;  // ���� �ؽ�Ʈ ����

        // �ؽ�Ʈ�� ���� ũ�� ��� (�ؽ�Ʈ ���뿡 ����)
        Vector2 preferredSize = new Vector2(
            tooltipText.preferredWidth,   // �ؽ�Ʈ �ʺ�
            tooltipText.preferredHeight   // �ؽ�Ʈ ����
        );

        // �г� ũ�� = �ؽ�Ʈ ũ�� + ����
        RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = preferredSize + padding;

        tooltipPanel.SetActive(true);  // ���� �г� Ȱ��ȭ
    }

  
    // ���� ����
    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
