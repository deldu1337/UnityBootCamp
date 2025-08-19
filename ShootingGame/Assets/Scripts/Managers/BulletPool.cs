using System.Collections.Generic;
using UnityEngine;

// ���� ����ϴ� ������Ʈ�� �̸� ������ ���� Ȱ��ȭ/��Ȱ��ȭ�� �ϴ� ���
// Destroy�� �� �� �߻��ϴ� GC�� ���� ���� ���ϸ� �����ϱ� ���� ������Ʈ Ǯ ���
public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int size = 20;
    public Transform spawnPoint;

    private List<GameObject> pool;

    private void Start()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            var bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
            bullet.transform.parent = transform;

            bullet.SetActive(false);

            bullet.GetComponent<Bullet>().SetPool(this);

            pool.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        foreach (var bullet in pool)
        {
            if (bullet == null) continue;

            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        var new_bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        new_bullet.transform.parent = transform;
        new_bullet.GetComponent<Bullet>().SetPool(this);
        pool.Add(new_bullet);
        return new_bullet;
    }

    public void Return(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}
