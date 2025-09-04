using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;               // ������ �÷��̾� ������
    public TileMapGenerator mapGenerator;   // �� ������ ����

    private GameObject currentPlayer;        // ���� ���� �����ϴ� �÷��̾�

    void Start()
    {
        // mapGenerator�� ������� �ʾ����� ���� ��� �� ����
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator�� �������ּ���!");
            return;
        }

        // �� ���� �Ϸ� �̺�Ʈ�� RespawnPlayer �޼��� ���
        mapGenerator.OnMapGenerated += ReloadPlayer;

        // ���� ���� �� �÷��̾� ����
        RespawnPlayer();
    }

    public void ReloadPlayer()
    {
        // �� ���� �Ϸ� �� �� ��ġ�� �̵�
        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 newPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);
        currentPlayer.transform.position = newPos;
    }

    public void RespawnPlayer()
    {
        // ���� �÷��̾� ����
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // �÷��̾� ���� ��ġ ���
        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 spawnPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);

        // ���ο� �÷��̾� �ν��Ͻ� ����
        currentPlayer = Instantiate(player, spawnPos, Quaternion.identity, transform);
    }
}