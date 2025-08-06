using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5.0f; // 총알 이동 속도
    public float damage = 20.0f; // 적 데미지

    private Transform player_position; // 플레이어 위치
    private EnemyPool pool; // 풀
    private Coroutine life_coroutine;

    private HP hp;

    // 풀 설정(풀에서 해당 값 호출)
    public void SetPool(EnemyPool pool)
    {
        this.pool = pool;
    }

    // 비활성화 단계
    private void OnDisable()
    {
        // if문 작성 시 명령문이 1줄일 경우 {} 생략 가능합니다.
        if (life_coroutine != null)
            StopCoroutine(life_coroutine);
    }

    void Start()
    {
        player_position = GameObject.FindGameObjectWithTag("Player")?.transform;
        hp = GameObject.FindGameObjectWithTag("Player").GetComponent<HP>();

        if (player_position != null)
        {
            StartCoroutine(EnemyMove());
        }
        else
        {
            Debug.LogWarning("게임 내에서 플레이어를 찾을 수 없습니다.");
        }
    }

    IEnumerator EnemyMove()
    {
        while (player_position != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player_position.position, speed * Time.deltaTime);
            Vector3 a = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            transform.rotation = Quaternion.LookRotation(a);

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 대상이 Enemy 태그를 가지고 있는 오브젝트일 경우
        // 데미지를 입힙니다.와 같은 데미지 관련 코드 작성

        // 이펙트 연출(파티클)
        if (other.gameObject.CompareTag("Player"))
        {
            if (hp != null)
            {
                //Debug.Log($"[플레이어] {gameObject.name} 에게 {damage} 데미지!");
                hp.Damage(damage); // 체력 감소
            }

            Destroy(gameObject);
        }
    }

    // 메소드의 명령이 1줄일 경우, => 로 사용할 수 있습니다.
    void ReturnPool() => pool.Return(gameObject);
}
