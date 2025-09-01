using UnityEngine;

// Rigidbody 컴포넌트가 반드시 필요함을 명시
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove1 : MonoBehaviour
{
    // 플레이어 이동 속도
    public float moveSpeed = 5f;
    // 회전 속도
    public float rotationSpeed = 10f;

    // Wall 레이어를 코드 내부에서 지정
    private LayerMask wallLayer;
    // 목표 지점
    private Vector3 targetPosition;
    // 이동 중인지 여부
    private bool isMoving = false;
    // Rigidbody 컴포넌트 참조
    private Rigidbody rb;
    // Animation 컴포넌트 참조
    private Animation animationComponent;

    void Awake()
    {
        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        // 회전 강제 고정 (물리적으로 회전하지 않도록)
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        wallLayer = LayerMask.GetMask("Wall");

        // Animation 컴포넌트 가져오기
        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 없습니다!");
    }

    void Update()
    {
        // 매 프레임 이동 입력 처리
        HandleMovementInput();
        if (isMoving)
        {
            // 플레이어 위치에서 목표 위치까지 빨간 선 표시
            Debug.DrawLine(transform.position, targetPosition, Color.green);

            // 목표 위치에 작은 구체 표시
            Debug.DrawRay(targetPosition + Vector3.up * 0.1f, Vector3.up * 0.2f, Color.green);
        }
    }

    void FixedUpdate()
    {
        // 물리 연산은 FixedUpdate에서 처리
        if (isMoving)
            MovePlayer();
    }

    // 마우스 입력을 받아 목표 위치 설정
    void HandleMovementInput()
    {
        // 마우스 우클릭 시 이동
        if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
        {
            // 카메라에서 마우스 위치로 Ray 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Ray가 닿은 지점을 목표 위치로 설정
                targetPosition = hit.point;
                targetPosition.y = transform.position.y; // y좌표는 플레이어 높이로 고정
                isMoving = true;
            }
        }

        if (isMoving && animationComponent != null && !animationComponent.IsPlaying("Run (ID 5 variation 0)"))
            animationComponent.Play("Run (ID 5 variation 0)");
    }

    // 실제 플레이어 이동 처리
    void MovePlayer()
    {
        // 목표 방향 계산 (정규화)
        Vector3 direction = (targetPosition - rb.position).normalized;
        // 이동할 거리 계산
        Vector3 moveDelta = direction * moveSpeed * Time.fixedDeltaTime;
        Vector3 nextPos = rb.position + moveDelta;

        // Wall 레이어 충돌 체크
        if (Physics.Raycast(rb.position, direction, moveDelta.magnitude + 0.1f, wallLayer))
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
            return;
        }

        rb.MovePosition(nextPos);

        // 방향이 있으면 회전 처리
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        // 목표 지점에 거의 도달하면 이동 중지 후 Idle 애니메이션 재생
        if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                //animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.1f);
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // 외부에서 이동 여부 확인
    public bool IsMoving() => isMoving;
    // 외부에서 애니메이션 컴포넌트 참조
    public Animation GetAnimation() => animationComponent;
}
