//using UnityEngine;

//public class PlayerSpawn : MonoBehaviour
//{
//    public GameObject player;               // ������ �÷��̾� ������
//    public TileMapGenerator mapGenerator;   // �� ������ ����

//    private GameObject currentPlayer;        // ���� ���� �����ϴ� �÷��̾�

//    void Start()
//    {
//        // mapGenerator�� ������� �ʾ����� ���� ��� �� ����
//        if (mapGenerator == null)
//        {
//            Debug.LogError("TileMapGenerator�� �������ּ���!");
//            return;
//        }

//        // �� ���� �Ϸ� �̺�Ʈ�� RespawnPlayer �޼��� ���
//        mapGenerator.OnMapGenerated += ReloadPlayer;

//        // ���� ���� �� �÷��̾� ����
//        RespawnPlayer();
//    }

//    public void ReloadPlayer()
//    {
//        // �� ���� �Ϸ� �� �� ��ġ�� �̵�
//        RectInt playerRoom = mapGenerator.GetPlayerRoom();
//        Vector3 newPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);
//        currentPlayer.transform.position = newPos;
//    }

//    public void RespawnPlayer()
//    {
//        // ���� �÷��̾� ����
//        if (currentPlayer != null)
//            Destroy(currentPlayer);

//        // �÷��̾� ���� ��ġ ���
//        RectInt playerRoom = mapGenerator.GetPlayerRoom();
//        Vector3 spawnPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);

//        // ���ο� �÷��̾� �ν��Ͻ� ����
//        currentPlayer = Instantiate(player, spawnPos, Quaternion.identity, transform);
//    }
//}

// PlayerSpawn.cs (��ü�ؼ� ���)
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public TileMapGenerator mapGenerator;

    private GameObject currentPlayer;

    void Start()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator�� �������ּ���!");
            return;
        }

        mapGenerator.OnMapGenerated += ReloadPlayer;

        if (string.IsNullOrEmpty(GameContext.SelectedRace))
        {
            // ĳ���� ���� ���� �ٷ� ���� ��� �̾��ϱ� �ó������� �� ������ �⺻���� ���
            GameContext.SelectedRace = "humanmale";
        }

        RespawnPlayer();
    }

    private void OnDestroy()
    {
        if (mapGenerator != null) mapGenerator.OnMapGenerated -= ReloadPlayer;
    }

    public void ReloadPlayer()
    {
        if (currentPlayer == null) return; // �� ����
        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 newPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);
        currentPlayer.transform.position = newPos;
    }

    public void RespawnPlayer()
    {
        if (currentPlayer != null)
            Destroy(currentPlayer);

        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 spawnPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);

        string prefabName = GameContext.SelectedRace; // ��: "humanmale"
        GameObject prefab = Resources.Load<GameObject>($"Characters/{prefabName}");
        if (prefab == null)
        {
            Debug.LogError($"������ 'Resources/Characters/{prefabName}.prefab' �� ã�� �� �����ϴ�.");
            return;
        }

        currentPlayer = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

        // �� ���� �ʱ�ȭ
        var stats = currentPlayer.GetComponent<PlayerStatsManager>();
        if (stats != null)
        {
            stats.InitializeForSelectedRace(); // �Ʒ� 3)���� ����
        }
        else
        {
            Debug.LogWarning("PlayerStatsManager ������Ʈ�� ã�� ���߽��ϴ�.");
        }
    }
}
