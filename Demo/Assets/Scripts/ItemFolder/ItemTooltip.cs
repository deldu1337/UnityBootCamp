using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;
    [SerializeField] private GameObject tooltipPanel; // �г�
    [SerializeField] private Text tooltipText;

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
            tooltipPanel.transform.position = mousePos + new Vector3(10f, -10f, 0f);
        }
    }

    public void Show(string info)
    {
        tooltipText.text = info;
        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
