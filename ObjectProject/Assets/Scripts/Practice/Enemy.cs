using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5.0f; // �Ѿ� �̵� �ӵ�
    public float damage = 20.0f; // �� ������

    private Transform player_position; // �÷��̾� ��ġ
    private EnemyPool pool; // Ǯ
    private Coroutine life_coroutine;

    private HP hp;

    // Ǯ ����(Ǯ���� �ش� �� ȣ��)
    public void SetPool(EnemyPool pool)
    {
        this.pool = pool;
    }

    // ��Ȱ��ȭ �ܰ�
    private void OnDisable()
    {
        // if�� �ۼ� �� ��ɹ��� 1���� ��� {} ���� �����մϴ�.
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
            Debug.LogWarning("���� ������ �÷��̾ ã�� �� �����ϴ�.");
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
        // �ε��� ����� Enemy �±׸� ������ �ִ� ������Ʈ�� ���
        // �������� �����ϴ�.�� ���� ������ ���� �ڵ� �ۼ�

        // ����Ʈ ����(��ƼŬ)
        if (other.gameObject.CompareTag("Player"))
        {
            if (hp != null)
            {
                //Debug.Log($"[�÷��̾�] {gameObject.name} ���� {damage} ������!");
                hp.Damage(damage); // ü�� ����
            }

            Destroy(gameObject);
        }
    }

    // �޼ҵ��� ����� 1���� ���, => �� ����� �� �ֽ��ϴ�.
    void ReturnPool() => pool.Return(gameObject);
}
