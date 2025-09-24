//using UnityEngine;

//public class EnemySpawn : MonoBehaviour
//{
//    public GameObject enemyPrefab;      // 생성할 적 프리팹
//    public TileMapGenerator mapGenerator; // 맵 정보를 가져오기 위한 TileMapGenerator
//    public float spawnFactor = 25f;     // 방 크기 대비 적 수 결정 계수

//    void Start()
//    {
//        // mapGenerator 연결 확인
//        if (mapGenerator == null)
//        {
//            Debug.LogError("TileMapGenerator를 연결해주세요!");
//            return;
//        }

//        // 맵 생성 완료 이벤트에 적 생성 함수 연결
//        mapGenerator.OnMapGenerated += GenerateEnemies;

//        // 초기 적 생성
//        GenerateEnemies();
//    }

//    public void GenerateEnemies()
//    {
//        // 기존에 스폰된 적 제거
//        foreach (Transform child in transform)
//            Destroy(child.gameObject);

//        // 각 방마다 적 생성
//        foreach (var room in mapGenerator.GetRooms())
//        {
//            int roomArea = room.width * room.height; // 방 면적 계산
//            int enemyCount = Mathf.Max(1, Mathf.RoundToInt(roomArea / spawnFactor)); // 방 면적 기반 최소 1명 이상

//            int tries = 0;   // 최대 시도 횟수 제한
//            int spawned = 0; // 실제 생성된 적 수

//            // 적 수가 부족하고 시도 제한 내에서 반복
//            while (spawned < enemyCount && tries < enemyCount * 10)
//            {
//                tries++;

//                // 랜덤 위치 결정 (방 내부)
//                float xF = Random.Range(room.xMin + 1, room.xMax - 1);
//                float zF = Random.Range(room.yMin + 1, room.yMax - 1);

//                int x = Mathf.RoundToInt(xF);
//                int z = Mathf.RoundToInt(zF);

//                Vector2Int pos = new Vector2Int(x, z);

//                // 바닥인지 확인하고, 플레이어가 있는 방은 제외
//                if (mapGenerator.IsFloor(x, z) && !mapGenerator.GetPlayerRoom().Contains(pos))
//                {
//                    Vector3 spawnPos = new Vector3(x, 1f, z); // y=1 위치에 스폰
//                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);

//                    // 스폰된 적의 원래 위치 저장
//                    EnemyMove move = enemy.GetComponent<EnemyMove>();
//                    if (move != null)
//                        move.SetSpawnPosition(spawnPos);

//                    spawned++;
//                }
//            }
//        }
//    }

//    // 적 재생성 (예: 플레이어 리스폰 시)
//    public void RespawnEnemies()
//    {
//        GenerateEnemies();
//    }
//}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private BossProximityWatcher bossWatcher;

    [Header("Refs")]
    public TileMapGenerator mapGenerator;
    public StageManager stageManager;

    [Header("스폰 수 조절")]
    public float spawnFactor = 25f; // 방 면적 / spawnFactor = 적 수(최소 1)
    public int bossCount = 1;
    public int triesPerEnemy = 10;

    [Header("스폰 높이/충돌")]
    public float spawnY = 1f;
    public LayerMask obstacleMask;

    [Header("프리팹 매핑")]
    public List<EnemyPrefabPair> prefabPairs = new(); // 인스펙터에서 id ↔ prefab 연결
    private Dictionary<string, GameObject> prefabMap;

    // DB 캐시
    private EnemyDatabase db;

    [System.Serializable]
    public struct EnemyPrefabPair
    {
        public string id;
        public GameObject prefab;
    }

    void Awake()
    {
        prefabMap = prefabPairs
            .Where(p => !string.IsNullOrEmpty(p.id) && p.prefab != null)
            .GroupBy(p => p.id)
            .ToDictionary(g => g.Key, g => g.First().prefab);
    }

    void OnEnable()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator를 연결해주세요!");
            return;
        }
        mapGenerator.OnMapGenerated += GenerateEnemies;
    }

    void OnDisable()
    {
        if (mapGenerator != null)
            mapGenerator.OnMapGenerated -= GenerateEnemies;
    }

    // DB 로드 (한 번)
    private void EnsureDbLoaded()
    {
        if (db != null) return;
        TextAsset json = Resources.Load<TextAsset>("Datas/enemyData");
        if (json == null)
        {
            Debug.LogError("Resources/Datas/enemyData.json이 필요합니다!");
            db = new EnemyDatabase { enemies = new EnemyData[0] };
            return;
        }
        db = JsonUtility.FromJson<EnemyDatabase>(json.text);
        if (db.enemies == null) db.enemies = new EnemyData[0];
    }

    public void GenerateEnemies()
    {
        // 이전 스폰 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        EnsureDbLoaded();

        if (stageManager == null)
        {
            Debug.LogWarning("[EnemySpawn] StageManager 없음 → 스테이지 기반 다양성 미적용");
            return;
        }

        if (stageManager.IsBossStage()) SpawnBossStage();
        else SpawnNormalStage();
    }

    void SpawnNormalStage()
    {
        var stage = stageManager.currentStage;

        // 언락된 일반몹
        var pool = db.enemies
            .Where(e => !e.isBoss && e.unlockStage <= stage && prefabMap.ContainsKey(e.id))
            .ToList();

        if (pool.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawn] Stage {stage}에서 언락된 일반몹 풀 없음");
            return;
        }

        // 가중치 합
        float totalWeight = pool.Sum(e => Mathf.Max(0.0001f, e.weight));

        foreach (var room in mapGenerator.GetRooms())
        {
            int roomArea = room.width * room.height;
            int enemyCount = Mathf.Max(1, Mathf.RoundToInt(roomArea / spawnFactor));

            int spawned = 0, tries = 0;
            while (spawned < enemyCount && tries < enemyCount * triesPerEnemy)
            {
                tries++;
                if (!TryPickPointInRoom(room, out Vector3 pos)) continue;

                // 가중치 랜덤 픽
                float pick = Random.value * totalWeight;
                EnemyData chosen = null;
                float acc = 0f;
                foreach (var e in pool)
                {
                    acc += Mathf.Max(0.0001f, e.weight);
                    if (pick <= acc) { chosen = e; break; }
                }
                if (chosen == null) chosen = pool[pool.Count - 1];

                SpawnById(chosen.id, pos);
                spawned++;
            }
        }
    }

    //void SpawnBossStage()
    //{
    //    // 항상 1마리
    //    var stage = stageManager.currentStage;

    //    var bosses = db.enemies
    //        .Where(e => e.isBoss && e.unlockStage <= stage && prefabMap.ContainsKey(e.id))
    //        .ToList();

    //    if (bosses.Count == 0)
    //    {
    //        Debug.LogWarning($"[EnemySpawn] Stage {stage} 보스 풀 없음 → 일반 스폰");
    //        SpawnNormalStage();
    //        return;
    //    }

    //    var br = mapGenerator.GetBossRoom();
    //    if (br.width <= 0 || br.height <= 0)
    //    {
    //        Debug.LogWarning("[EnemySpawn] 보스방 미정의 → 일반 스폰");
    //        SpawnNormalStage();
    //        return;
    //    }

    //    // 보스방 중심 또는 내부 바닥 타일로 스폰
    //    Vector2Int c = new Vector2Int(Mathf.RoundToInt(br.center.x), Mathf.RoundToInt(br.center.y));
    //    Vector3 pos;
    //    if (mapGenerator.IsFloor(c.x, c.y))
    //        pos = new Vector3(c.x, spawnY, c.y);
    //    else if (!TryPickPointInRoom(br, out pos))
    //        pos = new Vector3(br.xMin + br.width / 2f, spawnY, br.yMin + br.height / 2f);

    //    var boss = bosses[Random.Range(0, bosses.Count)];
    //    //SpawnById(boss.id, pos);
    //    GameObject go = SpawnById(boss.id, pos);   // 반환 받기
    //    if (go != null)
    //    {
    //        var esm = go.GetComponent<EnemyStatsManager>();
    //        if (bossWatcher == null) bossWatcher = FindAnyObjectByType<BossProximityWatcher>();
    //        if (bossWatcher != null && esm != null)
    //            bossWatcher.SetBoss(esm);
    //    }
    //}
    void SpawnBossStage()
    {
        var stage = stageManager.currentStage;

        var bosses = db.enemies
            .Where(e => e.isBoss && e.unlockStage <= stage && prefabMap.ContainsKey(e.id))
            .ToList();

        if (bosses.Count == 0) { Debug.LogWarning($"[EnemySpawn] Stage {stage} 보스 풀 없음 → 일반 스폰"); SpawnNormalStage(); return; }

        var br = mapGenerator.GetBossRoom();
        if (br.width <= 0 || br.height <= 0) { Debug.LogWarning("[EnemySpawn] 보스방 미정의 → 일반 스폰"); SpawnNormalStage(); return; }

        Vector2Int c = new Vector2Int(Mathf.RoundToInt(br.center.x), Mathf.RoundToInt(br.center.y));
        Vector3 pos;
        if (mapGenerator.IsFloor(c.x, c.y)) pos = new Vector3(c.x, spawnY, c.y);
        else if (!TryPickPointInRoom(br, out pos)) pos = new Vector3(br.xMin + br.width / 2f, spawnY, br.yMin + br.height / 2f);

        var boss = bosses[Random.Range(0, bosses.Count)];

        // ★ 보스만 Boss 태그!
        GameObject go = SpawnById(boss.id, pos, markAsBoss: true);
        if (go == null) { Debug.LogError("[EnemySpawn] 보스 스폰 실패"); return; }

        var esm = go.GetComponent<EnemyStatsManager>();
        if (esm == null) { Debug.LogError("[EnemySpawn] EnemyStatsManager 누락"); return; }

        if (bossWatcher == null) bossWatcher = FindAnyObjectByType<BossProximityWatcher>();
        if (bossWatcher != null) bossWatcher.SetBoss(esm);
    }


    // 반환형 GameObject 권장
    GameObject SpawnById(string enemyId, Vector3 position, bool markAsBoss = false)
    {
        if (!prefabMap.TryGetValue(enemyId, out var prefab))
        {
            Debug.LogWarning($"[EnemySpawn] '{enemyId}' 프리팹 매핑이 없습니다.");
            return null;
        }

        var go = Instantiate(prefab, position, Quaternion.identity, transform);

        var esm = go.GetComponent<EnemyStatsManager>();
        if (esm != null) esm.enemyId = enemyId;

        var move = go.GetComponent<EnemyMove>();
        if (move != null) move.SetSpawnPosition(position);

        // 보스일 때만 태그 지정
        if (markAsBoss) go.tag = "Boss";

        return go;
    }


    bool TryPickPointInRoom(RectInt room, out Vector3 pos)
    {
        for (int t = 0; t < triesPerEnemy; t++)
        {
            int x = Random.Range(room.xMin + 1, room.xMax - 1);
            int z = Random.Range(room.yMin + 1, room.yMax - 1);

            if (!mapGenerator.IsFloor(x, z)) continue;

            var pr = mapGenerator.GetPlayerRoom();
            if (pr.Contains(new Vector2Int(x, z))) continue;

            Vector3 candidate = new Vector3(x, spawnY, z);

            if (obstacleMask.value != 0 &&
                Physics.CheckSphere(candidate, 0.4f, obstacleMask))
                continue;

            pos = candidate; return true;
        }
        pos = default; return false;
    }
}
