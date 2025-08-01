using UnityEngine;
// Mathf 클래스에서 제공해주는 상수 값

public class MathfConstant : MonoBehaviour
{
    void Start()
    {
        Debug.Log(Mathf.PI); // 원주율 3.141592...
        Debug.Log(Mathf.Infinity); // 무한대
        // 수학적 연산에 의해서 표현할 수 있는 최대의 수를 넘는 경우라면 자동으로 처리되는 값
        // 직접적으로 Infinity를 작성해 명시적으로 무한대를 표현하기도 합니다.
        // 1) Pow(0, -2) = 0^-2 = 1/(0^2) = 1/0(Infinity)
        // 2) float 범위로 표현할 수 없는 큰 수를 제곱하는 경우, 연산 결과일 경우
        //    (+- 3.4 * 10^38)을 넘어서 오버플로우 현상이 발생했을 경우 -> Infinity
        
        // float의 최대 값 == float.MaxValue

        

        Debug.Log(Mathf.NegativeInfinity); // 음의 무한대
        // 음의 무한대는 어떤 숫자도 될 수 없는 음의 방향을 가리키는 개념
        // 1) 음수에 대한 지수 연산이 오버플로우 되는 경우
        // 2) 직접적으로 NegativeInfinity가 명시되는 경우

        Debug.Log(Mathf.Sqrt(-1)); // NaN(Not a Number) : 수학적으로 정의되지 않은 계산 결과일 경우 나오는 값
        // NaN에 대하여
        // 1. 두 값이 NaN일 경우 그 값에 대한 비교는 불가능합니다. (같은지 체크하면 false)
        //    float.IsNaN(값)을 통해 해당 값이 NaN인지만 확인이 가능하다.

        // 2. Infinity = NaN
        //    0/0 과 같이 수학적으로 말이 아예 안되는 값
        //    음수에 대한 루트 계산 (허수나 복소수 같은 개념은 사용자가 따로 설계한 기능으로 수행한다.)
        //    유니티나 C#에서 허수에 대한 직접적인 지원을 하지 않습니다.
        //    수학적으로의 간단 표현
        //    허수 -> 음의 제곱근 Sqrt(-1)
        //    복소수 + a + bi 형태로 표현
        //    C# System.Numerics.Complex 기능을 통해 허수에 대한 계산이 가능합니다.
        //    using System.Numerics;
        //    Complex c = Complex.Sqrt(-1);
        //    단점) 유니티 빌드 기준에 따라 사용이 제한되는 경우가 있습니다.(ex. WebGL)

        // NaN에 대한 방어 코드 작성 시 활용
        // position이 만약에 NaN이다? 오브젝트가 씬에서 사라집니다.
        // Rigidbody에서 사용하는 값 중 velocity(속력)이 NaN이라면? --> 물리 엔진 작용에 대한 고장
        // Quaternion(회전)에서 사용하는 값이 NaN이라면? --> 회전이 끊기거나 깨지는 현상 발생 / 회전이 정상적으로 안된다.
        Vector3 pos = transform.position;
        if(float.IsNaN(pos.x))
        {
            Debug.LogWarning("현재 위치에서 NaN 발견, 원점으로 이동합니다.");
            pos = Vector3.zero;
        }
        // 그 이외의 값
        // 1. Mathf.Deg2Rad() : Degree(각도)
        //                      Transform.Rotate(0,90,0) -> y축으로 90도만큼 회전
        //                      Transform.eulerAngles -> transform에서의 (x,y,z) 각도 표현(도)
        //                      Quaternion.AnglerAxis(45f, Vector3.up) -> 45도만큼 회전
        //                      Vector3.Angle(A,B); -> A벡터와 B벡터 사이의 각도(도)
        //                      Quaternion.Angle(A,B) -> 두 회전 간의 차이를 각도(도)
        //                      유니티 인스펙터에서 보여지는 회전 입력 -> 도
        //                      유니티의 애니메이션에서 사용하는 회전 속성 -> 도

        // 2. Mathf.Rad2Deg() : Radian(수학적으로 사용되는 각도의 단위)
        //                      반지름과 같은 길이를 가진 호가 가진 중심각 = 1 라디안
        //                      유니티에서 제공해주는 삼각 함수 계산에서 각도 대신 라디안을 요구함.

        // 유니티에서는 저 두 기능을 통해 라디안 -> 도 / 도 -> 라디안으로의 변환을 처리합니다.

        // 결론) 라디안은 원의 길이, 각속도, 미분 등의 작업을 진행할 때(수학/물리 작업)
        //       계산이 더 간단하게 진행되고 따라서 유니티 등에서 사용되는 삼각 함수와 같은 계산식에서
        //       사용됩니다.

        // 요약) degree : 직접적인 회전에 대한 표현(입력, 보여지는 각)
        //       Radian : 삼각 함수를 활용한 각 계산

        // 자주 사용되는 도와 라디안의 값
        // 0            0           0
        // 90           Pi/2        90
        // 180          Pi          180
        // 360          2Pi         360

        // 3. Mathf.Epsilon() : float형에서 0이 아닌 가장 작은 양수 값(거의 0에 가까운 값)
        //                      --> 미세한 값을 다룰 때 사용하는 값
        //                      float에서 0f보다 Epsilon으로 0을 체크하면 안전하게 계산됩니다.
        //                      0으로 나누는 상황을 방지합니다.                     

    }
}
