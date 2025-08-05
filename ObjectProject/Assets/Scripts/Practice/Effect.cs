using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private EffectPool pool; // 풀
    private Coroutine life_coroutine;

    // 풀 설정(풀에서 해당 값 호출)
    public void SetPool(EffectPool pool)
    {
        this.pool = pool;
    }

    // 활성화 단계
    private void OnEnable()
    {
        life_coroutine = StartCoroutine(EffecttReturn());
    }

    // 비활성화 단계
    private void OnDisable()
    {
        // if문 작성 시 명령문이 1줄일 경우 {} 생략 가능합니다.
        if (life_coroutine != null)
            StopCoroutine(life_coroutine);
    }

    IEnumerator EffecttReturn()
    {
        yield return new WaitForSeconds(2);
        ReturnPool();
    }

    // 메소드의 명령이 1줄일 경우, => 로 사용할 수 있습니다.
    void ReturnPool() => pool.Return(gameObject);
}
