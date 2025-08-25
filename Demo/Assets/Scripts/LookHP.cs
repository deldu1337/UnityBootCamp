using UnityEngine;

public class LookHP : MonoBehaviour
{
    private Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
    }

    void LateUpdate()
    {
        if (_cam == null) return;

        // ī�޶� ȸ���� �״�� ���� (���Ӻ� �������� ����)
        transform.rotation = _cam.transform.rotation;
    }
}
