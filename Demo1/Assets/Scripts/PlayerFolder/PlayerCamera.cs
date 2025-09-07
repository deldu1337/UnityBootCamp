using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 distance; // 카메라와 플레이어 간 상대 위치 (offset)

    void Start()
    {
        // 초기 카메라 위치를 기준으로 offset 계산
        Vector3 vector3 = new Vector3(-2f, 17f, -2f); // 카메라 초기 위치
        distance = vector3 - transform.position;       // 플레이어 위치 기준 offset 계산
        // 주석 처리됨, 고정된 초기 벡터로 offset 설정
    }

    void FixedUpdate()
    {
        // 카메라 위치를 플레이어 위치 + offset으로 설정
        Camera.main.transform.position = distance + transform.position;

        // 현재 카메라 위치 기준 offset 갱신
        distance = Camera.main.transform.position - transform.position;
    }
}