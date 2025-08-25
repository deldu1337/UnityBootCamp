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

        // 카메라 회전을 그대로 적용 (게임뷰 정면으로 고정)
        transform.rotation = _cam.transform.rotation;
    }
}
