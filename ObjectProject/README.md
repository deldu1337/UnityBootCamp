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


