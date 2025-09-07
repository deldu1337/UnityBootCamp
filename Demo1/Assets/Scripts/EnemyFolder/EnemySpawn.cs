using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;      // 생성할 적 프리팹
    public TileMapGenerator mapGenerator; // 맵 정보를 가져오기 위한 TileMapGenerator
    public float spawnFactor = 25f;     // 방 크기 대비 적 수 결정 계수

    void Start()
    {
        // mapGenerator 연결 확인
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator를 연결해주세요!");
            return;
        }

        // 맵 생성 완료 이벤트에 적 생성 함수 연결
        mapGenerator.OnMapGenerated += GenerateEnemies;

        // 초기 적 생성
        GenerateEnemies();
    }

    public void GenerateEnemies()
    {
        // 기존에 스폰된 적 제거
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 각 방마다 적 생성
        foreach (var room in mapGenerator.GetRooms())
        {
            int roomArea = room.width * room.height; // 방 면적 계산
            int enemyCount = Mathf.Max(1, Mathf.RoundToInt(roomArea / spawnFactor)); // 방 면적 기반 최소 1명 이상

            int tries = 0;   // 최대 시도 횟수 제한
            int spawned = 0; // 실제 생성된 적 수

            // 적 수가 부족하고 시도 제한 내에서 반복
            while (spawned < enemyCount && tries < enemyCount * 10)
            {
                tries++;

                // 랜덤 위치 결정 (방 내부)
                float xF = Random.Range(room.xMin + 1, room.xMax - 1);
                float zF = Random.Range(room.yMin + 1, room.yMax - 1);

                int x = Mathf.RoundToInt(xF);
                int z = Mathf.RoundToInt(zF);

                Vector2Int pos = new Vector2Int(x, z);

                // 바닥인지 확인하고, 플레이어가 있는 방은 제외
                if (mapGenerator.IsFloor(x, z) && !mapGenerator.GetPlayerRoom().Contains(pos))
                {
                    Vector3 spawnPos = new Vector3(x, 1f, z); // y=1 위치에 스폰
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);

                    // 스폰된 적의 원래 위치 저장
                    EnemyMove move = enemy.GetComponent<EnemyMove>();
                    if (move != null)
                        move.SetSpawnPosition(spawnPos);

                    spawned++;
                }
            }
        }
    }

    // 적 재생성 (예: 플레이어 리스폰 시)
    public void RespawnEnemies()
    {
        GenerateEnemies();
    }
}