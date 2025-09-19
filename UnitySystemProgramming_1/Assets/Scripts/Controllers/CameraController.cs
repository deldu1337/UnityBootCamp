using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Define.CameraMode _mode = Define.CameraMode.QuarterView;

    [SerializeField]
    private Vector3 _delta = Vector3.zero;

    [SerializeField]
    private PlayerController _player = null;

    void Start()
    {
        
    }

    void Update()
    {
        if (_mode == Define.CameraMode.QuarterView)
        {
            transform.position = _player.transform.position + _delta;
            transform.LookAt(_player.transform);
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }
}
