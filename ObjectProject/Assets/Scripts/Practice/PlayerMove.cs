using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    public float speed = 3.0f;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 pos = new Vector3(vertical,0,-horizontal);

        if (pos != Vector3.zero)
        {
            float angle = Mathf.Atan2(-pos.z, pos.x) * Mathf.Rad2Deg;
            Quaternion a = Quaternion.Euler(0, angle, 0);
            // 즉시 회전
            //transform.rotation = a;
            // 부드러운 회전 
            transform.rotation = Quaternion.Slerp(transform.rotation, a, speed * Time.fixedDeltaTime);
        }

        transform.position += pos * speed * Time.deltaTime;
    }
}
