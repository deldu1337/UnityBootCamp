using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 distance;
    void Start()
    {
        Vector3 vector3 = new Vector3(1.5f, 10f, 2.5f);
        //distance = Camera.main.transform.position - transform.position;
        distance = vector3 - transform.position;
    }

    void FixedUpdate()
    {
        Camera.main.transform.position = distance + transform.position;
        distance = Camera.main.transform.position - transform.position;
    }
}