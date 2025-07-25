using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;

    float spawnTime = 2.0f; // 2초 당 생성
    float time = 0.0f; // 시간 체크용 변수

    // 시간을 따로 계산해서, 변수로 저장하고
    // 그 변수가 스폰 타임보다 커지면 오브젝트 생성
    // 변수를 0으로 초기화

    

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > spawnTime)
        {
            GameObject go = Instantiate(objectPrefab);
            time = 0.0f;
            int rand = Random.Range(-3, 4);
            go.transform.position = new Vector3(rand, 5, 0);
        }
    }
}
