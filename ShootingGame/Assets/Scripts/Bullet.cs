using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5.0f;
    public float life_time = 2.0f; // 총알 반납 시간

    private Coroutine life_coroutine;
    private BulletPool pool; // 풀
    public void SetPool(BulletPool pool)
    {
        this.pool = pool;
    }

    // 활성화 단계
    private void OnEnable()
    {
        life_coroutine = StartCoroutine(BulletReturn());
    }

    // 비활성화 단계
    private void OnDisable()
    {
        // if문 작성 시 명령문이 1줄일 경우 {} 생략 가능합니다.
        if (life_coroutine != null)
            StopCoroutine(life_coroutine);
    }

    void Update()
    {
        Vector3 dir = Vector3.up;

        transform.position += dir * speed * Time.deltaTime;
    }

    IEnumerator BulletReturn()
    {
        yield return new WaitForSeconds(life_time);
        ReturnPool();
    }

    void ReturnPool() => pool.Return(gameObject);
}
