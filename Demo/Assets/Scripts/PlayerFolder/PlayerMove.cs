using Unity.Hierarchy;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animation animationComponent; // Animator 대신

    void Awake()
    {
        animationComponent = GetComponentInChildren<Animation>();

        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 Player 프리팹 또는 자식에 없습니다!");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
                targetPosition.y = transform.position.y; // 높이 고정
                isMoving = true;

                if (animationComponent != null)
                {
                    AnimationState runState = animationComponent["Run (ID 5 variation 0)"];
                    runState.speed = moveSpeed / 6.5f; // 속도 조절
                    animationComponent.Play("Run (ID 5 variation 0)"); // Run 애니메이션 재생
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 direction = (targetPosition - rb.position).normalized;
            Vector3 moveDelta = direction * moveSpeed * Time.fixedDeltaTime;
            Vector3 nextPos = rb.position + moveDelta;

            // --- 벽 충돌 체크 추가 ---
            if (!Physics.Raycast(rb.position, direction, moveDelta.magnitude + 0.1f))
            {
                rb.MovePosition(nextPos);
            }
            else
            {
                // 벽에 막히면 멈추기
                isMoving = false;
                if (animationComponent != null)
                    animationComponent.Play("Stand (ID 0 variation 0)");
            }

            // 회전
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            }

            if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                if (animationComponent != null)
                    animationComponent.Play("Stand (ID 0 variation 0)");
            }
        }
    }
}
