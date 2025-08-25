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
    private Animation animationComponent; // Animator ���

    void Awake()
    {
        animationComponent = GetComponentInChildren<Animation>();

        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� Player ������ �Ǵ� �ڽĿ� �����ϴ�!");
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
                targetPosition.y = transform.position.y; // ���� ����
                isMoving = true;

                if (animationComponent != null)
                {
                    AnimationState runState = animationComponent["Run (ID 5 variation 0)"];
                    runState.speed = moveSpeed / 6.5f; // �ӵ� ����
                    animationComponent.Play("Run (ID 5 variation 0)"); // Run �ִϸ��̼� ���
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

            // --- �� �浹 üũ �߰� ---
            if (!Physics.Raycast(rb.position, direction, moveDelta.magnitude + 0.1f))
            {
                rb.MovePosition(nextPos);
            }
            else
            {
                // ���� ������ ���߱�
                isMoving = false;
                if (animationComponent != null)
                    animationComponent.Play("Stand (ID 0 variation 0)");
            }

            // ȸ��
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
