using UnityEngine;

// Rigidbody ������Ʈ�� �ݵ�� �ʿ����� ���
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove1 : MonoBehaviour
{
    // �÷��̾� �̵� �ӵ�
    public float moveSpeed = 5f;
    // ȸ�� �ӵ�
    public float rotationSpeed = 10f;

    // Wall ���̾ �ڵ� ���ο��� ����
    private LayerMask wallLayer;
    // ��ǥ ����
    private Vector3 targetPosition;
    // �̵� ������ ����
    private bool isMoving = false;
    // Rigidbody ������Ʈ ����
    private Rigidbody rb;
    // Animation ������Ʈ ����
    private Animation animationComponent;

    void Awake()
    {
        // Rigidbody ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
        // ȸ�� ���� ���� (���������� ȸ������ �ʵ���)
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        wallLayer = LayerMask.GetMask("Wall");

        // Animation ������Ʈ ��������
        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� �����ϴ�!");
    }

    void Update()
    {
        // �� ������ �̵� �Է� ó��
        HandleMovementInput();
        if (isMoving)
        {
            // �÷��̾� ��ġ���� ��ǥ ��ġ���� ���� �� ǥ��
            Debug.DrawLine(transform.position, targetPosition, Color.green);

            // ��ǥ ��ġ�� ���� ��ü ǥ��
            Debug.DrawRay(targetPosition + Vector3.up * 0.1f, Vector3.up * 0.2f, Color.green);
        }
    }

    void FixedUpdate()
    {
        // ���� ������ FixedUpdate���� ó��
        if (isMoving)
            MovePlayer();
    }

    // ���콺 �Է��� �޾� ��ǥ ��ġ ����
    void HandleMovementInput()
    {
        // ���콺 ��Ŭ�� �� �̵�
        if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
        {
            // ī�޶󿡼� ���콺 ��ġ�� Ray �߻�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Ray�� ���� ������ ��ǥ ��ġ�� ����
                targetPosition = hit.point;
                targetPosition.y = transform.position.y; // y��ǥ�� �÷��̾� ���̷� ����
                isMoving = true;
            }
        }

        if (isMoving && animationComponent != null && !animationComponent.IsPlaying("Run (ID 5 variation 0)"))
            animationComponent.Play("Run (ID 5 variation 0)");
    }

    // ���� �÷��̾� �̵� ó��
    void MovePlayer()
    {
        // ��ǥ ���� ��� (����ȭ)
        Vector3 direction = (targetPosition - rb.position).normalized;
        // �̵��� �Ÿ� ���
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

        // ������ ������ ȸ�� ó��
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        // ��ǥ ������ ���� �����ϸ� �̵� ���� �� Idle �ִϸ��̼� ���
        if (Vector3.Distance(rb.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            if (animationComponent != null && !animationComponent.IsPlaying("Attack1H (ID 17 variation 0)"))
                //animationComponent.CrossFade("Stand (ID 0 variation 0)", 0.1f);
                animationComponent.Play("Stand (ID 0 variation 0)");
        }
    }

    // �ܺο��� �̵� ���� Ȯ��
    public bool IsMoving() => isMoving;
    // �ܺο��� �ִϸ��̼� ������Ʈ ����
    public Animation GetAnimation() => animationComponent;
}
