# 유니티 스크립트 예제
***
## 유니티 프리팹
***
프리팹은 유니티에서 미리 만들어놓은 게임 오브젝트의 설계도입니다.<br>
프리팹을 통해 동일한 오브젝트를 여러번 반복 생성할 수 있습니다.<br>
오브젝트의 모든 설정이 저장됩니다.<br>
만들어둔 프리팹은 인스턴스로 복사해 사용할 수 있습니다.

### 오브젝트 생성 및 파괴

|메소드명|내용|
|------|---|
|Instantiate(오브젝트)|해당 오브젝트를 원본과 동일하게 생성합니다.|
|Instantiate(오브젝트, 위치, 회전)|해당 위치(Vector)와 회전 값(Quaternion)을 설정해 생성합니다.|
|Instantiate(오브젝트, 부모)|생성된 오브젝트의 부모(Transform)을 설정할 수 있으며, 부모의 
자식으로서 생성됩니다. ex) Spawn 오브젝트가 부모이고 Bullet이 오브젝트일 경우 Spawn-Bullet|
|Destroy(게임 오브젝트)|이 스크립트가 붙은 게임 오브젝트를 삭제합니다.|
|Destroy(게임오브젝트, 시간)|설정한 시간 뒤 게임 오브젝트를 삭제합니다.|
|Destroy(컴포넌트)|이 스크립트가 붙은 게임 오브젝트의  특정 컴포넌트만 삭제합니다.|

<br>

*오브젝트를 생성하고 파괴하는 예시 코드*
```cs
using UnityEngine;
// 오브젝트 복사본 1개 (prefab), 내부에서 저장하는 값 1개 (spawned)

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    private GameObject spawned;
    public float delay = 3.0f;

    void Start()
    {
        if(prefab != null)
        {
            spawned = Instantiate(prefab);
            // 생성 코드 Instantiate
            // 1. Instantiate(prefab): 해당 프리팹의 정보에 맞게 위치와 회전 값 등이 설정되고 생성됩니다.
            // 2. Instantiate(prefab, transform.position, Quaternion.identity);
            // --> 해당 프리팹을 설정하고, 위치와 회전 값의 정보대로 오브젝트의 위치와 회전 값을 설정합니다.
            // 3. Instantiate(prefab, parent);
            // 오브젝트를 생성하고, 그 오브젝트를 전달한 위치의 자식으로써 등록합니다.
            // 4. Instantiate(prefab, transform.position, Quaternion.identity, parent);

            spawned.name = "생성된 오브젝트";
            // 생성된 오브젝트의 이름을 변경하는 코드

            //spawned.AddComponent<Rigidbody>();
            // 그 후 생성된 오브젝트에 컴포넌트를 추가합니다.

            Debug.Log(spawned.name + "이 생성되었습니다.");

            Destroy(spawned, delay); // delay 시간 이후 오브젝트 파괴
        }
        else
        {
            Debug.LogWarning("등록된 프리팹이 따로 없습니다.");
        }
    }
}
```

<br>

## 유니티의 코루틴
***
유니티의 코루틴은 프레임 간 또는 시간 지연이 필요한 비동기 작업을 간단하게 처리할 수 있게 지원되는 기능입니다.<br>
코루틴은 중간에 실행을 일시적으로 멈췄다가 다시 실행을 이어갈 수 있는 함수를 의미합니다.<br>
코루틴의 실행 코드인 StartCoroutine()은 MonoBehaviour에서만 사용이 가능합니다.<br>

#### 코루틴 사용이 고려되는 상황
1. 특정 시간 후의 이벤트
2. 일정 시간 간격으로 특정 작업 수행
3. 부드러운 애니메이션 연출
4. 로딩 화면 연출(AsyncOperation)
5. 조건 만족 전까지의 대기 상황

#### 코루틴 이해
유니티는 IEnumerator 형태의 return 값을 가지는 메소드를 코루틴으로 인식합니다.<br>
코루틴 함수에 대한 함수를 호출하면 IEnumerator 객체가 생성되며 내부의 MoveNext()가 호출됩니다.<br>
작성된 메소드 내의 yield return까지의 실행이 진행되며, 해당 값을 return하고 지연 설정한 시간 만큼 멈춰있다가 다음 MoveNext() 호출이 이어서 실행되는 방식입니다.<br>

*코루틴 사용 예시 코드*
```cs
using System.Collections;
using UnityEngine;

public class CoroutineSample : MonoBehaviour
{
    // 적용할 타겟
    public GameObject target;

    // 변화 시간
    float duration = 5.0f;

    // 바꾸고 싶은 색
    public Color color;

    //Coroutine coroutine;

    void Start()
    {
        //coroutine = StartCoroutine(ChangeColor());
        //StopCoroutine(coroutine);
        //StopCoroutine("ChangeColor");
        //StopAllCoroutines(); // 모든 코루틴에 대한 중지(현재 게임 오브젝트에서 실행중인)

        // 타겟이 설정되어있다면
        if(target != null)
        {
            StartCoroutine(ChangeColor());
            // StartCoroutine(메소드명()); IEnumerator 형태의 메소드를 코루틴으로 시작합니다.
            // 코드 작성 과정에서 메소드가 결정되어 안전함.
            // 메소드 호출은 컴파일 과정에서 확인되기에 찾아 실행하는 시간이 문자열보다 적게 듭니다.
            
            //StartCoroutine("ChangeColor");
            // StartCoroutine("메소드명"); 문자열을 통해 매개변수가 없는 코루틴을 호출할 수 있습니다.
            // 내부적으로 메소드의 이름을 문자열로 전달하고, 런타임에서 찾아서 실행하는 방식(리플렉션)
            // 타임 체크를 하지 않아서 잘못된 이름을 쓰면 런타임 오류 발생
        }
        else
        {
            Debug.LogWarning("현재 타겟이 등록되지 않은 상태입니다.");
        }
    }

    IEnumerator ChangeColor()
    {
        // 타겟으로부터 렌더러 컴포넌트에 대한 값을 얻어옵니다.
        var targetRenderer = target.GetComponent<Renderer>();

        // 조사한 타겟의 렌더러가 없을 경우
        if (targetRenderer == null)
        {
            Debug.LogWarning("렌더러를 얻어오지 못했습니다.(NULL)");
            // 작업 중단
            yield break;
        }

        // 이 위치의 코드는 정상적으로 렌더러가 있을 경우에 실행되는 위치
        float time = 0.0f;

        // 타겟의 렌더러가 가진 머티리얼의 색깔을 사용
        var start = targetRenderer.material.color;
        var end = color;

        // 반복 작업
        // 코루틴 내에서 반복문을 설계하면, yield에 의해 빠져나갔다가 다시 돌아와서 반복문을 실행하게 됩니다.
        while (time < duration) // 변화 시간만큼 작업
        {
            time += Time.deltaTime;
            var value = Mathf.PingPong(time, duration) / duration;
            // Mathf.PingPong(a,b);
            // 주어진 값을 a와 b 사이에서 반복되는 값을 생성합니다.(기본적인 왕복 운동)
            // 약 0에서 1까지의 변화 값이 계산됩니다.
            // 정규화 작업을 진행한 이유 : color는 0부터 1까지의 값

            targetRenderer.material.color = Color.Lerp(start, end, value);
            // 색상에 대한 부드러운 변경

            yield return null;
            Debug.Log("한 프레임이 끝났어요");
        }
    }
    // 코루틴 정지 기능
    //StopCoroutine(코루틴 객체);
}
```

<br>

### IEnumerator
IEnumerator는 C#에서 컬렉션(배열이나 리스트 등)을 순회할 때 사용되는 인터페이스 입니다.<br>

|메소드명|내용|
|------|---|
|bool MoveNext()|순회하고 있는 컬렉션이 다음 요소가 존재하면 true, 아니면 false를 return합니다.|
|object Current {get; }|현재의 위치의 요소를 return 합니다.|
|void Reset()|순회를 처음 위치로 설정합니다.|

#### IEnumerator와 IEnumerable 차이
**IEnumerator**
- 반복을 관리하는 인터페이스입니다.
- 컬렉션에서 하나씩 항목들을 반환하고, 그 상태를 관리하는 역할을 수행합니다.
  - MoveNext()를 통해서 다음 값을 접근
  - Current를 통해서 현재 값 확인
  - Reset()을 통해서 리셋 기능 처리

**IEnumerable**
- 반복 가능한 컬렉션을 나타내는 인터페이스입니다.
- 이 기능을 구현한 클래스는 반복할 수 있는 객체가 되며 foreach 등에서 순차적인 탐색을 진행할 수 있게 됩니다.
- 해당 인터페이스를 구현하기 위해서는 GetEnumerator() 메소드를 제공하고, 사용자가 구현해야 합니다.

### yield return
C#에서의 yield는 Enumerator 메소드를 작성할 때 사용하는 키워드로 함수 실행 중간에 값을 반환하고 실행을 멈추며, 다음 호출에 이어서 실행을 할 수 있게 해줍니다.(코루틴의 핵심)<br>

|메소드명|내용|
|------|---|
|yield return 값|현재 값을 호출자에게 return하고 실행을 일시적으로 멈추며, 다시 호출이 진행될 경우 멈췄던 위치부터의 실행이 다시 실행됩니다.|
|yield break;|반복 작업을 종료하고 함수의 실행을 끝냅니다.|

유니티의 코루틴 함수는 IEnumerator를 반환하며 yield return을 통해 호출 시점마다 IEnumerator 객체가 생성됩니다.<br>

*IEnumerator와 yield return를 사용한 예시 코드*
```cs
using System;
using System.Collections;
using UnityEngine;

public class IEnumeratorSample : MonoBehaviour
{
    class CustomCollection : IEnumerable
    {
        int[] numbers = { 6,7,8,9,10 };

        // GetEnumerator를 통해 순회 가능한 데이터를 IEnumerator 형태의 객체로 변환합니다.
        public IEnumerator GetEnumerator()
        {
            for(int i = 0;i<numbers.Length;i++)
            {
                yield return numbers[i];
            }
        }
    }

    int[] numbers = {1, 2, 3, 4, 5};

    void Start()
    {
        // Numbers라는 배열을 순회할 수 있는 IEnumerator 형태의 데이터로 변환합니다.
        IEnumerator enumerator = numbers.GetEnumerator();

        while (enumerator.MoveNext())
        {
            Debug.Log(enumerator.Current);
        }

        CustomCollection collection = new CustomCollection(); // 커스텀 컬렉션 생성

        // foreach는 순환 가능한 데이터를 순차적으로 작업할 때 사용하는 for문임으로 보기가 간결해집니다.
        foreach(int number in collection)
        {
            Debug.Log(number);
        }

        foreach(var number in GetMessage())
        {
            Debug.Log(number.ToString());
        }
    }

    // yield는 C#에서 한 번에 하나 씩 값을 넘기고, 메소드가 일시 정지 되며
    // 후속 값들을 지속적으로 반환하게 합니다.(반복적인 작업, 순차적인 데이터 처리에 사용됩니다.)

    // yield는 yield return, yield break로 사용됩니다.

    // yield return은 메소드가 값을 반환하면서 그 지점에서 메소드 실행을 일시 중지 시킵니다.
    // 호출자가 다음 값을 요구할 때마다 대기합니다.

    // yield break는 메소드에서의 반복을 종료하는 코드입니다.(실행 종료)

    // yield return을 사용하는 메소드는 IEnumerable 또는 IEnumerator 인터페이스를 구현하게 됩니다.
    // 컬렉션을 자동으로 순회하는 foreach와 자주 사용됩니다.

    // 장점 : 값을 필요로 할 때까지 계산을 미루는 방식(메모리 효율이 좋음, 큰 데이터를 처리할 때 이점이 큽니다.)
    // --> 모든 데이터를 저장하는게 아닌 필요한 부분만 처리할 수 있게 되기 때문

    static IEnumerable GetMessage()
    {
        Debug.Log("메소드 시작");
        yield return 1;
        // 야를 내보내고, 다시 해당 메소드로 돌아옵니다.
        Debug.Log("탈출 했다가 돌아옴 1");
        yield return 2;
        Debug.Log("탈출 했다가 돌아옴 2");
        yield break; // 순환 작업 종료

        // -- Unreachable Code -- (접근 불가)
        Debug.Log("탈출 했다가 돌아옴 3");
        yield return 3;
    }

}
```

|기능|호출 시 객체 생성 여부|설명|
|------|---|---|
|yield return null|객체 생성 x, 내부적으로 처리|다음 프레임까지 대기|
|yield return new WaitForSeconds(float x)|WaitForSeconds 객체|x(초)만큼의 시간을 대기|
|yield return new WaitFixedUpdate()|WaitFixedUpdate 객체|다음 프레임의 FixedUpdate 까지 대기|
|yield return new WaitForEndOfFrame()|WaitForEndOfFrame 객체|모든 렌더링 작업이 끝날 때까지 대기|
|yield return startCoroutine(string s)|Coroutine 객체|입력한 다른 코루틴이 끝날 때까지 대기|

### 인스턴스 캐싱
매번 코루틴 함수를 호출하면 새 IEnumerator 인스턴스가 생성되기에 과도한 생성 시 문제가 될 수 있음.<br>
그런 경우 다음과 같이 미리 만들어두고 사용하면, 생성하지 않고 재사용으로 진행할 수 있음.(GC 부담을 줄이고 싶을 경우)<br>
```cs
private IEnumerator cached;
void Start()
{
    cached = Sample();
}
IEnumerator Sample() { … }
```
단 상태가 변하는 코루틴을 재사용 시 상태가 꼬일 수 있어서 새 인스턴스를 만드는 것이 좋습니다.<br>
재사용 시의 상태 초기화 코드도 필요할 수 있습니다.<br>

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
