# 유니티 스크립트 예제
***
## 유니티의 생명주기
> 유니티에서는 프로그램의 실행부터 종료까지의 작업 영역을 함수로 제공합니다.

|제목|내용|설명|역할|
|------|---|---|---|
|Awake()|씬이 시작될 때 한 번만 호출|해당 스크립트가 비활성화 되어 있어도 이 위치의 작업은 실행되며, 코루틴 사용 불가능|주로 변수 초기화, 값 참조할 때 사용|
|OnEnable()|오브젝트 또는 스크립트가 활성화 될 때 호출|이벤트에 대한 연결에 사용하며, 코루틴 사용 불가능, 반대의 개념은 OnDisable()|활성화 시 구현되어야 하는 기능에 사용|
|Start()|모든 스크립트의 Awake()가 다 실행된 이후 호출|Awake()를 사용해도 무방하며 상황에 따라 설계, 코루틴 사용 가능|게임 로직에 대한 실행, 초기화된 데이터를 기반으로 작업 수행할 때 사용|
|Update()|프레임마다 호출(1초에 약 60번)|프로그램 내에서 핵심적으로 자주 사용되는 메인 로직을 짤 때 사용되며, 가장 많이 사용되는 영역|계산에 대한 보정 값, 키 입력 기반 움직임 등의 정규화/단위 벡터를 이용한 작업 처리
|FixedUpdate()|프레임마다 호출되는 것이 아닌, Fixed TimeStep이라는 설정 값에 의해 호출(default: 0.02초)|일정한 발생 주기가 보장되어야 하는 로직을 구현할 때 사용|물리 연산(Rigidbody)이 적용된 오브젝트에 대한 조정|
|LateUpdate()|모든 Update 함수(Update, FixedUpdate)가 호출된 이후 마지막으로 호출|후처리 작업에 사용|렌더링 이후 동작하는 카메라 처리와 같은 후처리 작업|

[이벤트 함수의 실행 순서](https://docs.unity3d.com/kr/current/Manual/ExecutionOrder.html)

<br>

## 오브젝트 캐싱(Object Cashing)
### 캐시(Cashe): 자주 사용되는 데이터나 값을 미리 복사해두는 임시 저장소
#### 캐시 사용 의도
> 1. 시간 지역성: 가장 최근에 사용된 값이 다시 사용될 가능성이 높다.
> 2. 공간 지역성: 최근에 접근한 주소와 인접한 주소의 변수가 사용될 가능성이 높다.

*예시 코드*
```cs
using UnityEngine;
public class Sample3 : MonoBehaviour
{
    Rigidbody rb;
    Vector3 pos;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 캐싱(cashing)
    }

    void Update()
    {
        GetComponent<Rigidbody>().AddForce(pos * 5); // 프레임마다 호출
    }
}
```

<br>

## 유니티의 벡터
### 벡터(Vector): 벡터는 크기와 방향을 가진 물리량으로 유니티에서 위치(Position), 이동(Movement), 방향(Direction), 힘(Force)등을 표현할 때 사용합니다.
#### 벡터의 요소
> 1. X: X축의 값
> 2. Y: Y축의 값
> 3. Z: Z축의 값
> 4. W: 셰이더나 수학 계산 등에서 사용되는 Vector4에서의 W축의 값

