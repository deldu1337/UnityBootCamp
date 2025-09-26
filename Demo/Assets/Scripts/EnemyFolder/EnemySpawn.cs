//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class EnemySpawn : MonoBehaviour
//{
//    [SerializeField] private BossProximityWatcher bossWatcher;

//    [Header("Refs")]
//    public TileMapGenerator mapGenerator;
//    public StageManager stageManager;

//    [Header("���� �� ����")]
//    public float spawnFactor = 25f; // �� ���� / spawnFactor = �� ��(�ּ� 1)
//    public int bossCount = 1;
//    public int triesPerEnemy = 10;

//    [Header("���� ����/�浹")]
//    public float spawnY = 1f;
//    public LayerMask obstacleMask;

//    [Header("������ ����")]
//    public List<EnemyPrefabPair> prefabPairs = new(); // �ν����Ϳ��� id �� prefab ����
//    private Dictionary<string, GameObject> prefabMap;

//    // DB ĳ��
//    private EnemyDatabase db;

//    [System.Serializable]
//    public struct EnemyPrefabPair
//    {
//        public string id;
//        public GameObject prefab;
//    }

//    void Awake()
//    {
//        prefabMap = prefabPairs
//            .Where(p => !string.IsNullOrEmpty(p.id) && p.prefab != null)
//            .GroupBy(p => p.id)
//            .ToDictionary(g => g.Key, g => g.First().prefab);
//    }

//    void OnEnable()
//    {
//        if (mapGenerator == null)
//        {
//            Debug.LogError("TileMapGenerator�� �������ּ���!");
//            return;
//        }
//        mapGenerator.OnMapGenerated += GenerateEnemies;
//    }

//    void OnDisable()
//    {
//        if (mapGenerator != null)
//            mapGenerator.OnMapGenerated -= GenerateEnemies;
//    }

//    // DB �ε� (�� ��)
//    private void EnsureDbLoaded()
//    {
//        if (db != null) return;
//        TextAsset json = Resources.Load<TextAsset>("Datas/enemyData");
//        if (json == null)
//        {
//            Debug.LogError("Resources/Datas/enemyData.json�� �ʿ��մϴ�!");
//            db = new EnemyDatabase { enemies = new EnemyData[0] };
//            return;
//        }
//        db = JsonUtility.FromJson<EnemyDatabase>(json.text);
//        if (db.enemies == null) db.enemies = new EnemyData[0];
//    }

//    public void GenerateEnemies()
//    {
//        // ���� ���� ����
//        for (int i = transform.childCount - 1; i >= 0; i--)
//            Destroy(transform.GetChild(i).gameObject);

//        EnsureDbLoaded();

//        if (stageManager == null)
//        {
//            Debug.LogWarning("[EnemySpawn] StageManager ���� �� �������� ��� �پ缺 ������");
//            return;
//        }

//        if (stageManager.IsBossStage()) SpawnBossStage();
//        else SpawnNormalStage();
//    }

//    void SpawnNormalStage()
//    {
//        var stage = stageManager.currentStage;

//        // ����� �Ϲݸ�
//        var pool = db.enemies
//            .Where(e => !e.isBoss && e.unlockStage <= stage && prefabMap.ContainsKey(e.id))
//            .ToList();

//        if (pool.Count == 0)
//        {
//            Debug.LogWarning($"[EnemySpawn] Stage {stage}���� ����� �Ϲݸ� Ǯ ����");
//            return;
//        }

//        // ����ġ ��
//        float totalWeight = pool.Sum(e => Mathf.Max(0.0001f, e.weight));

//        foreach (var room in mapGenerator.GetRooms())
//        {
//            int roomArea = room.width * room.height;
//            int enemyCount = Mathf.Max(1, Mathf.RoundToInt(roomArea / spawnFactor));

//            int spawned = 0, tries = 0;
//            while (spawned < enemyCount && tries < enemyCount * triesPerEnemy)
//            {
//                tries++;
//                if (!TryPickPointInRoom(room, out Vector3 pos)) continue;

//                // ����ġ ���� ��
//                float pick = Random.value * totalWeight;
//                EnemyData chosen = null;
//                float acc = 0f;
//                foreach (var e in pool)
//                {
//                    acc += Mathf.Max(0.0001f, e.weight);
//                    if (pick <= acc) { chosen = e; break; }
//                }
//                if (chosen == null) chosen = pool[pool.Count - 1];

//                SpawnById(chosen.id, pos);
//                spawned++;
//            }
//        }
//    }

//    //void SpawnBossStage()
//    //{
//    //    // �׻� 1����
//    //    var stage = stageManager.currentStage;

//    //    var bosses = db.enemies
//    //        .Where(e => e.isBoss && e.unlockStage <= stage && prefabMap.ContainsKey(e.id))
//    //        .ToList();

//    //    if (bosses.Count == 0)
//    //    {
//    //        Debug.LogWarning($"[EnemySpawn] Stage {stage} ���� Ǯ ���� �� �Ϲ� ����");
//    //        SpawnNormalStage();
//    //        return;
//    //    }

//    //    var br = mapGenerator.GetBossRoom();
//    //    if (br.width <= 0 || br.height <= 0)
//    //    {
//    //        Debug.LogWarning("[EnemySpawn] ������ ������ �� �Ϲ� ����");
//    //        SpawnNormalStage();
//    //        return;
//    //    }

//    //    // ������ �߽� �Ǵ� ���� �ٴ� Ÿ�Ϸ� ����
//    //    Vector2Int c = new Vector2Int(Mathf.RoundToInt(br.center.x), Mathf.RoundToInt(br.center.y));
//    //    Vector3 pos;
//    //    if (mapGenerator.IsFloor(c.x, c.y))
//    //        pos = new Vector3(c.x, spawnY, c.y);
//    //    else if (!TryPickPointInRoom(br, out pos))
//    //        pos = new Vector3(br.xMin + br.width / 2f, spawnY, br.yMin + br.height / 2f);

//    //    var boss = bosses[Random.Range(0, bosses.Count)];
//    //    //SpawnById(boss.id, pos);
//    //    GameObject go = SpawnById(boss.id, pos);   // ��ȯ �ޱ�
//    //    if (go != null)
//    //    {
//    //        var esm = go.GetComponent<EnemyStatsManager>();
//    //        if (bossWatcher == null) bossWatcher = FindAnyObjectByType<BossProximityWatcher>();
//    //        if (bossWatcher != null && esm != null)
//    //            bossWatcher.SetBoss(esm);
//    //    }
//    //}
//    void SpawnBossStage()
//    {
//        var stage = stageManager.currentStage;

//        var bosses = db.enemies
//            .Where(e => e.isBoss && e.unlockStage <= stage && prefabMap.ContainsKey(e.id))
//            .ToList();

//        if (bosses.Count == 0) { Debug.LogWarning($"[EnemySpawn] Stage {stage} ���� Ǯ ���� �� �Ϲ� ����"); SpawnNormalStage(); return; }

//        var br = mapGenerator.GetBossRoom();
//        if (br.width <= 0 || br.height <= 0) { Debug.LogWarning("[EnemySpawn] ������ ������ �� �Ϲ� ����"); SpawnNormalStage(); return; }

//        Vector2Int c = new Vector2Int(Mathf.RoundToInt(br.center.x), Mathf.RoundToInt(br.center.y));
//        Vector3 pos;
//        if (mapGenerator.IsFloor(c.x, c.y)) pos = new Vector3(c.x, spawnY, c.y);
//        else if (!TryPickPointInRoom(br, out pos)) pos = new Vector3(br.xMin + br.width / 2f, spawnY, br.yMin + br.height / 2f);

//        var boss = bosses[Random.Range(0, bosses.Count)];

//        // �� ������ Boss �±�!
//        GameObject go = SpawnById(boss.id, pos, markAsBoss: true);
//        if (go == null) { Debug.LogError("[EnemySpawn] ���� ���� ����"); return; }

//        var esm = go.GetComponent<EnemyStatsManager>();
//        if (esm == null) { Debug.LogError("[EnemySpawn] EnemyStatsManager ����"); return; }

//        if (bossWatcher == null) bossWatcher = FindAnyObjectByType<BossProximityWatcher>();
//        if (bossWatcher != null) bossWatcher.SetBoss(esm);
//    }


//    // ��ȯ�� GameObject ����
//    GameObject SpawnById(string enemyId, Vector3 position, bool markAsBoss = false)
//    {
//        if (!prefabMap.TryGetValue(enemyId, out var prefab))
//        {
//            Debug.LogWarning($"[EnemySpawn] '{enemyId}' ������ ������ �����ϴ�.");
//            return null;
//        }

//        var go = Instantiate(prefab, position, Quaternion.identity, transform);

//        var esm = go.GetComponent<EnemyStatsManager>();
//        if (esm != null) esm.enemyId = enemyId;

//        var move = go.GetComponent<EnemyMove>();
//        if (move != null) move.SetSpawnPosition(position);

//        // ������ ���� �±� ����
//        if (markAsBoss) go.tag = "Boss";

//        return go;
//    }


//    bool TryPickPointInRoom(RectInt room, out Vector3 pos)
//    {
//        for (int t = 0; t < triesPerEnemy; t++)
//        {
//            int x = Random.Range(room.xMin + 1, room.xMax - 1);
//            int z = Random.Range(room.yMin + 1, room.yMax - 1);

//            if (!mapGenerator.IsFloor(x, z)) continue;

//            var pr = mapGenerator.GetPlayerRoom();
//            if (pr.Contains(new Vector2Int(x, z))) continue;

//            Vector3 candidate = new Vector3(x, spawnY, z);

//            if (obstacleMask.value != 0 &&
//                Physics.CheckSphere(candidate, 0.4f, obstacleMask))
//                continue;

//            pos = candidate; return true;
//        }
//        pos = default; return false;
//    }
//}
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;              // �� min/maxStage �ݿ��� ���� ���÷���
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private BossProximityWatcher bossWatcher;

    [Header("Refs")]
    public TileMapGenerator mapGenerator;
    public StageManager stageManager;

    [Header("���� �� ����")]
    public float spawnFactor = 25f; // �� ���� / spawnFactor = �� ��(�ּ� 1)
    public int bossCount = 1;
    public int triesPerEnemy = 10;

    [Header("���� ����/�浹")]
    public float spawnY = 1f;
    public LayerMask obstacleMask;

    [Header("������ ����")]
    public List<EnemyPrefabPair> prefabPairs = new(); // �ν����Ϳ��� id �� prefab ����
    private Dictionary<string, GameObject> prefabMap;

    // DB ĳ��
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
            Debug.LogError("TileMapGenerator�� �������ּ���!");
            return;
        }
        mapGenerator.OnMapGenerated += GenerateEnemies;
    }

    void OnDisable()
    {
        if (mapGenerator != null)
            mapGenerator.OnMapGenerated -= GenerateEnemies;
    }

    // DB �ε� (�� ��)
    private void EnsureDbLoaded()
    {
        if (db != null) return;
        TextAsset json = Resources.Load<TextAsset>("Datas/enemyData");
        if (json == null)
        {
            Debug.LogError("Resources/Datas/enemyData.json�� �ʿ��մϴ�!");
            db = new EnemyDatabase { enemies = new EnemyData[0] };
            return;
        }
        db = JsonUtility.FromJson<EnemyDatabase>(json.text);
        if (db.enemies == null) db.enemies = new EnemyData[0];
    }

    public void GenerateEnemies()
    {
        // ���� ���� ����
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        EnsureDbLoaded();

        if (stageManager == null)
        {
            Debug.LogWarning("[EnemySpawn] StageManager ���� �� �������� ��� �پ缺 ������");
            return;
        }

        if (stageManager.IsBossStage()) SpawnBossStage();
        else SpawnNormalStage();
    }

    // ====== �ڡ� �ٽ�: �������� ���� ��ƿ (JSON�� ������ �⺻�� ����) �ڡ� ======
    private static readonly FieldInfo _fiMinStage = typeof(EnemyData).GetField("minStage");
    private static readonly FieldInfo _fiMaxStage = typeof(EnemyData).GetField("maxStage");

    private int GetMinStage(EnemyData e)
    {
        // JSON�� minStage �ʵ尡 ������ ���, ������ unlockStage ������� ����
        if (_fiMinStage != null)
        {
            int v = (int)_fiMinStage.GetValue(e);
            if (v > 0) return v;
        }
        return Math.Max(1, e.unlockStage);
    }

    private int GetMaxStage(EnemyData e)
    {
        if (_fiMaxStage != null)
        {
            int v = (int)_fiMaxStage.GetValue(e);
            if (v > 0) return v;
        }
        return int.MaxValue;
    }

    private bool IsAvailableOnStage(EnemyData e, int stage, bool isBossStage)
    {
        if (e.isBoss != isBossStage) return false;
        // unlockStage�� ������ �������� ����
        if (stage < Math.Max(1, e.unlockStage)) return false;
        int minS = GetMinStage(e);
        int maxS = GetMaxStage(e);
        return stage >= minS && stage <= maxS;
    }
    // ====================================================================

    void SpawnNormalStage()
    {
        int stage = stageManager.currentStage;

        // �� unlockStage + (min/maxStage ����) + ������ ���� üũ
        var pool = db.enemies
            .Where(e => IsAvailableOnStage(e, stage, isBossStage: false) && prefabMap.ContainsKey(e.id))
            .ToList();

        if (pool.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawn] Stage {stage}���� ���� ������ �Ϲ� ���� �����ϴ�.");
            return;
        }

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

                // ����ġ ���� ��
                float pick = UnityEngine.Random.value * totalWeight;
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

    void SpawnBossStage()
    {
        int stage = stageManager.currentStage;

        // �� ������ ���� üũ
        var bosses = db.enemies
            .Where(e => IsAvailableOnStage(e, stage, isBossStage: true) && prefabMap.ContainsKey(e.id))
            .ToList();

        if (bosses.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawn] Stage {stage} ���� Ǯ ���� �� �Ϲ� �������� ��ü");
            SpawnNormalStage();
            return;
        }

        var br = mapGenerator.GetBossRoom();
        if (br.width <= 0 || br.height <= 0)
        {
            Debug.LogWarning("[EnemySpawn] ������ ������ �� �Ϲ� �������� ��ü");
            SpawnNormalStage();
            return;
        }

        Vector2Int c = new Vector2Int(Mathf.RoundToInt(br.center.x), Mathf.RoundToInt(br.center.y));
        Vector3 pos;
        if (mapGenerator.IsFloor(c.x, c.y)) pos = new Vector3(c.x, spawnY, c.y);
        else if (!TryPickPointInRoom(br, out pos)) pos = new Vector3(br.xMin + br.width / 2f, spawnY, br.yMin + br.height / 2f);

        var boss = bosses[UnityEngine.Random.Range(0, bosses.Count)];

        // ���� �±� �ο�
        GameObject go = SpawnById(boss.id, pos, markAsBoss: true);
        if (go == null) { Debug.LogError("[EnemySpawn] ���� ���� ����"); return; }

        var esm = go.GetComponent<EnemyStatsManager>();
        if (esm == null) { Debug.LogError("[EnemySpawn] EnemyStatsManager ����"); return; }

        if (bossWatcher == null) bossWatcher = FindAnyObjectByType<BossProximityWatcher>();
        if (bossWatcher != null) bossWatcher.SetBoss(esm);
    }

    // ��ȯ�� GameObject ����
    GameObject SpawnById(string enemyId, Vector3 position, bool markAsBoss = false)
    {
        if (!prefabMap.TryGetValue(enemyId, out var prefab))
        {
            Debug.LogWarning($"[EnemySpawn] '{enemyId}' ������ ������ �����ϴ�.");
            return null;
        }

        var go = Instantiate(prefab, position, Quaternion.identity, transform);

        var esm = go.GetComponent<EnemyStatsManager>();
        if (esm != null) esm.enemyId = enemyId;

        var move = go.GetComponent<EnemyMove>();
        if (move != null) move.SetSpawnPosition(position);

        if (markAsBoss) go.tag = "Boss";

        return go;
    }

    bool TryPickPointInRoom(RectInt room, out Vector3 pos)
    {
        for (int t = 0; t < triesPerEnemy; t++)
        {
            int x = UnityEngine.Random.Range(room.xMin + 1, room.xMax - 1);
            int z = UnityEngine.Random.Range(room.yMin + 1, room.yMax - 1);

            if (!mapGenerator.IsFloor(x, z)) continue;

            var pr = mapGenerator.GetPlayerRoom();
            if (pr.Contains(new Vector2Int(x, z))) continue;

            Vector3 candidate = new Vector3(x, spawnY, z);

            if (obstacleMask.value != 0 &&
                Physics.CheckSphere(candidate, 0.4f, obstacleMask))
                continue;

            pos = candidate;
            return true;
        }
        pos = default;
        return false;
    }
}
