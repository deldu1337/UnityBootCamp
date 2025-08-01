using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jp;
    public LayerMask ground;
    //public int gound = LayerMask.NameToLayer("ground");

    private Rigidbody rb;
    private bool isGrounding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Ű �Է�
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        // ���� ����
        Vector3 dir = new Vector3(x, 0, z);

        // �̵� �ӵ� ����
        Vector3 velocity = dir * speed;

        rb.linearVelocity = velocity;
        // ������ �ٵ��� �Ӽ�
        // linearVelocity = ���� �ӵ� (��ü�� ���� �󿡼� �̵��ϴ� �ӵ�)
        // angularVelocity = �� �ӵ� (��ü�� ȸ���ϴ� �ӵ�)

        // ���� ��� �߰�
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb.AddForce(Vector3.up * jp, ForceMode.Impulse);
            // ForceMode.Impulse : �������� ��
            // ForceMode.Force : �������� ��
        }
    }

    private bool isGrounded()
    {
        // �Ʒ� �������� 1��ŭ ���̸� ���� ���̾� üũ
        return Physics.Raycast(transform.position, Vector3.down, 1.0f, ground);
    }
}
