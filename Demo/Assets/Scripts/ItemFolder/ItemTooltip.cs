using UnityEngine;
using UnityEngine.UI;

// 아이템 툴팁 관리 스크립트
public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;           // 싱글톤 인스턴스

    [SerializeField] private GameObject tooltipPanel; // 툴팁 전체 패널
    [SerializeField] private Text tooltipText;        // 툴팁에 표시될 텍스트

    private Vector2 padding = new Vector2(15f, 5f);   // 패널과 텍스트 사이 여백 (좌우, 상하)

    // Awake: 싱글톤 초기화 및 패널 숨김
    void Awake()
    {
        Instance = this;  // 싱글톤 인스턴스 설정
        Hide();           // 시작 시 툴팁 숨김
    }

    // 매 프레임 업데이트: 마우스 따라다니게
    void Update()
    {
        if (tooltipPanel.activeSelf) // 툴팁이 활성화되어 있으면
        {
            Vector3 mousePos = Input.mousePosition;           // 현재 마우스 위치
            tooltipPanel.transform.position = mousePos + new Vector3(0f, 5f, 0f); // 마우스 위치 기준 오프셋 적용
        }
    }

    // 툴팁 표시
    public void Show(string info)
    {
        tooltipText.text = info;  // 툴팁 텍스트 설정

        // 텍스트의 실제 크기 계산 (텍스트 내용에 맞춤)
        Vector2 preferredSize = new Vector2(
            tooltipText.preferredWidth,   // 텍스트 너비
            tooltipText.preferredHeight   // 텍스트 높이
        );

        // 패널 크기 = 텍스트 크기 + 여백
        RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = preferredSize + padding;

        tooltipPanel.SetActive(true);  // 툴팁 패널 활성화
    }

  
    // 툴팁 숨김
    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
