using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject unitPrefab; // ���� ������
    public Transform spawnPoint; // ���� ��ġ
    public float interval = 5.0f; // ���� ���� ����
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
            Debug.Log($"{spawnPoint.name}���� {unitPrefab.name}�� �����Ǿ����ϴ�.");
            time = 0;
        }
    }

    //IEnumerator Spawn()
    //{
    //    while (true)
    //    {
    //        // ������ �����մϴ�.
    //        // ���� ��ġ�� spawnPoint�κ��� �޽��ϴ�.
    //        //Instantiate(unitPrefab, spawnPoint.position, Quaternion.identity);

    //        //Debug.Log($"{spawnPoint.name}���� {unitPrefab.name}�� �����Ǿ����ϴ�.");

    //        //// ���� ���� ��ŭ ����մϴ�.
    //        //yield return new WaitForSeconds(interval);
    //        pool.GetEnemy();

    //        Debug.Log($"{spawnPoint.name}���� {unitPrefab.name}�� �����Ǿ����ϴ�.");

    //        yield return new WaitForSeconds(interval);
    //    }
    //}
}
