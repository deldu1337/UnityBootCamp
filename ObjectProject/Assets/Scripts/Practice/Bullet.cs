using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

// 총알에 대한 정보, 총알 반납, 총알 이동
public class Bullet : MonoBehaviour
{
    public float speed = 500f; // 총알 이동 속도
    public float life_time = 2.0f; // 총알 반납 시간
    public float damage = 20.0f; // 총알 데미지
    public GameObject effect_prefab; // 이펙트 프리팹

    private BulletPool pool; // 풀
    private EffectPool effect_pool; // 풀
    private Coroutine life_coroutine;

    private HP hp;


    // 풀 설정(풀에서 해당 값 호출)
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
        if(life_coroutine != null)
            StopCoroutine(life_coroutine);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime; 
    }

    IEnumerator BulletReturn()
    {
        yield return new WaitForSeconds(life_time);
        ReturnPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 대상이 Enemy 태그를 가지고 있는 오브젝트일 경우
        // 데미지를 입힙니다.와 같은 데미지 관련 코드 작성

        //hp.Damage(damage);
        // 이펙트 연출(파티클)
        hp = other.GetComponent<HP>();
        if (hp != null)
        {
            Debug.Log($"[총알] {other.gameObject.name} 에게 {damage} 데미지!");
            hp.Damage(damage); // 체력 감소
        }

        if (effect_prefab != null)
        {
            Instantiate(effect_prefab, transform.position, Quaternion.identity);

        }

        ReturnEffectPool();
        ReturnPool();
    }

    // 메소드의 명령이 1줄일 경우, => 로 사용할 수 있습니다.
    void ReturnPool() => pool.Return(gameObject);
    void ReturnEffectPool() => effect_pool.Return(gameObject);
}
