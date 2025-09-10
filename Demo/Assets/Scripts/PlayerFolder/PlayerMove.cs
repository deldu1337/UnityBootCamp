using UnityEngine;

// Rigidbody ������Ʈ�� �ݵ�� �ʿ����� ���
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    // ȸ�� �ӵ� �⺻�� (Dex ��� ����)
    [SerializeField] private float baseRotationSpeed = 10f;

    // ��ǥ ����
    private Vector3 targetPosition;
    private bool isMoving = false;

    // ������Ʈ ����
    private Rigidbody rb;
    private Animation animationComponent;
    private PlayerCombatStats stats;

    // Wall ���̾�
    private LayerMask wallLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animationComponent = GetComponent<Animation>();
        stats = GetComponent<PlayerCombatStats>();

        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� �����ϴ�!");
        if (stats == null)
            Debug.LogError("PlayerCombatStats ������Ʈ�� �����ϴ�!");

        wallLayer = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        HandleMovementInput();

        if (isMoving)
        {
            // ����� ���� & ��ǥ ���� �ð�ȭ
            Debug.DrawLine(transform.position, targetPosition, Color.green);
            Debug.DrawRay(targetPosition + Vector3.up * 0.1f, Vector3.up * 0.2f, Color.green);
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
            MovePlayer();
    }

    // ���콺 �Է��� �޾� ��ǥ ��ġ ����
    void HandleMovementInput()
    {
        // ���콺 ��Ŭ������ �̵�
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

        // �̵� �ִϸ��̼� ó��
        if (isMoving && animationComponent != null)
        {
            if (!animationComponent.IsPlaying("Attack1H (ID 17 variation 0)") &&
                !animationComponent.IsPlaying("Run (ID 5 variation 0)"))
            {
                animationComponent.Play("Run (ID 5 variation 0)");
            }
        }
    }

    // ���� �̵� ó��
    void MovePlayer()
    {
        // === PlayerCombatStats���� �̵� �ӵ� �������� ===
        float moveSpeed = stats.Dex; // ��ø ��ġ�� �� �̵��ӵ�
        float rotationSpeed = baseRotationSpeed + stats.Dex * 0.5f; // ��ø�� ����� ȸ���ӵ� ����

        Vector3 direction = (targetPosition - rb.position).normalized;
        Vector3 moveDelta = direction * moveSpeed * Time.fixedDeltaTime;
        Vector3 nextPos = rb.position + moveDelta;

        // Wall ���̾� �浹 üũ
        if (Physics.Raycast(rb.position, direction, moveDelta.magnitude + 0.1f, wallLayer))
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
            return;
        }

        rb.MovePosition(nextPos);

        // ���� ȸ��
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        // ��ǥ ���� ���� �� Idle
        if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // �ܺ� ���ٿ�
    public bool IsMoving() => isMoving;
    public Animation GetAnimation() => animationComponent;
}
