using System.Collections.Generic;
using UnityEngine;

// 자주 사용하는 오브젝트를 미리 생성해 놓고 활성화/비활성화만 하는 방식
// Destroy를 한 후 발생하는 GC로 인한 성능 저하를 방지하기 위해 오브젝트 풀 사용
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
