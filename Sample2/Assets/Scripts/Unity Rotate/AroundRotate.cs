using UnityEngine;

public class AroundRotate : MonoBehaviour
{
    public Transform pivot; // ȸ���� �߽���
    public float speed = 1f;

    void Update()
    {
        transform.RotateAround(pivot.position, Vector3.up, speed);
    }
}
