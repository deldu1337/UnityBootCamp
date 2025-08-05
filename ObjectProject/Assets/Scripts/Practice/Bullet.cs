using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

// �Ѿ˿� ���� ����, �Ѿ� �ݳ�, �Ѿ� �̵�
public class Bullet : MonoBehaviour
{
    public float speed = 500f; // �Ѿ� �̵� �ӵ�
    public float life_time = 2.0f; // �Ѿ� �ݳ� �ð�
    public float damage = 20.0f; // �Ѿ� ������
    public GameObject effect_prefab; // ����Ʈ ������

    private BulletPool pool; // Ǯ
    private EffectPool effect_pool; // Ǯ
    private Coroutine life_coroutine;

    private HP hp;


    // Ǯ ����(Ǯ���� �ش� �� ȣ��)
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
        // �ε��� ����� Enemy �±׸� ������ �ִ� ������Ʈ�� ���
        // �������� �����ϴ�.�� ���� ������ ���� �ڵ� �ۼ�

        //hp.Damage(damage);
        // ����Ʈ ����(��ƼŬ)
        hp = other.GetComponent<HP>();
        if (hp != null)
        {
            Debug.Log($"[�Ѿ�] {other.gameObject.name} ���� {damage} ������!");
            hp.Damage(damage); // ü�� ����
        }

        if (effect_prefab != null)
        {
            Instantiate(effect_prefab, transform.position, Quaternion.identity);

        }

        ReturnEffectPool();
        ReturnPool();
    }

    // �޼ҵ��� ����� 1���� ���, => �� ����� �� �ֽ��ϴ�.
    void ReturnPool() => pool.Return(gameObject);
    void ReturnEffectPool() => effect_pool.Return(gameObject);
}
