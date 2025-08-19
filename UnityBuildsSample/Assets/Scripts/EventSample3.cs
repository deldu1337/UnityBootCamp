using System;
using UnityEngine;
using UnityEngine.Events;
// UnityEngine.Events 네임스페이스 연결이 필수입니다.

// C#의 event와의 차이점
// 1. 인스펙터에서 확인이 가능하다.
// 2. 추가 제거 기능을 +=, -=으로 하지 않고 AddListener와 RemoveListener로 진행합니다.

//                     UnityAction           vs           UnityEvent
// 타입                 delegate                             class
// 기능                 함수 참조                   에디터에서 핸들러 직접 등록 가능
// 구독                   +, -                      AddListener(), RemoveListener()
// 사용할 상황        스크립트 코드 내 처리         인스펙터용 이벤트 시스템
// 속도                   빠름                      느림(연결 정보에 대한 파싱 후 런타임 실행 방식)
// 메모리                 적음                      많음
// GC 발생 여부           낮음                      높음
// 편의성             자체 설계 해야함              바로 사용 가능. 쉽고 편함

// UnityAction은 UnityEvent를 사용하는 코드를 설계할 때 효과적입니다.
// 일반 delegate는 UnityAction<T>와 같이 타입에 대한 설정이 안되어있어 따로 만들어서 사용해야 합니다.

// 사용할 수 있는 선택지
// 1. C# delegate
// 2. Unity UnityAction
// 3. C# Func<T>, Aciton<T>
public class EventSample3 : MonoBehaviour
{
    public UnityEvent OnKButtonEnter;
    public UnityAction OnAction;

    private void Start()
    {
        //OnKButtonEnter += Sample; // 오류
        OnKButtonEnter.AddListener(Sample);
        OnAction += Sample2;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            OnKButtonEnter?.Invoke();
        }
    }

    private void Sample()
    {
        Debug.Log("<color=cyan>실행</color>");
    }

    private void Sample2()
    {
        Debug.Log("<color=green>실행</color>");
    }
}
