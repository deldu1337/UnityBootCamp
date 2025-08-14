using UnityEngine;
using UnityEngine.EventSystems;
// 1. 이벤트 시스템 컴포넌트가 씬에 존재해야 합니다.
// 2. UI의 경우 그래픽 레이캐스터 컴포넌트가 추가되어있어야 하며, RayCast의 Target도
//    체크가 되어 있어야 합니다.
// 3. 오브젝트의 경우에는 Collider / Collider2D 컴포넌트가 추가되어 있어야 합니다.
// 4. 메인 카메라에 Physics RayCaster가 추가되어 있어야 합니다.

// 제공되는 기능 기준 일반적인 순서
// 1. Pointer Enter
// 2. Pointer Down
// 3. Begin Drag
// 4. Dragging
// 5. End Drag
// 6. Pointer Up
// 7. Pointer Exit
// 8. Select
// 9. Submit
// 10. Move
// 11. Cancel

public class ClickTester : MonoBehaviour, IPointerClickHandler,
                            IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // PointerEventData: 마우스 클릭 시의 클릭 버튼, 클릭 횟수,
    //                   포인터 위치 등에 대한 정보를 가지고 있는 클래스

    // 마우스로 1회 클릭하는 경우
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("터치나 마우스 포인터의 위치 : " + eventData.position);
        Debug.Log("클릭 횟수 : " + eventData.clickCount);
        Debug.Log("클릭 시간 : " + eventData.clickTime);
        Debug.Log("포인터의 이동량 : " + eventData.delta);
        Debug.Log("현재 드래그 여부 : " + eventData.dragging);
        // eventData.button = 버튼
        // eventData.phase = 현재의 이벤트 단계
        // eventData.pointId = 터치 또는 마우스 포인터의 ID
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("포인터 눌렀을 때");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("포인터 탈출");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("포인터 터치를 뗄 때");
    }
}

