using UnityEngine;

// 목표 방향으로 회전시키는 코드
public class TargetRotate : MonoBehaviour
{
    public Transform target;

    public float speed = 90.0f; // 초당 회전 속도(degree)

    void Update()
    {
        // Quaternion.LookRotation(Vector3 forward) : 특정 방향을 바라보는 회전을 계산
        var targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // Quaternion.RotateTowards(위치, 타겟 위치, 속도) :
        // 현재의 회전에서 타겟의 회전으로 일정 속도로 부드럽게 회전을 진행하는 함수
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed);
    }
}
