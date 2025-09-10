using UnityEngine;

// Rigidbody 컴포넌트가 반드시 필요함을 명시
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    // 회전 속도 기본값 (Dex 기반 보정)
    [SerializeField] private float baseRotationSpeed = 10f;

    // 목표 지점
    private Vector3 targetPosition;
    private bool isMoving = false;

    // 컴포넌트 참조
    private Rigidbody rb;
    private Animation animationComponent;
    private PlayerCombatStats stats;

    // Wall 레이어
    private LayerMask wallLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animationComponent = GetComponent<Animation>();
        stats = GetComponent<PlayerCombatStats>();

        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 없습니다!");
        if (stats == null)
            Debug.LogError("PlayerCombatStats 컴포넌트가 없습니다!");

        wallLayer = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        HandleMovementInput();

        if (isMoving)
        {
            // 디버그 라인 & 목표 지점 시각화
            Debug.DrawLine(transform.position, targetPosition, Color.green);
            Debug.DrawRay(targetPosition + Vector3.up * 0.1f, Vector3.up * 0.2f, Color.green);
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
            MovePlayer();
    }

    // 마우스 입력을 받아 목표 위치 설정
    void HandleMovementInput()
    {
        // 마우스 우클릭으로 이동
        if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
                targetPosition.y = transform.position.y;
                isMoving = true;
            }
        }

        // 이동 애니메이션 처리
        if (isMoving && animationComponent != null)
        {
            if (!animationComponent.IsPlaying("Attack1H (ID 17 variation 0)") &&
                !animationComponent.IsPlaying("Run (ID 5 variation 0)"))
            {
                animationComponent.Play("Run (ID 5 variation 0)");
            }
        }
    }

    // 실제 이동 처리
    void MovePlayer()
    {
        // === PlayerCombatStats에서 이동 속도 가져오기 ===
        float moveSpeed = stats.Dex; // 민첩 수치가 곧 이동속도
        float rotationSpeed = baseRotationSpeed + stats.Dex * 0.5f; // 민첩에 비례해 회전속도 증가

        Vector3 direction = (targetPosition - rb.position).normalized;
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

        // 방향 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        // 목표 지점 도달 시 Idle
        if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // 외부 접근용
    public bool IsMoving() => isMoving;
    public Animation GetAnimation() => animationComponent;
}
