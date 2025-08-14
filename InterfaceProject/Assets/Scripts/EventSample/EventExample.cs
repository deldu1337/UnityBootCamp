using System;
using UnityEngine;
// C# event 526 page

// event: 특정 상황이 발생했을 때 알림을 보내는 메커니즘
// 1. 플레이어가 죽었을 때, 알림 전달, 메소드 호출

// Action

//public class Tester : MonoBehaviour
//{
//    private void Start()
//    {
//        EventExample eventExample = new EventExample();
//        eventExample.onDeath?.Invoke();
//        //eventExample.onStart?.Invoke(); // event 키워드가 있을 경우, 외부에서의 호출 불가능

//        eventExample.onDeath = null;
//        //eventExample.onStart = null; // event 키워드는 대입이 불가능

//        eventExample.onStart += Samples; // 구독 가능 / 구독 취소도 가능
//    }

//    private void Samples()
//    {

//    }
//}

//                   Action          vs          EventAction
// 외부 호출           o                             x
// 외부 대입           o                             x
// 구독 기능           o                             o
// 구독 취소           o                             o
// 주 용도        내부로직, 콜백                 이벤트 알림

public class EventExample : MonoBehaviour
{
    // Action vs Event Action ??
    public Action onDeath;
    public event Action onStart;

    private void Start()
    {
        // 액션은 +=를 통해 함수(메소드)를 액션에 등록할 수 있습니다. (등록)
        // 액션은 -=를 통해 함수(메소드)를 액션에서 제거할 수 있습니다. (해제)
        // 액션의 기능을 호출하면 등록되어있는 함수가 순차적으로 호출됩니다.
        onStart += Ready;
        onStart += Fight;

        onDeath += Damaged;
        onDeath += Dead;

        // onStart에 등록된 기능을 수행하는 코드 invoke();
        onStart?.Invoke();
        onDeath?.Invoke();

        // 함수처럼 호출하는 것도 가능합니다.
        onStart();
        onDeath();

        // 둘의 차이? 없음. 문법 스타일 차이
        // 함수 호출도 어차피 내부적으로 invoke()를 호출하게 되어있음.
        // Invoke 방식이면 null 체크 가능. 외부에서의 호출, 안전성 요구 시 추천
        // 함수 형태면 따로 설계해줘야 함. 내부 코드이거나 단순 호출일 경우 해당 방식 추천
    }

    private void Fight()
    {
        Debug.Log("<color=yellow><b>Fight!!</b></color>");
    }

    private void Ready()
    {
        Debug.Log("<color=cyan><b>Ready??</b></color>");
    }

    private void Dead()
    {
        Debug.Log("<color=blue><b>A hero is fallen.</b></color>");
    }

    private void Damaged()
    {
        Debug.Log("<color=red><b>Critical Demage!</b></color>");
    }
}
