using UnityEngine;
// 유니티의 회전(Rotate)
// 1. 오일러 각(Euler Angle)에 의한 회전 -> x, y, z 회전 값
// 유니티 인스펙터에서의 Transform 컴포넌트의 Rotation에 표기된 값(각도 기준)
// ex) Rotation X 120, Y 45, Z 0 --> X축으로 120도 Y축으로 45도 회전되었음을 의미합니다.

// 2. 쿼터니언(Quaternion) -> 회전을 계산하기에 안정적인 수학적인 방식을 가진 값
// 유니티 엔진 내부에서 실제로 저장되고 계산되는 값

// 쿼터니언으로 처리하는 이유
// 유니티에서 쿼터니언 -> 오일러 각 변환 시 360도 이상의 회전은 보정될 수 있음
// ex) 380도 회전 -> 20도 회전

// 짐벌 락 현상(Gimbal Lock) : 오일러 각을 이용해 회전을 표현하는 경우에 발생하는 회전 자유도의 손실

// --> 어떤 축이 다른 축과 정렬되는 순간, 한 축의 회전이 무효되면서 회전 방향이 3개가 아닌 2개만 남는 현상

// 쿼터니언의 구조
// 4차원 복소수 (x,y,z,w(스칼라)) 하나의 벡터(x,y,z), 하나의 스칼라 w
// 쿼터니언도 벡터처럼 방향인 동시에 회전의 개념을 가지고 있습니다. (벡터는 위치이면서 방향의 개념)
// 세 축을 동시에 회전시켜 짐벌 락 현상이 발생하지 않도록 구성되어있음(x,y,z는 항상 동시에 변화한다.)
// 쿼터니언은 회전의 원점과 방향을 비교해 회전을 측정할 수 있습니다.
// orientation : 방향

// 단점 : 180도 이상의 표현 불가
//        직관적으로 바로 이해하기 어려운 구조




public class ObjectRotate : MonoBehaviour
{
    void Update()
    {
        // transform.Rotate()는 회전을 진행시키는 가장 기본적인 코드
        // transform.Rotate(Vector3 eulerAngles); // 지정한 축을 기준으로 회전
        // transform.Rotate(float x, float y, float z); // 로컬(게임 오브젝트) 기준의 회전
        // transform.Rotate(Vector3 axis, float angle); // 특정 축을 중심으로 회전
        // transform.Rotate(Vector3 axis, float angle, Space.World); // 월드 또는 로컬 중에 선택

        // 월드 기준으로 z축으로 60도만큼 회전하라.
        // ex) transform.Rotate(Vector3.forward, 60f * Time.deltaTime, Space.World);

        // 게임 오브젝트를 기준으로 회전을 진행합니다.
        //transform.Rotate(new Vector3(20,20,20) * Time.deltaTime);

        // 절대 좌표를 기준으로 회전을 진행합니다.
        transform.Rotate(new Vector3(20, 20, 20) * Time.deltaTime, Space.World);
    }
}
