using UnityEngine;
// 1. 프래팹을 직접 등록한다.
// 2. 생성된 오브젝트에 대한 정보를 내부에서 가진다.
// 3. 생성 후에 파괴에 대한 딜레이 시간을 가진다.

// 해당 스크립트를 가진 오브젝트가 실행하면, 생성을 진행하고 일정 시간 뒤 파괴하도록 처리합니다.
// 조건) 프리팹이 등록이 되어있을 때 해당 작업 수행, 아닐 경우 경고 메세지를 안내합니다.

// 오브젝트 복사본 1개 (prefab), 내부에서 저장하는 값 1개 (spawned)

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab;
    private GameObject spawned;
    public float delay = 3.0f;

    void Start()
    {
        if(prefab != null)
        {
            spawned = Instantiate(prefab);
            // 생성 코드 Instantiate
            // 1. Instantiate(prefab): 해당 프리팹의 정보에 맞게 위치와 회전 값 등이 설정되고 생성됩니다.
            // 2. Instantiate(prefab, transform.position, Quaternion.identity);
            // --> 해당 프리팹을 설정하고, 위치와 회전 값의 정보대로 오브젝트의 위치와 회전 값을 설정합니다.
            // 3. Instantiate(prefab, parent);
            // 오브젝트를 생성하고, 그 오브젝트를 전달한 위치의 자식으로써 등록합니다.
            // 4. Instantiate(prefab, transform.position, Quaternion.identity, parent);

            spawned.name = "생성된 오브젝트";
            // 생성된 오브젝트의 이름을 변경하는 코드

            //spawned.AddComponent<Rigidbody>();
            // 그 후 생성된 오브젝트에 컴포넌트를 추가합니다.

            Debug.Log(spawned.name + "이 생성되었습니다.");

            Destroy(spawned, delay); // delay 시간 이후 오브젝트 파괴
        }
        else
        {
            Debug.LogWarning("등록된 프리팹이 따로 없습니다.");
        }
    }
}
