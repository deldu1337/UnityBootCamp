using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5.0f; // �Ѿ� �̵� �ӵ�
    public float life_time = 2.0f; // �Ѿ� �ݳ� �ð�

    private Transform player_position; // �÷��̾� ��ġ
    private EnemyPool pool; // Ǯ
    private Coroutine life_coroutine;

    // Ǯ ����(Ǯ���� �ش� �� ȣ��)
    public void SetPool(EnemyPool pool)
    {
        this.pool = pool;
    }

    // Ȱ��ȭ �ܰ�
    private void OnEnable()
    {
        life_coroutine = StartCoroutine(EnemytReturn());
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
            float distance = Vector3.Distance(transform.position, player_position.position);

            transform.position = Vector3.MoveTowards(transform.position, player_position.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(transform.position);

            yield return null;
        }
    }

    IEnumerator EnemytReturn()
    {
        yield return new WaitForSeconds(life_time);
        ReturnPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ε��� ����� Enemy �±׸� ������ �ִ� ������Ʈ�� ���
        // �������� �����ϴ�.�� ���� ������ ���� �ڵ� �ۼ�

        // ����Ʈ ����(��ƼŬ)
        

        ReturnPool();
    }

    // �޼ҵ��� ����� 1���� ���, => �� ����� �� �ֽ��ϴ�.
    void ReturnPool() => pool.Return(gameObject);
}
