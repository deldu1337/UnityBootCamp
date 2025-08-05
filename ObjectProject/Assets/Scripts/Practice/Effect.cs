using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private EffectPool pool; // Ǯ
    private Coroutine life_coroutine;

    // Ǯ ����(Ǯ���� �ش� �� ȣ��)
    public void SetPool(EffectPool pool)
    {
        this.pool = pool;
    }

    // Ȱ��ȭ �ܰ�
    private void OnEnable()
    {
        life_coroutine = StartCoroutine(EffecttReturn());
    }

    // ��Ȱ��ȭ �ܰ�
    private void OnDisable()
    {
        // if�� �ۼ� �� ��ɹ��� 1���� ��� {} ���� �����մϴ�.
        if (life_coroutine != null)
            StopCoroutine(life_coroutine);
    }

    IEnumerator EffecttReturn()
    {
        yield return new WaitForSeconds(2);
        ReturnPool();
    }

    // �޼ҵ��� ����� 1���� ���, => �� ����� �� �ֽ��ϴ�.
    void ReturnPool() => pool.Return(gameObject);
}
