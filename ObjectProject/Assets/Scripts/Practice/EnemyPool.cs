using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemy_prefab;
    public int size = 10;
    public Transform spawnPoint; // 생성 위치

    // 풀로 자주 사용되는 자료구조
    // 1. 리스트(List) : 데이터를 순차적으로 저장하고 추가, 삭제가 자유롭기 때문에 효과적
    // 2. 큐(Queue) : 데이터가 들어온 순서대로 데이터가 빠져나가는 형태의 자료구조
    private List<GameObject> pool;

    private void Start()
    {
        // 총알 생성
        pool = new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            var enemy = Instantiate(enemy_prefab, spawnPoint.position, Quaternion.identity);
            enemy.transform.parent = transform;
            // 생성된 총알은 현재 스크립트를 가진 오브젝트의 자식으로 관리됩니다.

            enemy.SetActive(false); // 비활성화 설정

            enemy.GetComponent<Enemy>().SetPool(this);

            pool.Add(enemy);
            // 리스트명.Add(값) : 리스트에 해당 값을 추가하는 문법
        }
    }

    public GameObject GetEnemy()
    {
        // 비활성화되어있는 총알을 찾아서 활성화합니다.
        foreach (var enemy in pool)
        {
            if (enemy == null) continue;
            // 계층 창에서 활성화가 안되어있다면 (사용하고 있지 않는다면)
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }
        // 총알이 부족한 경우에는 새롭게 만들어서 리스트에 등록합니다.
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
