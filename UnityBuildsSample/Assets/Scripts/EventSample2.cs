using System;
using UnityEngine;

// EventSample 컴포넌트로 붙어있는 객체에 연결해주겠습니다.
public class EventSample2 : MonoBehaviour
{
    void Start()
    {
        // 다른 클래스에서 이벤트를 구독하는 방법

        // 클래스 접근
        EventSample event_sample = GetComponent<EventSample>();

        // 클래스가 가진 이벤트에 추가
        event_sample.OnSpaceEnter += OnSpaceButton;
    }
    private void OnSpaceButton(object sender, EventArgs e)
    {
        Debug.Log("<color=blue>Sample2에서 등록한 기능</color>");
    }
}
