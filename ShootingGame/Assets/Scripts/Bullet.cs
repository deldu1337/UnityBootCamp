using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5.0f;
    public float life_time = 2.0f; // �Ѿ� �ݳ� �ð�

    private Coroutine life_coroutine;
    private BulletPool pool; // Ǯ
    public void SetPool(BulletPool pool)
    {
        this.pool = pool;
    }

    // Ȱ��ȭ �ܰ�
    private void OnEnable()
    {
        life_coroutine = StartCoroutine(BulletReturn());
    }

    // ��Ȱ��ȭ �ܰ�
    private void OnDisable()
    {
        // if�� �ۼ� �� ��ɹ��� 1���� ��� {} ���� �����մϴ�.
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
