using UnityEngine;

// 쿼터니언 기능 정리
// Quaternion.identity = 회전 없음
// Quaternion.Euler(x,y,z) = 오일러 각 -> 쿼터니언 변환
// Quaternion.AngleAxis(angle, axis) 특정 축 기준의 회전
// Quaternion.LookRotation(forward,upward[default : Vector3.up]) : 해당 벡터 방향을 바라보는 회전 상태에 대한 return

// forward : 회전 시킬 방향(Vector3)
// upward : 방향을 바라보고 있을 때의 위 부분을 설정

// 회전 값 적용
// transform.rotation = Quaternion.Euler(x,y,z) : // 현재 오브젝트의 회전 값을 적용합니다.

// 회전에 대한 보간
// Quaternion.Slerp(from, to, t) : 구면 선형 보간
// Quaternion.Lerp(from, to, t) : 선형 보간
// Quaternion.RotateTowards(from, to, maxDegree) : 일정 각도만큼 점진적으로 회전을 처리합니다.

// transform.LookAt() vs Quaternion.LookRotation()
// 둘다 특정 방향을 보게하는 코드

// 1. LookAt(target) : 추가 회전 보간이나 제어가 어렵고, 설정해 준 값을 기준으로 transform.rotation을
//                     자동으로 설정해주는 기능(내부에서 Quaternion.LookRotation()을 사용하는 방식)
// --> 그냥 날 바라봤으면 좋겠다.

// 2. LookRotation(direction) 의 경우는 회전 값만 계산하고 직접적인 적용은 하지 않습니다.
// --> 계산은 끝났으나 추가적인 작업으로 계산을 처리하면 되지 않을까?

public class QuaternionSample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
