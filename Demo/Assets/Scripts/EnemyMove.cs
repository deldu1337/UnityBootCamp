using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target; // ������ ��ǥ (�÷��̾� ��)
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator ������Ʈ�� Enemy�� �����ϴ�!");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            // ��ǥ ������ ��� ����
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 direction = (target.position - rb.position);
        direction.y = 0; // ���� ����
        float distance = direction.magnitude;

        if (distance > 0.1f)
        {
            Vector3 moveDir = direction.normalized;

            // �̵�
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // �̵� �ӵ��� ���� �ִϸ��̼� ��ȯ
            animator.SetFloat("Speed", moveSpeed);
        }
        else
        {
            // ��ǥ ��ó�� ���
            animator.SetFloat("Speed", 0f);
        }
    }
}
