using UnityEngine;
using UnityEngine.EventSystems; // 추가 필요

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float baseRotationSpeed = 10f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Rigidbody rb;
    private Animation animationComponent;
    private PlayerStatsManager stats;

    private LayerMask wallLayer;

    // 추가: 이동 잠금 플래그
    private bool movementLocked = false;

    //void Awake()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.constraints = RigidbodyConstraints.FreezeRotation;

    //    animationComponent = GetComponent<Animation>();
    //    stats = GetComponent<PlayerStatsManager>();

    //    if (animationComponent == null)
    //        Debug.LogError("Animation 컴포넌트가 없습니다!");
    //    if (stats == null)
    //        Debug.LogError("PlayerCombatStats 컴포넌트가 없습니다!");

    //    wallLayer = LayerMask.GetMask("Wall");
    //}
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animationComponent = GetComponent<Animation>();
        stats = PlayerStatsManager.Instance; // ← 싱글톤

        if (animationComponent == null)
            Debug.LogError("Animation 컴포넌트가 없습니다!");
        if (stats == null)
            Debug.LogError("PlayerStatsManager 싱글톤이 없습니다!");

        wallLayer = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        // 잠금 중에는 입력 자체를 받지 않음
        if (!movementLocked)
            HandleMovementInput();

        if (isMoving)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.green);
            Debug.DrawRay(targetPosition + Vector3.up * 0.1f, Vector3.up * 0.2f, Color.green);
        }
    }

    void FixedUpdate()
    {
        // 잠금 중에는 실제 이동도 수행하지 않음
        if (isMoving && !movementLocked)
            MovePlayer();
    }

    void HandleMovementInput()
    {
        // 방어 로직 (중복 안전장치)
        if (movementLocked) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 마우스 우클릭 이동
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

    void MovePlayer()
    {
        float moveSpeed = stats.Data.Dex;
        float rotationSpeed = baseRotationSpeed + stats.Data.Dex * 0.5f;

        Vector3 direction = (targetPosition - rb.position).normalized;
        Vector3 moveDelta = direction * moveSpeed * Time.fixedDeltaTime;
        Vector3 nextPos = rb.position + moveDelta;

        if (Physics.SphereCast(rb.position, 0.1f, direction, out _, moveDelta.magnitude + 0.1f, wallLayer))
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
            return;
        }
        if (moveDelta.magnitude < 0.001f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
            return;
        }

        rb.MovePosition(nextPos);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        if (Vector3.Distance(rb.position, targetPosition) < 0.2f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // === 외부 접근용 ===
    public bool IsMoving() => isMoving;
    public Animation GetAnimation() => animationComponent;

    // 추가: 스킬 등이 호출하는 이동 잠금 토글
    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        if (locked)
        {
            // 즉시 이동 정지 (물리 속도도 0으로)
            isMoving = false;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            //주의: 여기서 Stand를 강제로 재생하지 않습니다.
            // 스킬 애니메이션(또는 다른 애니메이션)이 이미 재생 중일 수 있으므로 간섭 금지.
        }
        // 잠금 해제 시엔 입력/이동 루틴이 자연스럽게 재개됩니다.
    }

    // (선택) 외부에서 상태 확인하고 싶다면:
    public bool IsMovementLocked() => movementLocked;
}
