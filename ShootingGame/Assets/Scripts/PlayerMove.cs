using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    float h, v;

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3 (h, v);

        transform.position += dir * speed * Time.deltaTime;
    }
}
