using UnityEngine;
using UnityEngine.EventSystems; // �߰� �ʿ�

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

    // �߰�: �̵� ��� �÷���
    private bool movementLocked = false;

    //void Awake()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    rb.constraints = RigidbodyConstraints.FreezeRotation;

    //    animationComponent = GetComponent<Animation>();
    //    stats = GetComponent<PlayerStatsManager>();

    //    if (animationComponent == null)
    //        Debug.LogError("Animation ������Ʈ�� �����ϴ�!");
    //    if (stats == null)
    //        Debug.LogError("PlayerCombatStats ������Ʈ�� �����ϴ�!");

    //    wallLayer = LayerMask.GetMask("Wall");
    //}
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animationComponent = GetComponent<Animation>();
        stats = PlayerStatsManager.Instance; // �� �̱���

        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� �����ϴ�!");
        if (stats == null)
            Debug.LogError("PlayerStatsManager �̱����� �����ϴ�!");

        wallLayer = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        // ��� �߿��� �Է� ��ü�� ���� ����
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
        // ��� �߿��� ���� �̵��� �������� ����
        if (isMoving && !movementLocked)
            MovePlayer();
    }

    void HandleMovementInput()
    {
        // ��� ���� (�ߺ� ������ġ)
        if (movementLocked) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // ���콺 ��Ŭ�� �̵�
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

    // === �ܺ� ���ٿ� ===
    public bool IsMoving() => isMoving;
    public Animation GetAnimation() => animationComponent;

    // �߰�: ��ų ���� ȣ���ϴ� �̵� ��� ���
    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        if (locked)
        {
            // ��� �̵� ���� (���� �ӵ��� 0����)
            isMoving = false;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            //����: ���⼭ Stand�� ������ ������� �ʽ��ϴ�.
            // ��ų �ִϸ��̼�(�Ǵ� �ٸ� �ִϸ��̼�)�� �̹� ��� ���� �� �����Ƿ� ���� ����.
        }
        // ��� ���� �ÿ� �Է�/�̵� ��ƾ�� �ڿ������� �簳�˴ϴ�.
    }

    // (����) �ܺο��� ���� Ȯ���ϰ� �ʹٸ�:
    public bool IsMovementLocked() => movementLocked;
}
