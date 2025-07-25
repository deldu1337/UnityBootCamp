using UnityEngine;
using UnityEngine.UI;

public class ObjectController : MonoBehaviour
{
    public GameObject player;
    public Text text;
    private float t;
    private float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = 0;
        speed = -0.5f;
        player = GameObject.Find("mini simple skeleton demo");
        text.text = $"{t}";
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);

        // 만약에 낙하물의 y축이 -2보다 작다면 낙하물을 파괴하는 코드
        if (transform.position.y < -2)
        {
            Destroy(gameObject); // Destroy는 너무 빈번한 사용은 안 좋음
                                 // Destroy 발동 시 C# 내의 가비지 컬렉터(GC)가 치움
        }

        // 충돌 판정 처리
        // 원에 의한 충돌 판정 로직 사용
        Vector3 v1 = transform.position; // 낙하물 좌표
        Vector3 v2 = player.transform.position; // 플레이어 좌표

        Vector3 dir = v1 - v2; // v1과 v2 사이의 위치

        float d = dir.magnitude; // 벡터의 크기 또는 길이를 의미합니다.(두 점 사이의 거리를 계산할 때 사용합니다.)

        float obj_r1 = 0.5f;
        float obj_r2 = 1.0f;

        // 두 값 사이의 거리인 d의 값이 설정한 지점들의 합보다 크다면 충돌하지 않는 상황
        if(d < obj_r1 + obj_r2)
        {
            Destroy(gameObject);
        }

        t += 1 * Time.deltaTime;
        text.text = $"{(int)t}";
    }
}
