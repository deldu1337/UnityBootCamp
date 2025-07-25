using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 게임 오브젝트 타입 변수 player(공개)
    public GameObject player;

    // 카메라와 플레이어 사이의 변수 offset(Vector3 : 비공개)
    private Vector3 offset;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 카메라와 플레이어의 거리 차이를 offset의 값으로 설정합니다.
        offset = transform.position - player.transform.position;
    }

    // Update()에서 처리할 부분을 다 처리한 후에 호출되는 위치
    // 카메라 작업에서 자주 사용됨 (오브젝트 추적이 목적인 경우)
    void LateUpdate()
    {
        // 카메라의 위치는 플레이어와의 일정 거리를 유지한다. (offset)
        transform.position = player.transform.position + offset;
    }
}
