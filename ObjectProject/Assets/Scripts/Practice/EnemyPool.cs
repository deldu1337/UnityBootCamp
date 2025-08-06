using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemy_prefab;
    public int size = 10;
    public Transform spawnPoint; // ���� ��ġ

    // Ǯ�� ���� ���Ǵ� �ڷᱸ��
    // 1. ����Ʈ(List) : �����͸� ���������� �����ϰ� �߰�, ������ �����ӱ� ������ ȿ����
    // 2. ť(Queue) : �����Ͱ� ���� ������� �����Ͱ� ���������� ������ �ڷᱸ��
    private List<GameObject> pool;

    private void Start()
    {
        // �Ѿ� ����
        pool = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            var enemy = Instantiate(enemy_prefab, spawnPoint.position, Quaternion.identity);
            enemy.transform.parent = transform;
            // ������ �Ѿ��� ���� ��ũ��Ʈ�� ���� ������Ʈ�� �ڽ����� �����˴ϴ�.

            enemy.SetActive(false); // ��Ȱ��ȭ ����

            enemy.GetComponent<Enemy>().SetPool(this);

            pool.Add(enemy);
            // ����Ʈ��.Add(��) : ����Ʈ�� �ش� ���� �߰��ϴ� ����
        }
    }

    public GameObject GetEnemy()
    {
        // ��Ȱ��ȭ�Ǿ��ִ� �Ѿ��� ã�Ƽ� Ȱ��ȭ�մϴ�.
        foreach (var enemy in pool)
        {
            if (enemy == null) continue;
            // ���� â���� Ȱ��ȭ�� �ȵǾ��ִٸ� (����ϰ� ���� �ʴ´ٸ�)
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }
        // �Ѿ��� ������ ��쿡�� ���Ӱ� ���� ����Ʈ�� ����մϴ�.
        var new_enemy = Instantiate(enemy_prefab, spawnPoint.position, Quaternion.identity);
        new_enemy.transform.parent = transform;
        new_enemy.GetComponent<Enemy>().SetPool(this);
        pool.Add(new_enemy);
        return new_enemy;
    }

    public void Return(GameObject enemy)
    {
        enemy.SetActive(false);
    }
}
