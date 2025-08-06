using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject unitPrefab; // 유닛 프리팹
    public Transform spawnPoint; // 생성 위치
    public float interval = 5.0f; // 유닛 생성 간격
    public EnemyPool pool;

    private float time;

    private void Start()
    {
        //StartCoroutine(Spawn());
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time > interval)
        {
            pool.GetEnemy();
            Debug.Log($"{spawnPoint.name}에서 {unitPrefab.name}이 생성되었습니다.");
            time = 0;
        }
    }

    //IEnumerator Spawn()
    //{
    //    while (true)
    //    {
    //        // 유닛을 생성합니다.
    //        // 생성 위치는 spawnPoint로부터 받습니다.
    //        //Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);

    //        //Debug.Log($"{spawnPoint.name}에서 {unitPrefab.name}이 생성되었습니다.");

    //        //// 생성 간격 만큼 대기합니다.
    //        //yield return new WaitForSeconds(interval);
    //        pool.GetEnemy();

    //        Debug.Log($"{spawnPoint.name}에서 {unitPrefab.name}이 생성되었습니다.");

    //        yield return new WaitForSeconds(interval);
    //    }
    //}
}
