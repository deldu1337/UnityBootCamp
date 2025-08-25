using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public TileMapGenerator mapGenerator;
    public float spawnFactor = 25f;

    void Start()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator를 연결해주세요!");
            return;
        }

        mapGenerator.OnMapGenerated += GenerateEnemies;
        GenerateEnemies();
    }

    public void GenerateEnemies()
    {
        // 기존 적 제거
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 방별 적 생성
        foreach (var room in mapGenerator.GetRooms())
        {
            int roomArea = room.width * room.height;
            int enemyCount = Mathf.Max(1, Mathf.RoundToInt(roomArea / spawnFactor));

            int tries = 0;
            int spawned = 0;

            while (spawned < enemyCount && tries < enemyCount * 10)
            {
                tries++;
                float xF = Random.Range(room.xMin + 1, room.xMax - 1);
                float zF = Random.Range(room.yMin + 1, room.yMax - 1);

                int x = Mathf.RoundToInt(xF);
                int z = Mathf.RoundToInt(zF);

                Vector2Int pos = new Vector2Int(x, z);

                // 바닥 확인 + 플레이어 방 내부 제외
                if (mapGenerator.IsFloor(x, z) && !mapGenerator.GetPlayerRoom().Contains(pos))
                {
                    Vector3 spawnPos = new Vector3(x, 1f, z);
                    Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
                    spawned++;
                }
            }
        }
    }

    public void RespawnEnemies()
    {
        GenerateEnemies();
    }
}
