using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;      // ������ �� ������
    public TileMapGenerator mapGenerator; // �� ������ �������� ���� TileMapGenerator
    public float spawnFactor = 25f;     // �� ũ�� ��� �� �� ���� ���

    void Start()
    {
        // mapGenerator ���� Ȯ��
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator�� �������ּ���!");
            return;
        }

        // �� ���� �Ϸ� �̺�Ʈ�� �� ���� �Լ� ����
        mapGenerator.OnMapGenerated += GenerateEnemies;

        // �ʱ� �� ����
        GenerateEnemies();
    }

    public void GenerateEnemies()
    {
        // ������ ������ �� ����
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // �� �渶�� �� ����
        foreach (var room in mapGenerator.GetRooms())
        {
            int roomArea = room.width * room.height; // �� ���� ���
            int enemyCount = Mathf.Max(1, Mathf.RoundToInt(roomArea / spawnFactor)); // �� ���� ��� �ּ� 1�� �̻�

            int tries = 0;   // �ִ� �õ� Ƚ�� ����
            int spawned = 0; // ���� ������ �� ��

            // �� ���� �����ϰ� �õ� ���� ������ �ݺ�
            while (spawned < enemyCount && tries < enemyCount * 10)
            {
                tries++;

                // ���� ��ġ ���� (�� ����)
                float xF = Random.Range(room.xMin + 1, room.xMax - 1);
                float zF = Random.Range(room.yMin + 1, room.yMax - 1);

                int x = Mathf.RoundToInt(xF);
                int z = Mathf.RoundToInt(zF);

                Vector2Int pos = new Vector2Int(x, z);

                // �ٴ����� Ȯ���ϰ�, �÷��̾ �ִ� ���� ����
                if (mapGenerator.IsFloor(x, z) && !mapGenerator.GetPlayerRoom().Contains(pos))
                {
                    Vector3 spawnPos = new Vector3(x, 1f, z); // y=1 ��ġ�� ����
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);

                    // ������ ���� ���� ��ġ ����
                    EnemyMove move = enemy.GetComponent<EnemyMove>();
                    if (move != null)
                        move.SetSpawnPosition(spawnPos);

                    spawned++;
                }
            }
        }
    }

    // �� ����� (��: �÷��̾� ������ ��)
    public void RespawnEnemies()
    {
        GenerateEnemies();
    }
}