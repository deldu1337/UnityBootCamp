using UnityEngine;

public class AroundRotate : MonoBehaviour
{
    public Transform pivot; // 회전의 중심점
    public float speed = 1f;

    void Update()
    {
        transform.RotateAround(pivot.position, Vector3.up, speed);
    }
}
