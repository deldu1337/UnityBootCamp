using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target; // 추적할 목표 (플레이어 등)
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator 컴포넌트가 Enemy에 없습니다!");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            // 목표 없으면 대기 상태
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 direction = (target.position - rb.position);
        direction.y = 0; // 높이 무시
        float distance = direction.magnitude;

        if (distance > 0.1f)
        {
            Vector3 moveDir = direction.normalized;

            // 이동
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // 이동 속도에 따라 애니메이션 전환
            animator.SetFloat("Speed", moveSpeed);
        }
        else
        {
            // 목표 근처면 대기
            animator.SetFloat("Speed", 0f);
        }
    }
}
