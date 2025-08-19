using UnityEngine;
using System;
// C#의 Event 이해
// 클릭이나 터치에 따른 반응을 처리하는 하나의 시스템

// 제공자(publisher)
// 사용자의 행동을 기다리다가 구독자에게 알려주는 역할을 진행합니다.

// 구독자(Subscribers)
// 이벤트 제공자에 대한 구독을 통해 사용자의 행동을 전달받아서 반응하는 역할을
// 진행합니다.

// 구독자의 경우, 구독자의 행동 자체를 제공자가 알아야 할 필요는 따로 없음.
// 제공자의 경우, 무분별하게 삭제할 경우 관련 구독자들이 기능을 사용할 수 없음.

// event 사용을 위해선 System 네임스페이스를 사용해야 합니다.

public class EventSample : MonoBehaviour
{
    public event EventHandler OnSpaceEnter;
    // 이벤트 변수의 이름은 보통 On + 동사 / 시제로 만들어집니다.

    // C#에서 제공해주는 delegate 타입
    // EventHandler의 경우 터치나 클릭 등의 이벤트를 관찰하는 용도
    // 매개변수
    // Object sender <- object 타입의 데이터를 전달받을 수 있으며,
    // 이벤트를 발생시킨 대상을 의미합니다.

    // EventArgs e <- 이벤트와 관련된 데이터를 담고 있는 객체를 의미합니다.
    // 해당 값은 EventArgs 또는 해당 자식 클래스가 들어갈 수 있습니다.

    private void Start()
    {
        // 구독 방법
        // 이벤트명 += 형태에 맞는 메소드 이름;
        OnSpaceEnter += Debug_OnSpaceEnter;
    }

    void Update()
    {
        // 1) 이벤트 실행 방식 이벤트명(this, EventArgs.Empty)
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 버튼 클릭
        {
            // Null 검사를 진행하고 실행(이벤트 구독이 안되어 있을 경우에는 실행하면 안되기 때문)
            if(OnSpaceEnter != null)
            {
                OnSpaceEnter(this, EventArgs.Empty);
            }
            // this: 이벤트를 발생시킨 객체(현재 클래스)
            // EventArgs.Empty: 이벤트 실행에 있어 특별히 추가되는 데이터가 없음을 의미합니다.
        }

        // 2) 이벤트 실행 방식 Invoke 함수를 사용하는 방식
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 버튼 클릭
        {
            // ?.를 통해 null이 아닐 때 처리되도록 한다.
            OnSpaceEnter?.Invoke(this, EventArgs.Empty);
            // 또는 OnSpaceEnter?.Invoke(OnSpaceEnter, EventArgs.Empty);

            // int?과 같이 자료형에 ?를 붙이고 Nullable 값 타입으로 사용하는 경우
            // 정수형이지만 null 값도 가질 수 있게 해줍니다. (T?)
            // 타입을 선언할 때 사용됩니다.
            // 값 형태의 타입에 사용됩니다.

            // ?. 의 형태로 쓰이는 경우 Null 조건 연산자로 사용되는 경우
            // 객체가 null이 아닐 때만 멤버에 대한 호출을 진행하도록 설정합니다.
            // 메소드, 속성, 이벤트 등의 호출 시에 사용됩니다.
            // 레퍼런스 형태의 타입 또는 nullable 객체를 대상으로 사용됩니다.

            // >> if(obj != null) 형태의 코드 대신 사용
        }
    }
    private void Debug_OnSpaceEnter(object sender, EventArgs e)
    {
        Debug.Log("<color=yellow>엔터 키 입력 이벤트 실행</color>");
    }
}
