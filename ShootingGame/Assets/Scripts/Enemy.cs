using UnityEngine;
// 플레이어와 적의 차이
// 플레이어: 컨트롤을 사용자가 진행합니다.
// 적: 멀티 게임이 아니라면, 따로 정해진 명령에 따라 자동으로 움직이게 됩니다.

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Down, Chase // 아래로 내려가는 패턴, 플레이어를 추적하는 패턴
    }
    // 이동 속도
    public float speed = 5.0f;
    public EnemyType type = EnemyType.Down; // 기본적으로는 아래로 내려가는 기믹만 설계
    Vector3 dir; // 방향 설정

    // 적에 대한 패턴
    private void Start()
    {
        // 함수 분리
        // 장점: 가독성 높아짐
        //       재사용성이 높아질 수 있음(공격 패턴 리셋, 재생성 시의 방향 설정 등)
        PatternSetting();
    }

    void PatternSetting()
    {
        int rand = Random.Range(0, 10); // 0 ~ 9까지의 값 중 하나의 값을 랜덤으로 가져오겠습니다.

        if (rand < 3) // 0, 1, 2 (전체 숫자 10개 중에 3개니까 30%)
        {
            type = EnemyType.Chase;
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            dir = target.transform.position - transform.position; // 타겟 위치 - 본인 위치 = 방향
            dir.Normalize(); // 방향의 크기는 1로 설정합니다.
        }
        else
        {
            type = EnemyType.Down;
            // 아래로 내려가는 기능
            dir = Vector3.down;
        }
    }

    
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    // 충돌 이벤트 설계
    // 오브젝트와 오브젝트 간의 물리적인 충돌 발생 시 호출됩니다.
    // 둘 중 하나라도 Rigidbody(강체)를 가지고 있어야 처리됩니다.

    // OnCollisionEnter : 충돌 발생 시 1번만 호출
    // OnCollisionStay : 충돌 유지하는 동안 호출
    // OnCollisionExit : 충돌 발생 후 충돌 작업에서 벗어날 경우 1번 호출

    // 트리거도 OnTriggerXXX로 위와 같은 형태의 문법을 가지고 있습니다.
    // 2D일 경우 OnCollisionEnter2D처럼 마지막에 2D를 명시합니다.

    // 일반적인 물리적 충돌 Collision (실제적인 힘에 의해 물체가 회전하거나 이동됨)
    // Is Trigger 체크가 진행된 오브젝트와의 트리거 충돌 Trigger (충돌 여부만 체크함)
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject); // 상대방 파괴
        Destroy(gameObject); // 자신 파괴
    }
}
