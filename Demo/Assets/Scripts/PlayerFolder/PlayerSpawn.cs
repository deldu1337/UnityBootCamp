using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;               // 스폰할 플레이어 프리팹
    public TileMapGenerator mapGenerator;   // 맵 생성기 참조

    private GameObject currentPlayer;        // 실제 씬에 존재하는 플레이어

    void Start()
    {
        // mapGenerator가 연결되지 않았으면 오류 출력 후 종료
        if (mapGenerator == null)
        {
            Debug.LogError("TileMapGenerator를 연결해주세요!");
            return;
        }

        // 맵 생성 완료 이벤트에 RespawnPlayer 메서드 등록
        mapGenerator.OnMapGenerated += ReloadPlayer;

        // 게임 시작 시 플레이어 스폰
        RespawnPlayer();
    }

    public void ReloadPlayer()
    {
        // 맵 생성 완료 후 새 위치로 이동
        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 newPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);
        currentPlayer.transform.position = newPos;
    }

    public void RespawnPlayer()
    {
        // 기존 플레이어 제거
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // 플레이어 시작 위치 계산
        RectInt playerRoom = mapGenerator.GetPlayerRoom();
        Vector3 spawnPos = new Vector3(playerRoom.center.x, 0.5f, playerRoom.center.y);

        // 새로운 플레이어 인스턴스 생성
        currentPlayer = Instantiate(player, spawnPos, Quaternion.identity, transform);
    }
}