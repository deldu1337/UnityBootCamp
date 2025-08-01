using Unity.VisualScripting;
using UnityEngine;
// 중요 클래스 Mathf
// 유니티에서 수학 관련으로 제공되는 유틸리티 클래스
// 게임 개발에서 사용될 수 있는 수학 연산에 대한 정적 메소드와 상수를 제공합니다.

// 정적 메소드 : static 키워드로 구성된 해당 메소드는 클래스명.메소드명()
//               으로 사용이 가능합니다.
// ex) Mathf.Abs(-5);

public class MathfSample : MonoBehaviour
{
    float abs = -5;
    float ceil = 4.1f;
    float floor = 4.6f;
    float round = 4.51f;
    float clamp;
    float clamp01;
    float pow = 2;
    float sqrt = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"절댓값 {abs} = " + Mathf.Abs(abs)); // 절댓값(absolute number)
        Debug.Log($"올림 {ceil} = " + Mathf.Ceil(ceil));           // 올림(소수점과 상관없이 값을 올림 처리합니다.)
        Debug.Log($"내림 {floor} = " + Mathf.Floor(floor));          // 내림(소수점과 상관없이 값을 내림 처리합니다.)
        Debug.Log($"반올림 {round} = " + Mathf.Round(round));          // 반올림(4 이하면 내리고 5 이상이면 올립니다.)
        Debug.Log($"값 제한 (7, 0, 4) = " + Mathf.Clamp(7, 0, 4));        // 현재 전달 받은 값 = 7, 최소 = 0, 최대 = 4, 결과 = 4
                                                // (값, 최소, 최대) 순으로 값을 입력합니다.
        Debug.Log($"값 제한(1) {5} = " + Mathf.Clamp01(5));            // 현재 전달 받은 값 = 5, 최소 = 0, 최대 = 1. 결과 = 1
                                                // 벗어나면 최솟값 0 또는 최대값 1로 처리
                                                // 퍼센트 개념의 값을 처리할 때 자주 사용되는 코드
                                                // 최소 최대 범위에 따라 0과 1이 결정됩니다.
        // Clamp vs Clamp01
        // Clamp ==> 체력, 점수, 속도 등의 능력치 개념에서의 범위 제한
        // Clamp01 ==> 비율(퍼센트), 보간 값(1), 알파 값(색깔에서의 투명도)
        Debug.Log($"제곱 {pow} = " + Mathf.Pow(pow, 2));           // (값, 제곱 수(지수))를 전달 받아서 해당 수의 거듭 제곱 계산
        Debug.Log($"제곱근 {pow} = " + Mathf.Pow(pow, 0.5f));      // Mathf.Sqrt()로 계산하는 것 보다 연산이 매우 느림
        Debug.Log("지수가 음수일 경우 값은 역수 형태로 계산합니다." + Mathf.Pow(pow, -2));          // 2의 -2 제곱 => 1/4
        Debug.Log(Mathf.Sqrt(sqrt));            // 값을 전달 받아 해당 값의 제곱근(루트)을 계산
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
