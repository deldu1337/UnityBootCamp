# 유니티 스크립트 예제
***
## 유니티의 생명주기
***
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
***
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
***
### 벡터(Vector): 벡터는 크기와 방향을 가진 물리량으로 유니티에서 위치(Position), 이동(Movement), 방향(Direction), 힘(Force)등을 표현할 때 사용합니다.
#### 벡터의 요소
> 1. X: X축의 값
> 2. Y: Y축의 값
> 3. Z: Z축의 값
> 4. W: 셰이더나 수학 계산 등에서 사용되는 Vector4에서의 W축의 값

#### 벡터의 특징
> 1. 값 타입(value)으로 참조가 아닌 값 그 자체를 의미합니다.(구조체 struct)
> 2. 값을 복사할 경우 값 그 자체를 복사하기만 하면 됩니다.
> 3. 벡터에 대한 계산 보조 기능이 많이 제공됩니다.(magnitude, normalize, Dot, Cross, ...)
> 4. 벡터는 스택(stack) 영역의 메모리에서 저장됩니다.

#### 벡터 생성
기본 벡터 생성
```cs
Vector3 vector = new Vector3();
```
<br>

x,y,z 축 값을 지정해 생성하는 벡터
```cs
Vector3 vector = new Vector3(float x, float y, float z);
```
<br>

방향 벡터(유니티에서 제공하는 기본 벡터)
|방향 벡터|값|
|------|---|
|Vector3.up|(0,1,0)|
|Vector3.down|(0,-1,0)|
|Vector3.left|(-1,0,0)|
|Vector3.right|(1,1,0)|
|Vector3.forward|(0,0,1)|
|Vector3.back|(0,0,-1)|
|Vector3.one|(1,1,1)|
<br>

#### 값(Value) vs 참조(Reference)
> 값: 변수에 데이터가 직접 저장되는 경우
> > ex) int a = 5;

> 참조: 변수에 데이터가 저장된 메모리 주소 값이 저장되는 경우
> > ex) VectorSample = new VectorSample(); (클래스는 대표적인 참조 타입)

#### 메모리 저장 영역
> 프로그램이 실행되기 위해서는 운영체제(OS)가 프로그램의 정보를 메모리에 로드해야 합니다.

> 프로그램이 실행되는 동안 중앙 제어 장치(CPU)가 코드를 처리하기 위해 메모리가 명령어와 데이터들을 저장하고 있어야 합니다.

> 컴퓨터 메모리는 바이트(byte) 단위로 번호가 새겨진 선형 공간을 의미합니다.

> 낮은 주소부터 높은 주소까지 저장되는 영역이 다르게 설정되어 있습니다.
> > 낮은 주소: 메모리의 시작 부분

> > 높은 주소: 메모리의 끝 부분

#### 대표적인 메모리 공간
|제목|내용|설명|
|------|---|---|
|코드(Code)|실행할 프로그램 코드가 저장되는 영역(텍스트 영역)|프로그램 시작부터 종료까지 계속 남아있는 값, CPU에서 저장된 명령을 하나씩 가져가서 처리합니다.|
|데이터(Data)|프로그램에서 전역 변수, 정적 변수가 저장되는 영역|전역 변수(global): 프로그램 어디서나 접근 가능한 변수<br><br>정적 변수(static): 프로그램 시작부터 할당되고 그 이후부터 종료 시점까지 유지되는 변수, static 키워드가 붙은 변수는 별도의 객체 생성 없이 클래스명.변수명으로 직접 접근하는 것이 가능합니다.|
|힙(Heap)|프로그래머가 직접 저장 공간에 대한 할당과 해제를 진행하는 영역<br>값에 대한 등록도, 값에 대한 제거도 프로그래머가 설계합니다.|참조 타입은 힙에 저장됩니다.<br>C#의 힙 영역의 데이터는 GC에 의해 자동으로 관리됩니다.<br>저장 순서, 정렬에 대한 작업을 따로 신경 쓸 필요가 없습니다.<br>단, 메모리가 크고 GC에 의해 자동으로 처리되는 만큼, 많이 사용되면 그만큼 성능이 저하됩니다.|
|스택(Stack)|프로그램이 자동으로 사용하는 임시 메모리 영역<br>함수 호출 시 생성되는 변수(지역 변수, 매개변수)가 저장되는 영역|함수의 호출이 완료되면 사라지는 데이터, 이 때의 호출 정보 == stack frame(스택 프레임)<br>매우 빠른 속도로 접근이 가능합니다.(할당과 해제의 비용이 사실 상 없음)<br>먼저 들어온 데이터가 누적되고 가장 마지막에 남은 데이터가 먼저 제거되는 방식(LIFO)|

#### 선형 보간과 구면 선형 보간
> 선형 보간: 두 지점을 선형으로 연결해서 두 지점사이의 위치를 파악하는 방법(Lerp)<br>
> 구면 선형 보간: 두 지점 사이의 위치를 곡선으로 파악하는 방법(Slerp)

#### Lerp와 Slerp
> Lerp: 직선 이동
> > 체력 게이지 등이 일정하게 변화하는 경우
```cs
transform.position = Vector3.Lerp(start_position, target.position, t);
```
start_position -> target.position까지 t에 따라 선형 보간(일정 간격 천천히 직선 이동)합니다.
<br><br>

> Slerp: 곡선 이동
> > 회전이나 각도의 개념이 필요한 경우<br>
> > 3D 회전(쿼터니언) / 벡터 간의 곡선 경로 확인 / 방향 회전이 부드럽게 대상 방향을 바라봐야 할 경우
```cs
transform.position = Vector3.Slerp(start_position, target.position, t);
```
start_position -> target.position까지 t에 따라 구면 선형 보간(일정 간격 천천히 곡선 이동)합니다.
<br><br>

#### Lerp와 Slerp가 사용되는 경우
1. 단순한 위치 이동? -> Lerp
2. 회전 및 방향 전환? -> Slerp(Vector3.Slerp, Quaternion.Slerp)
3. 자연스러운 카메라의 움직임? -> Slerp

## 유니티의 특성(Attribute)
***
### 유니티의 Attribute는 코드 메타 데이터를 제공하거나 유니티 에디터와 런타임 동작을 제어할 때 사용됩니다.
|Attribute|설명|
|------|---|
|SerializeField|직렬화|
|HideInInspector|인스펙터에서 숨김|
|Range(min,max)|슬라이더로 표현|
|Tooltip("")|마우스 오버 시 설명 표시|
|Header("")|섹션 헤더 표시|
|Space(float)|공백 추가하기|
|TextArea(min, max)|멀티라인 텍스트 입력으로 표시|
|Multiline(lines)|직렬화|
|ContextMenu("")|인스펙터에서 숨김|
|ContextMenuItem("","")|슬라이더로 표현|
|RequireComponent(typeof(T))|마우스 오버 시 설명 표시|
|AddComponentMenu("/")|섹션 헤더 표시|
|Default Execution Order(int order)|공백 추가하기|

#### 사용 목적
사용자가 에디터를 더 직관적으로, 편의적으로 사용하기 위해서<br>

### 에디터 속성
#### AddComponentMenu("그룹이름/기능이름")
Editor의 Component에 메뉴를 추가하는 기능
```cs
public class MenuAttributes : MonoBehaviour
{
    [ContextMenuItem("메시지 초기화","MessageReset")]
    public string message = "";
    public void MessageReset()
    {
        message = "";
    }

    [ContextMenu("경고 메시지 호출")]
    public void MenuAttributesMethod()
    {
        Debug.LogWarning("경고 메시지!");
    }
}
```
> [ContextMenuItem("기능으로 표현할 이름", "함수의 이름")]<br>
> message를 대상으로 MessageReset 기능을 에디터로서 사용할 수 있습니다.
```cs
[ContextMenuItem("메시지 초기화","MessageReset")] public string message = "";
```
> 위처럼 ContextMenuItem은 필드 값이기 때문에 뒤에 값이 붙여있는 것과 동일합니다.

#### [ExecuteInEditMode]
Play를 누르지 않아도 Editor 내에서 Update 등에 설계한 기능들을 실행해 볼 수 있습니다.
```cs
[ExecuteInEditMode]
public class EditMenuSample : MonoBehaviour
{
    void Update()
    {
        // 에디터에서만 실행해보는 코드
        if(!Application.isPlaying)
        {
            // 현재 오브젝트 y축을 0으로 고정하는 코드
            Vector3 pos = transform.position;
            pos.y = 0;
            transform.position = pos;
            Debug.Log("Editor Test...(이 스크립트를 낀 오브젝트는 y축이 0으로 고정됩니다.)");
        }
    }
}
```

### 인스펙터 속성
#### [Serializable]
직렬화(Serialization): 데이터를 저장 가능한 형식으로 변환하는 작업
```cs
[Serializable]
public class Items
{
    public string name;
    public string description;
}
```
> 이 변환을 통해 Scene, Prefab, Asset 등에 저장하고 복원하는 작업을 수행할 수 있습니다.

**직렬화 조건**
1. public 혹은 [SerializeField]
2. static이 아닌 경우
3. 직렬화 가능한 데이터 타입인 경우

**직렬화가 가능한 데이터**
1. 기본 데이터(int, float, bool, string, ...)
2. 유니티에서 제공해주는 구조체(Vector3, Quaternion, Color)
3. 유니티 객체 참조(GameObject, Transform, Material)
4. [Serializable] 속성이 붙은 클래스
5. 배열 / 리스트

**직렬화 불가능한 데이터**
1. Dictionary<K,V>
2. Interface (인터페이스)
3. static 키워드가 붙은 필드
4. abstract(추상 클래스) 키워드가 붙은 필드

#### [HideInInspector]
인스펙터에 공개하기 싫은 값에 대한 설정
```cs
[HideInInspector]
public int value = 5;
```

#### [Space()]
Space(수치): 해당 수치만큼 높이 간격 설정
```cs
[Space(30)]
```

#### [TextArea()]
TextArea(기본 표시 줄, 최대 줄): 긴 문자열을 여러 줄로 적을 수 있는 설정
```cs
[TextArea(5,10)]
```
#### 유니티에서의 Struct와 Class
**Struct**
GC 필요 없음(작은 데이터의 묶음을 자주 할당/복사하는 개념에서 활용 ex. Vector3)
<br>
**Class**
객체를 위한 설계도(속성과 기능) / 유니티에서는 class 사용 권장(안정성이 높음)

#### 인스펙터 속성 예시 코드
```cs
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum Job
{
    전사,
    도적,
    궁수,
    마법사
}

[Serializable]
public class Player
{
    public string name;
    public Job job;
    public int HP;
    public int MP;
    public int ATK;
    public int INT;
    public int DEX;
    [HideInInspector]
    private int gold;
}

[Serializable]
public class Item
{
    public string name;
    public int level;
    public int HP;
    public int MP;
    public int ATK;
    public int INT;
    public int DEX;
    public int sell;
}

public class PlayerStat : MonoBehaviour
{
    public Player player = new Player();

    [Space(10)]
    [Header("Infomation")]
    public int level;
    public int exp;

    [Space(20)]
    [TextArea(5, 10)]
    public string intro;

    [Space(20)]
    public List<Item> item;

    [Space(10)]
    [Header("Setting")]
    [Range(0, 100)] public int sound;
    [Range(0, 100)] public float mouse;
    [Range(0, 100)] public int FieidOfVision;
}
```

## 유니티의 입력 시스템
***
### 입력 관리자(Input Manager)
Edit -> Project Settings -> Input을 통해 확인할 수 있습니다.
#### 입력 문법
Axes 키에 대한 입력(인풋 매니저에 설정되어있는 키에 대한 입력을 받을 수 있습니다.)<br>
음수는 negative, 양수는 positive
|이름|설명|
|------|---|
|Input.GetAxis("키 이름")|-1 ~ 1까지의 실수를 반환합니다.|
|Input.GetAxisRaw("키 이름")|-1, 1, 0을 반환합니다.|
<br>
Button에 대한 입력(KeyCode를 사용합니다.)

|이름|설명|
|------|---|
|Input.GetButton("버튼 이름")|마우스 버튼이 눌렸는지를 확인합니다.|
|Input.GetMouseButtonDown()|마우스 버튼이 처음 눌렀을 때를 확인합니다.|
|Input.GetMouseButtonUp()|마우스 버튼이 떼어졌을 때를 확인합니다.|
|Input.mousePosition|마우스의 화면 내의 위치를 얻습니다.|
<br>
마우스 입력( 0 : 왼쪽 1: 오른쪽 2: 중앙(휠) )

|이름|설명|
|------|---|
|Input.GetMouseButton()|마우스 버튼이 눌렸는지를 확인합니다.|
|Input.GetMouseButtonDown()|마우스 버튼이 처음 눌렀을 때를 확인합니다.|
|Input.GetMouseButtonUp()|마우스 버튼이 떼어졌을 때를 확인합니다.|
|Input.mousePosition|마우스의 화면 내의 위치를 얻습니다.|

### Unity Input System
유니티 2019 버전 이후부터 New Input System을 새로 만들어 해당 시스템을 사용합니다. 현재의 legacy 코드는 
기존과의 호환성, 간단한 기능 구현에 사용됩니다.
이전 버전에서는 패키지 매니저에 따라 Input System을 별도로 설치해야 하며, 현재의 버전에서는 자동으로 설치가 
되어있습니다.
1. 멀티 플랫폼 지원(다양한 디바이스에서 동일한 코드로 입력 보장)
2. 동적 입력 바인딩(사용자가 게임 내에서 키 매핑 수정 가능)
3. 이벤트 기반의 입력(기존의 레거시는 폴링 방식, 현재의 인풋 시스템은 이벤트 기반)
4. 입력 액션 시스템(액션을 통해 다양한 입력에 대한 처리를 한 곳에서 관리 가능)

#### 기존 시스템과의 성능 차이점
|Legacy|New|
|------|---|
|매 프레임 마다 입력 확인(폴링)|입력 발생 시에만 처리(이벤트)|
|입력 없는 장치도 체크될 우려 있음|정해진 특정 이벤트 때만 실행|
|메모리 사용 적음|메모리 사용량 많음|
|장치별로 개별적 처리가 필요함|입력 액션에서의 통일성, 다양한 디바이스 제공|
|유지보수 어려움|유연하고 확장성 높음|

#### 키를 입력하면 텍스트에 특정 메세지가 나오도록 하는 예시 코드
```cs
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LegacyExample : MonoBehaviour
{
    public Text text;

    KeyCode key;

    // Text는 Canvas 안에 있는 값이기 때문에 GetComponent의 자식(Children)으로 가져옴
    private void Start()
    {
        text = GetComponentInChildren<Text>();
        // GetComponentInChildren<T>();
        // 현 오브젝트의 자식으로부터 컴포넌트를 얻어오는 기능
    }

    void Update()
    {
        // 배열과 같은 묶음으로 관리되는 데이터를 순차적으로 조사하는 코드
        // KeyCode 형태의 데이터 전체를 조사합니다.
        foreach(KeyCode Key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(Key))
            {
                switch (Key)
                {
                    case KeyCode.Escape:
                        text.text = "";
                        break;
                    case KeyCode.Space:
                        text.text = "pata";
                        break;
                    case KeyCode.Return:
                        text.text = "pong";
                        break;
                }
            }
        }
    }
}
```
> GetComponentInChildren<T>();
> > 현 오브젝트의 자식으로부터 컴포넌트를 얻어오는 기능<br>
> > 위 코드에서는 Canvas(부모) 안에 Text(자식)을 불러오기 위해 사용

#### Player Input
유니티 Input System에서 사용하는 컴포넌트입니다.<br>
InputActions를 등록해 자동으로 매핑합니다.<br>
이벤트 기반으로 할지, 메시지 기반으로 할지, 수동으로 호출할지 설정이 가능합니다.<br>
여러 Player Input도 지원합니다.(멀티 플레이)

#### Player Input을 이용한 키 입력 시 움직이는 예시 코드
```cs
using UnityEngine;
using UnityEngine.InputSystem;

// RequireComponent(typeof(T))는 이 스크립트를 컴포넌트로
// 사용할 경우 해당 오브젝트는 반드시 T를 가지고 있어야 합니다.
// 없는 경우라면 자동으로 등록해주고, 이 코드가 존재하는 한
// 에디터에서 게임 오브젝트는 해당 컴포넌트를 제거할 수 없습니다.
[RequireComponent(typeof(PlayerInput))]
public class InputSystemExample : MonoBehaviour
{
    // 현재 Action Map : Sample
    //      Action : Move
    //      Type : Value
    //      Compos : Vector2
    //      Binding : 2D Vector(WASD)
    private Vector2 moveInputValue;
    private float speed = 3.0f;

    // Send Messages로 사용되는 경우
    // 특정 키가 들어오면, 특정 함수로 호출합니다.
    // 함수 명은 On + Actions name, 현재 만든 Actions의 이름 Move라면
    // 함수명은 OnMove가 됩니다.
    void OnMove(InputValue value)
    {
        moveInputValue = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInputValue.x, 0, moveInputValue.y); // 좌우 x축, 상하 z축

        transform.Translate(move * speed * Time.deltaTime);
    }
}
```
