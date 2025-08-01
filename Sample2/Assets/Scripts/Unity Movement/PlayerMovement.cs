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
        // 키 입력
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        // 방향 설계
        Vector3 dir = new Vector3(x, 0, z);

        // 이동 속도 설정
        Vector3 velocity = dir * speed;

        rb.linearVelocity = velocity;
        // 리지드 바디의 속성
        // linearVelocity = 선형 속도 (물체가 공간 상에서 이동하는 속도)
        // angularVelocity = 각 속도 (물체가 회전하는 속도)

        // 점프 기능 추가
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb.AddForce(Vector3.up * jp, ForceMode.Impulse);
            // ForceMode.Impulse : 순간적인 힘
            // ForceMode.Force : 지속적인 힘
        }
    }

    private bool isGrounded()
    {
        // 아래 방향으로 1만큼 레이를 쏴서 레이어 체크
        return Physics.Raycast(transform.position, Vector3.down, 1.0f, ground);
    }
}
