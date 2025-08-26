using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Rigidbody rb;
    private Animation animationComponent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 없습니다!");
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        if (isMoving)
            MovePlayer();
    }

    void HandleMovementInput()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
                targetPosition.y = transform.position.y;
                isMoving = true;

                // Run 애니메이션은 공격 중이어도 재생 가능 (디아블로 스타일)
                if (animationComponent != null && !animationComponent.IsPlaying("Run (ID 5 variation 0)"))
                    animationComponent.Play("Run (ID 5 variation 0)");
            }
        }
    }

    void MovePlayer()
    {
        Vector3 direction = (targetPosition - rb.position).normalized;
        Vector3 moveDelta = direction * moveSpeed * Time.fixedDeltaTime;
        Vector3 nextPos = rb.position + moveDelta;

        if (!Physics.Raycast(rb.position, direction, moveDelta.magnitude + 0.1f))
        {
            rb.MovePosition(nextPos);
        }
        else
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.1f);
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.1f);
        }
    }

    public bool IsMoving() => isMoving;
    public Animation GetAnimation() => animationComponent;
}
