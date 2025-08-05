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

        // Y=0 ���� ���� ��� ���� (�÷��̾ �ִ� ����)
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        //  1. �̵�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(vertical, 0, -horizontal).normalized;
        transform.position += moveDir * speed * Time.deltaTime;

        //  2. ȸ�� - ��Ȯ�� ���콺 ���� ��ġ ��� (����ĳ��Ʈ ��� Plane.Raycast ���)
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
