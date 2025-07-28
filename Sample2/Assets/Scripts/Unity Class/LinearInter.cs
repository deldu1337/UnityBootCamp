using UnityEngine;

public class LinearInter : MonoBehaviour
{
    //Vector3.Lerp(start,end,t);
    // start -> end까지 t에 따라 선형 보간합니다.
    // --> 해당 방향으로 일정 간격 천천히 이동하는 코드
    // t의 범위(0 ~ 1)이고 float
    public Transform target;
    public float speed = 1.0f;

    private Vector3 start_position;
    private float t = 0.0f;

    private void Start()
    {
        start_position = transform.position;
    }

    private void Update()
    {
        // 보간이 끝나지 않았을 때만 이동을 진행하겠습니다.
        if(t < 1.0f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start_position, target.position, t);
        }
    }
}
