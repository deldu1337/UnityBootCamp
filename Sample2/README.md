# 유니티 스크립트 예제
***
## 유니티의 생명주기
***
유니티에서는 프로그램의 실행부터 종료까지의 작업 영역을 함수로 제공합니다.

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
캐시(Cashe): 자주 사용되는 데이터나 값을 미리 복사해두는 임시 저장소
#### 캐시 사용 의도
1. 시간 지역성: 가장 최근에 사용된 값이 다시 사용될 가능성이 높다.
2. 공간 지역성: 최근에 접근한 주소와 인접한 주소의 변수가 사용될 가능성이 높다.

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
벡터(Vector): 벡터는 크기와 방향을 가진 물리량으로 유니티에서 위치(Position), 이동(Movement), 방향(Direction), 힘(Force)등을 표현할 때 사용합니다.
#### 벡터의 요소
1. X: X축의 값
2. Y: Y축의 값
3. Z: Z축의 값
4. W: 셰이더나 수학 계산 등에서 사용되는 Vector4에서의 W축의 값

#### 벡터의 특징
1. 값 타입(value)으로 참조가 아닌 값 그 자체를 의미합니다.(구조체 struct)
2. 값을 복사할 경우 값 그 자체를 복사하기만 하면 됩니다.
3. 벡터에 대한 계산 보조 기능이 많이 제공됩니다.(magnitude, normalize, Dot, Cross, ...)
4. 벡터는 스택(stack) 영역의 메모리에서 저장됩니다.

#### 벡터 생성
*기본 벡터 생성*
```cs
Vector3 vector = new Vector3();
```
<br>

*x,y,z 축 값을 지정해 생성하는 벡터*
```cs
Vector3 vector = new Vector3(float x, float y, float z);
```
<br>

*방향 벡터(유니티에서 제공하는 기본 벡터)*

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
값: 변수에 데이터가 직접 저장되는 경우
- ex) int a = 5;

참조: 변수에 데이터가 저장된 메모리 주소 값이 저장되는 경우
- ex) VectorSample = new VectorSample(); (클래스는 대표적인 참조 타입)

#### 메모리 저장 영역
프로그램이 실행되기 위해서는 운영체제(OS)가 프로그램의 정보를 메모리에 로드해야 합니다.

프로그램이 실행되는 동안 중앙 제어 장치(CPU)가 코드를 처리하기 위해 메모리가 명령어와 데이터들을 저장하고 있어야 합니다.

컴퓨터 메모리는 바이트(byte) 단위로 번호가 새겨진 선형 공간을 의미합니다.

낮은 주소부터 높은 주소까지 저장되는 영역이 다르게 설정되어 있습니다.
- 낮은 주소: 메모리의 시작 부분

- 높은 주소: 메모리의 끝 부분

#### 대표적인 메모리 공간
|제목|내용|설명|
|------|---|---|
|코드(Code)|실행할 프로그램 코드가 저장되는 영역(텍스트 영역)|프로그램 시작부터 종료까지 계속 남아있는 값, CPU에서 저장된 명령을 하나씩 가져가서 처리합니다.|
|데이터(Data)|프로그램에서 전역 변수, 정적 변수가 저장되는 영역|전역 변수(global): 프로그램 어디서나 접근 가능한 변수<br><br>정적 변수(static): 프로그램 시작부터 할당되고 그 이후부터 종료 시점까지 유지되는 변수, static 키워드가 붙은 변수는 별도의 객체 생성 없이 클래스명.변수명으로 직접 접근하는 것이 가능합니다.|
|힙(Heap)|프로그래머가 직접 저장 공간에 대한 할당과 해제를 진행하는 영역<br>값에 대한 등록도, 값에 대한 제거도 프로그래머가 설계합니다.|참조 타입은 힙에 저장됩니다.<br>C#의 힙 영역의 데이터는 GC에 의해 자동으로 관리됩니다.<br>저장 순서, 정렬에 대한 작업을 따로 신경 쓸 필요가 없습니다.<br>단, 메모리가 크고 GC에 의해 자동으로 처리되는 만큼, 많이 사용되면 그만큼 성능이 저하됩니다.|
|스택(Stack)|프로그램이 자동으로 사용하는 임시 메모리 영역<br>함수 호출 시 생성되는 변수(지역 변수, 매개변수)가 저장되는 영역|함수의 호출이 완료되면 사라지는 데이터, 이 때의 호출 정보 == stack frame(스택 프레임)<br>매우 빠른 속도로 접근이 가능합니다.(할당과 해제의 비용이 사실 상 없음)<br>먼저 들어온 데이터가 누적되고 가장 마지막에 남은 데이터가 먼저 제거되는 방식(LIFO)|

#### 선형 보간과 구면 선형 보간
선형 보간: 두 지점을 선형으로 연결해서 두 지점사이의 위치를 파악하는 방법(Lerp)<br>
구면 선형 보간: 두 지점 사이의 위치를 곡선으로 파악하는 방법(Slerp)

#### Lerp와 Slerp
Lerp: 직선 이동
- 체력 게이지 등이 일정하게 변화하는 경우
```cs
transform.position = Vector3.Lerp(start_position, target.position, t);
```
- start_position -> target.position까지 t에 따라 선형 보간(일정 간격 천천히 직선 이동)합니다.
<br><br>

Slerp: 곡선 이동
- 회전이나 각도의 개념이 필요한 경우<br>
- 3D 회전(쿼터니언) / 벡터 간의 곡선 경로 확인 / 방향 회전이 부드럽게 대상 방향을 바라봐야 할 경우
```cs
transform.position = Vector3.Slerp(start_position, target.position, t);
```
- start_position -> target.position까지 t에 따라 구면 선형 보간(일정 간격 천천히 곡선 이동)합니다.
<br><br>

#### Lerp와 Slerp가 사용되는 경우
1. 단순한 위치 이동? -> Lerp
2. 회전 및 방향 전환? -> Slerp(Vector3.Slerp, Quaternion.Slerp)
3. 자연스러운 카메라의 움직임? -> Slerp

## 유니티의 특성(Attribute)
***
유니티의 Attribute는 코드 메타 데이터를 제공하거나 유니티 에디터와 런타임 동작을 제어할 때 사용됩니다.

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
- [ContextMenuItem("기능으로 표현할 이름", "함수의 이름")]<br>
- message를 대상으로 MessageReset 기능을 에디터로서 사용할 수 있습니다.
```cs
[ContextMenuItem("메시지 초기화","MessageReset")] public string message = "";
```
- 위처럼 ContextMenuItem은 필드 값이기 때문에 뒤에 값이 붙여있는 것과 동일합니다.

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
- 이 변환을 통해 Scene, Prefab, Asset 등에 저장하고 복원하는 작업을 수행할 수 있습니다.

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

*인스펙터 속성 예시 코드*
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

<br>

*키를 입력하면 텍스트에 특정 메세지가 나오도록 하는 예시 코드*
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
**GetComponentInChildren<T>();**
현 오브젝트의 자식으로부터 컴포넌트를 얻어오는 기능<br>
- 위 코드에서는 Canvas(부모) 안에 Text(자식)을 불러오기 위해 사용

#### Player Input
유니티 Input System에서 사용하는 컴포넌트입니다.<br>
InputActions를 등록해 자동으로 매핑합니다.<br>
이벤트 기반으로 할지, 메시지 기반으로 할지, 수동으로 호출할지 설정이 가능합니다.<br>
여러 Player Input도 지원합니다.(멀티 플레이)

*Player Input을 이용한 키 입력 시 움직이는 예시 코드*
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

<br>

*입력 시스템(Legacy)을 이용해서 특정 키를 눌렀을 경우 화면에 있는 아이템을 강화하는 과제 코드*
```cs
using UnityEngine;
using UnityEngine.UI;

public class Practice : MonoBehaviour
{
    public Text[] texts;
    public int rand;
    public int str;
    public int atk;
    public int cnt;
    public int num;

    KeyCode key;

    // Text는 Canvas 안에 있는 값이기 때문에 GetComponent의 자식(Children)으로 가져옴
    private void Start()
    {
        // 텍스트 배열 불러오기
        texts = GetComponentsInChildren<Text>();

        // 아이템 강화 초기화
        str = 0;
        texts[0].text = "쓸데없는 칼";

        // 공격력 초기화
        atk = 0;
        texts[1].text = $"공격력: 50";

        // 강화 수치 초기화
        cnt = 0;
        texts[2].text = $"강화 수치 {cnt}/10";

        // 성공 확률 초기화
        num = 100;
        texts[3].text = $"성공 확률: {num}%";

        // GetComponentInChildren<T>();
        // 현 오브젝트의 자식으로부터 컴포넌트를 얻어오는 기능
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter 키
        {
            rand = Random.Range(1, 101);
            if (rand <= num)
            {
                if (cnt < 3)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"쓸데없는 칼 ( +{str} )";
                }
                else if (cnt >=3 && cnt < 6)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"쓸모있는 칼 ( +{str} )";
                }
                else if (cnt >= 6 && cnt < 8)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"용사의 칼 ( +{str} )";
                }
                else if (cnt >= 8)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"개 쎈 칼 ( +{str} )";
                }
                //// 아이템 강화
                //str++;
                //texts[0].text = $"쓸데없는 칼 ( +{str} )";

                // 공격력 증가
                atk += 5;
                texts[1].text = $"공격력: 50(+{atk})";

                // 강화 수치
                cnt++;
                texts[2].text = $"강화 수치 {cnt}/10";

                // 성공 확률
                num -= 10;
                texts[3].text = $"성공 확률: {num}%";
            }
        }
    }
}
```

## 유니티의 게임 수학
***
### Mathf
유니티에서 수학 관련으로 제공되는 유틸리티 클래스
- 게임 개발에서 사용될 수 있는 수학 연산에 대한 **정적 메소드**와 **상수**를 제공합니다.
> 정적 메소드 : static 키워드로 구성된 해당 메소드는 클래스명.메소드명()으로 사용이 가능합니다.


|정적 메소드명|설명|예시|
|------|---|---|
|Mathf.Abs()|절댓값|Mathf.Abs(-5) = 5|
|Mathf.Ceil()|올림(소수점과 상관없이 값을 올림 처리)|Mathf.Ceil(4.1f) = 5|
|Mathf.Floor()|내림(소수점과 상관없이 값을 내림 처리)|Mathf.Floor(4.9f) = 4|
|Mathf.Round()|반올림(소수점 5이하면 내림, 5초과이면 올림)|Mathf.Round(5.6f) = 6|
|Mathf.Clamp()|값 제한(값, 최소, 최대)|Mathf.Clamp(7, 0, 4) = 4|
|Mathf.Clamp01()|값 제한(최솟값 0 또는 최대값 1로 처리)|Mathf.Clamp01(5) = 1|
|Mathf.Pow()|제곱|Mathf.Pow(2) = 4|
|Mathf.Sqrt()|제곱근(루트)|Mathf.Sqrt(4) = 2|

<br>

|상수명|설명|
|------|---|
|Mathf.PI|원주율|
|Mathf.Infinity|수학적 연산에 의해서 표현할 수 있는 최대의 수를 넘는 경우라면 자동으로 처리되는 값(무한대)|
|Mathf.NegativeInfinity|어떤 숫자도 될 수 없는 음의 방향을 가리키는 개념(음의 무한대)|

#### NaN(Not a Number)
수학적으로 정의되지 않은 계산 결과일 경우 나오는 값
1. 두 값이 NaN일 경우 그 값에 대한 비교는 불가능합니다.
   - float.IsNaN(값)을 통해 해당 값이 NaN인지만 확인이 가능합니다.
2. Infinity = NaN
3. 0/0 과 같이 수학적으로 말이 아예 안되는 값

*NaN 사용 예시 코드*
```cs
Vector3 pos = transform.position;
if(float.IsNaN(pos.x))
{
    Debug.LogWarning("현재 위치에서 NaN 발견, 원점으로 이동합니다.");
    pos = Vector3.zero;
}
```
- NaN에 대한 방어 코드 작성 시 활용 가능

<br>

### 유니티의 삼각 함수
유니티에서 제공해주는 삼각 함수는 주로 회전, 카메라 제어, 곡선, 움직임에 대한 표현으로 사용됩니다.
- 단위를 라디안으로 사용합니다. 1라디안 = 약 57도

|메소드명|설명|
|------|---|
|Mathf.Deg2Rad()|Degree를 Radian으로 변환|
|Mathf.Rad2Deg()|Radian을 Degree로 변환|
|Mathf.Epsilon()|float형에서 0이 아닌 가장 작은 양수 값(미세한 값을 다룰 때 사용)|
|Mathf.Sin()|주어진 각도의 Y 좌표 (세로 방향 위치)|
|Mathf.Cos()|주어진 각도의 X 좌표 (가로 방향 위치)|

#### Degree(각도)
1. Transform.Rotate(0,90,0) -> y축으로 90도만큼 회전
2. Transform.eulerAngles -> transform에서의 (x,y,z) 각도 표현(도)
3. Quaternion.AnglerAxis(45f, Vector3.up) -> 45도만큼 회전
4. Vector3.Angle(A,B); -> A벡터와 B벡터 사이의 각도(도)
5. Quaternion.Angle(A,B) -> 두 회전 간의 차이를 각도(도)
- 유니티 인스펙터에서 보여지는 회전 입력 -> 도
- 유니티의 애니메이션에서 사용하는 회전 속성 -> 도
- 직접적인 회전에 대한 표현(입력, 보여지는 각)

#### Radian
- 수학적으로 사용되는 각도의 단위
- 삼각 함수를 활용한 각 계산
- 반지름과 같은 길이를 가진 호가 가진 중심각 = 1라디안
- 유니티에서 제공해주는 삼각 함수 계산에서 각도 대신 라디안을 요구합니다.

유니티에서는 저 두 기능을 통해 라디안 -> 도 / 도 -> 라디안으로의 변환을 처리합니다.<br>

*플레이어를 45도 방향으로 직진하는 예시 코드*
```cs
using UnityEngine;

// 플레이어를 45도 방향으로 직진하는 코드
public class AngleMove : MonoBehaviour
{
    [SerializeField] float angle_degree; // 각도(도)
    [SerializeField] float speed;        // 속도(움직일 때 쓸 수치)

    void Update()
    {
        var radian = Mathf.Deg2Rad * angle_degree;
        Vector3 pos = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));

        transform.Translate(pos * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            transform.position = Vector3.zero;
        }
    }
}
```
<br>

*여러 종류의 움직임 예시 코드*
```cs
using UnityEngine;
public class Tfunction : MonoBehaviour
{
    // 원형 회전
    public void CircleRotate()
    {
        float angle = Time.time * 90.0f;
        float radian = angle * Mathf.Deg2Rad; // 도에 해당 값을 곱해주면 라디안으로 변환됩니다.

        var x = Mathf.Cos(radian) * 5.0f;
        var y = Mathf.Sin(radian) * 5.0f;

        transform.position = new Vector3(x, y, 0);
    }

    public void ButterFly()
    {
        float t = Time.time * 2;
        float x = Mathf.Sin(t) * 2;
        float y = Mathf.Sin(t * 2f) * 2 * 2;

        transform.position = new Vector3(x, y, 0);
    }

    // Time.time : 프레임이 시작한 순간부터의 시간
    // Time.deltaTime : 프레임이 시작하고 끝나는 시간
    public void Wave()
    {
        var offset = Mathf.Sin(Time.time * 2.0f) * 0.5f;
        transform.position = pos + Vector3.up * offset;
    }

    Vector3 pos; // 좌표(위치)
    private void Start()
    {
        pos = transform.position; // 시작할 때 오브젝트의 위치 이동
    }

    void Update()
    {
        // 마우스 왼쪽 클릭 유지 동안 CircleRotate 호출
        if(Input.GetMouseButton(0))
        {
            CircleRotate();
        }
        // 마우스 오른쪽 클릭 유지 동안 Wave 호출
        if (Input.GetMouseButton(1))
        {
            Wave();
        }
        // 마우스 휠 클릭 유지 동안 ButterFly 호출
        if (Input.GetMouseButton(2))
        {
            ButterFly();
        }
    }
}
```

### 유니티의 회전
1. 오일러 각(Euler Angle)에 의한 회전 -> x, y, z 회전 값
- 유니티 인스펙터에서의 Transform 컴포넌트의 Rotation에 표기된 값(각도 기준)
> ex) Rotation X 120, Y 45, Z 0 --> X축으로 120도 Y축으로 45도 회전되었음을 의미합니다.
2. 쿼터니언(Quaternion) -> 회전을 계산하기에 안정적인 수학적인 방식을 가진 값
- 유니티 엔진 내부에서 실제로 저장되고 계산되는 값

|메소드명|설명|
|------|---|
|transform.Rotate()|회전을 진행시키는 가장 기본적인 코드|
|transform.Rotate(Vector3 eulerAngles)|지정한 축을 기준으로 회전|
|transform.Rotate(float x, float y, float z)|로컬(게임 오브젝트) 기준의 회전|
|transform.Rotate(Vector3 axis, float angle)|특정 축을 중심으로 회전|
|transform.Rotate(Vector3 axis, float angle, Space.World)|월드 또는 로컬 중에 선택|

<br>

*월드 기준으로 z축으로 60도만큼 회전하는 예시 코드*
```cs
transform.Rotate(Vector3.forward, 60f * Time.deltaTime, Space.World);
```
<br>

*게임 오브젝트를 기준으로 회전하는 예시 코드*
```cs
transform.Rotate(new Vector3(20,20,20) * Time.deltaTime);
```
<br>

*절대 좌표를 기준으로 회전하는 예시 코드*
```cs
transform.Rotate(new Vector3(20, 20, 20) * Time.deltaTime, Space.World);
```
<br>


*목표 오브젝트 기준으로 회전하는 예시 코드*
```cs
using UnityEngine;

public class AroundRotate : MonoBehaviour
{
    public Transform pivot; // 회전의 중심점
    public float speed = 1f;

    void Update()
    {
        transform.RotateAround(pivot.position, Vector3.up, speed);
    }
}
```
<br>

#### 쿼터니언으로 처리하는 이유
유니티에서 쿼터니언 -> 오일러 각 변환 시 360도 이상의 회전은 보정될 수 있음
> ex) 380도 회전 -> 20도 회전

**짐벌 락 현상(Gimbal Lock): 오일러 각을 이용해 회전을 표현하는 경우에 발생하는 회전 자유도의 손실**
- 어떤 축이 다른 축과 정렬되는 순간, 한 축의 회전이 무효되면서 회전 방향이 3개가 아닌 2개만 남는 현상

#### 쿼터니언의 구조
4차원 복소수 (x,y,z,w(스칼라)) 하나의 벡터(x,y,z), 하나의 스칼라 w
쿼터니언도 벡터처럼 방향인 동시에 회전의 개념을 가지고 있습니다. (벡터는 위치이면서 방향의 개념)
세 축을 동시에 회전시켜 짐벌 락 현상이 발생하지 않도록 구성되어 있습니다.
쿼터니언은 회전의 원점과 방향을 비교해 회전을 측정할 수 있습니다.

#### 쿼터니언 기능 정리
|메소드명|설명|
|------|---|
|Quaternion.identity|회전 없음|
|Quaternion.Euler(x,y,z)|오일러 각 -> 쿼터니언 변환|
|Quaternion.AngleAxis(angle, axis)|특정 축 기준의 회전|
|Quaternion.LookRotation(forward,upward[default : Vector3.up])|해당 벡터 방향을 바라보는 회전 상태에 대한 return|

- forward : 회전 시킬 방향(Vector3)
- upward : 방향을 바라보고 있을 때의 위 부분을 설정

*회전 값 적용 예시 코드*
```cs
transform.rotation = Quaternion.Euler(x,y,z);
```
<br>

#### 회전에 대한 보간
|메소드명|설명|
|------|---|
|Quaternion.Slerp(from, to, t)|구면 선형 보간|
|Quaternion.Lerp(from, to, t)|선형 보간|
|Quaternion.RotateTowards(from, to, maxDegree)|일정 각도만큼 점진적으로 회전을 처리합니다.|

#### transform.LookAt() vs Quaternion.LookRotation()
둘다 특정 방향을 보게하는 코드
1. LookAt(target) : 추가 회전 보간이나 제어가 어렵고, 설정해 준 값을 기준으로 transform.rotation을 자동으로 설정해주는 기능(내부에서 Quaternion.LookRotation()을 사용하는 방식)
2. LookRotation(direction) 의 경우는 회전 값만 계산하고 직접적인 적용은 하지 않습니다.

### RayCast
레이캐스트는 가상의 **레이저 빔**을 원점에서부터 레이에 따라 씬 안의 콜라이더에 충돌할 때까지 보냅니다. 그 다음 오브젝트와 RaycastHit 오브젝트의 충돌된 점에 대한 정보를 반환합니다.
- 시작 위치에서 특정 방향으로 가상의 광선을 썼을 때 부딪히는 오브젝트가 있는지를 판단

*RayCast 사용 예시 코드*
```cs
using UnityEngine;
// 1) 특정 오브젝트를 충돌 범위에서 제외하는 기능
// 2) 특정 오브젝트에 대한 충돌 판정을 확인하는 코드

public class RayCastSample : MonoBehaviour
{
    RaycastHit hit; // 충돌 감지용 변수

    // ref : 변수를 참조로 전달, 변수가 메소드 안에서 변경될 수 있음을 알리는 용도
    // out : 변수를 참조로 전달, 변수 전달 과정에서 변수에 대한 초기화를 진행할 필요가 없음.

    // Physics.RayCast(Vector3 origin, Vector3 direction, out RayCastHit hitinfo,
    // float maxDistance, int layerMask);

    // origin 방향에서 direction 방향으로 maxDistance 길이의 광선을 발사합니다.

    // hitinfo는 충돌체에 대한 정보를 의미합니다.

    const float length = 20.0f; // 길이 변경 5 -> 20

    void Start()
    {
        // 한 번에 여러 개의 레이캐스트 충돌 처리

        // 선 그리기
        Debug.DrawRay(transform.position, transform.forward * length, Color.red);

        // 레이어 마스크 설정하기
        int ignoreLayer = LayerMask.NameToLayer("Red"); // 충돌 시키고 싶지 않은 레이어
        int layerMask = ~(1 << ignoreLayer); // 비트 반전

        // 충돌체 설정
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, length, layerMask);
        // RaycastAll : 한 방향으로 쏜 레이가 충돌한 충돌체를 배열로 return 합니다.

        // 반복문으로 콜라이더 체크
        for (int i = 0; i < hits.Length; i++) // hits의 길이(개수)만큼 반복을 진행합니다.
        {
            Debug.Log(hits[i].collider.name + "를 HIT 했습니다.");
            hits[i].collider.gameObject.SetActive(false);
            // hits 배열의 i번째 값의 충돌체가 가진 게임 오브젝트를 비활성화합니다.
        }
    }

    //void Update()
    //{
    //    // 오브젝트 위치에서 정면으로 length만큼의 길이에 해당하는 디버깅 광선을 쏘는 코드
    //    // 주로 레이캐스트 작업에서 레이가 안보이기 때문에 보여주는 용도로 사용합니다.
    //    Debug.DrawRay(transform.position, transform.forward * length, Color.red);

    //    // 1. 충돌 시키고 싶지 않은 레이어에 대한 변수 설정
    //    int ignoreLayer = LayerMask.NameToLayer("Red"); // 충돌 시키고 싶지 않은 레이어
    //    // 2. ~(1 << LayerMask.NameToLayer("레이어 이름")) 해당 레이어 이외의 값
    //    int layerMask = ~(1 << ignoreLayer); // 비트 반전

    //    // ex) 만약에 Red 레이어랑 Blue 레이어를 둘다 제외하고 싶은 경우
    //    //int ignoreLayers = (1 << LayerMask.NameToLayer("Red")) | (1 << LayerMask.NameToLayer("Blue"));
    //    //int layerMasks = ~ignoreLayers;

    //    // 왼쪽 버튼을 눌렀을 경우 레이 발사
    //    //if (Input.GetMouseButton(0))
    //    //{
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, length, layerMask))
    //    {   
    //        Debug.Log("히히히 발싸");
    //        Debug.Log(hit.collider.name);
    //        hit.collider.gameObject.SetActive(false);

    //        // 레이어마스크는 비트 마스크이며, 각 비트는 하나의 레이어를 의미합니다.
    //        // 1 << n은 n번째 레이어만 포함하는 마스크를 의미합니다.
    //        // ~에 의해 작성된 ~(1<<n)은 해당 레이어를 제외한 모든 레이어를 의미합니다.
    //    }
    //    //}

    //    // 오브젝트 위치에서 정면으로 length만큼의 길이에 해당하는 디버깅 광선을 쏘는 코드
    //    // 주로 레이캐스트 작업에서 레이가 안보이기 때문에 보여주는 용도로 사용합니다.
        
    //}
}
```
