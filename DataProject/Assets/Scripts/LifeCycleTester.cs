using System.Collections;
using UnityEngine;

// 유니티 라이프 사이클에 대한 동작 순서 확인 코드
// Update를 활용해 프레임 별 호출을 순서대로 확인해봅시다.

public class LifeCycleTester : MonoBehaviour
{
    private int count_per_frame = 0; // 프레임 단위 호출 카운트

    private void Awake()
    {
        Debug.Log("[Awake] 오브젝트의 생성 시 딱 한번만 실행되는 영역");
    }

    private void OnEnable()
    {
        Debug.Log("[OnEnable] 오브젝트의 활성화 시 호출되는 영역");
    }

    void Start()
    {
        Debug.Log("[Start] 첫 프레임 시작 전에 호출되는 영역");
        StartCoroutine(CustomCoroutine());
    }

    private void FixedUpdate()
    {
        Debug.Log($"[Frame : {count_per_frame}] [FixedUpdate] 물리에 대한 업데이트가 진행되는 영역");
    }

    void Update()
    {
        count_per_frame++; // 카운트 증가
        Debug.Log($"[Frame : {count_per_frame}] [Update] 게임 로직에 대한 호출 진행되는 영역");

        // 증가된 카운트에 따라 테스트 진행
        if(count_per_frame == 3)
        {
            Debug.Log($"[Frame : {count_per_frame}] [Test 1] 오브젝트의 비활성화 작업을 진행합니다.");
            gameObject.SetActive(false);
        }

        if (count_per_frame == 6)
        {
            Debug.Log($"[Frame : {count_per_frame}] [Test 2] 오브젝트의 활성화 작업을 진행합니다.");
            gameObject.SetActive(true);
        }

        if (count_per_frame == 9)
        {
            Debug.Log($"[Frame : {count_per_frame}] [Test 3] 오브젝트의 파괴 작업을 진행합니다.");
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        Debug.Log($"[Frame : {count_per_frame}] [LateUpdate] 물리 연산, 로직 이후의 후처리");
    }

    // 코루틴 설계
    // yield에 의해 대기 후 사이클로 돌아오는 과정이 존재하며, 보통 Update의 틈새에 맞춰져 실행됩니다.
    IEnumerator CustomCoroutine()
    {
        Debug.Log("[Coroutine] 코루틴에 대한 시작: StartCoroutine");
        yield return null;
        Debug.Log("[Coroutine] 1 프레임 후 다시 실행됩니다.");

        yield return new WaitForSeconds(1.0f); // 1초에 대한 대기
        Debug.Log("[Coroutine] [Seconds] 1 초 후 다시 실행됩니다.");

        yield return new WaitForFixedUpdate();
        Debug.Log("[Coroutine] [FixedUpdate] FixedUpdate가 지나면 다시 실행됩니다.");

        yield return new WaitForEndOfFrame();
        Debug.Log("[Coroutine] [End of Frame] 프레임의 끝이 지나면 다시 실행됩니다.");
    }

    private void OnDisable()
    {
        Debug.Log("[OnDisable] 오브젝트의 비활성화 시 호출되는 영역");
    }

    private void OnDestroy()
    {
        Debug.Log("[OnDestroy] 오브젝트가 파괴되었을 경우 호출되는 영역");
        // 이 위치에서는 파괴 절차가 진행되고 있기 때문에, 다음과 같은 작업은 불가능하거나 무의미 합니다.
        // gameObject.SetActive(true / false) --> 진행이 되도 의미가 없음.(상태 변경은 파괴 전 단계에서 처리)
        // 자기 자신에 대한 복구 작업은 불가능합니다. 계속 파괴됩니다.
        // 새로운 게임 오브젝트를 생성하는 작업은 가능합니다.
    }
}
