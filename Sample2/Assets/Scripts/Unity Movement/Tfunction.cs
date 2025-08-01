using UnityEngine;
// 삼각 함수
// 유니티에서 제공해주는 삼각 함수는 주로 회전, 카메라 제어, 곡선, 움직임에 대한 표현으로 사용됩니다.

// 특징) 단위를 라디안으로 사용합니다. 1 라디안 = 약 57도


public class Tfunction : MonoBehaviour
{
    // 요약
    // Sin(Radian) : 주어진 각도의 Y 좌표 (세로 방향 위치)
    // Cos(Radian) : 주어진 각도의 X 좌표 (가로 방향 위치)
    // Tan(Radian) : 주어진 각도의 기울기 ( Y/X )

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
        if (Input.GetMouseButton(1))
        {
            Wave();
        }
        if (Input.GetMouseButton(2))
        {
            ButterFly();
        }
    }
}
