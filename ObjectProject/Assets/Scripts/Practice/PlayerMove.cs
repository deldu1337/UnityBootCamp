using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    public float speed = 3.0f;
    public float rotateSpeed = 10f;

    private Camera cam;
    private Plane groundPlane;

    void Start()
    {
        cam = Camera.main;

        // Y=0 기준 지면 평면 생성 (플레이어가 있는 지면)
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        //  1. 이동
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(vertical, 0, -horizontal).normalized;
        transform.position += moveDir * speed * Time.deltaTime;

        //  2. 회전 - 정확한 마우스 지면 위치 계산 (레이캐스트 대신 Plane.Raycast 사용)
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = (hitPoint - transform.position).normalized;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(-lookDir.z, lookDir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(0, angle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }
    }
}
