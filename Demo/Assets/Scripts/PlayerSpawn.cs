using UnityEngine;
using UnityEngine.LowLevel;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;
    public TileMapGenerator mapGenerator;

    void Start()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator를 연결해주세요!");
            return;
        }

        mapGenerator.OnMapGenerated += RespawnPlayer;
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        // 기존 적 제거
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 vector3 = new Vector3(playerRoom.center.x, 0, playerRoom.center.y);

        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );

        Instantiate(player, vector3, Quaternion.identity, transform);
    }
}
