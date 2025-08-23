using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMapGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public int minRoomSize = 10;
    public int maxRoomSize = 24;
    public int maxDepth = 20;
    public int corridorWidth = 5;

    private int[,] map;
    private List<RectInt> rooms;
    private RectInt playerRoom; // 플레이어 전용 방 저장

    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    void Start()
    {
        GenerateMap();
        RenderMap();
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();

        // 전체 벽 초기화
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // BSP 분할
        RectInt root = new RectInt(1, 1, width - 2, height - 2);
        SplitRoom(root, maxDepth, rooms);

        // 플레이어 전용 방 생성 (10x10)
        playerRoom = new RectInt(2, 2, 10, 10);

        // 플레이어 방 벽 초기화
        for (int x = playerRoom.xMin; x < playerRoom.xMax; x++)
            for (int y = playerRoom.yMin; y < playerRoom.yMax; y++)
                map[x, y] = 1;

        // 플레이어 방 내부 바닥 생성 (가장자리 제외)
        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
                map[x, y] = 0;

        // 방 바닥 생성 (플레이어 방 제외)
        foreach (var room in rooms)
        {
            if (!room.Overlaps(playerRoom))
            {
                for (int x = room.xMin; x < room.xMax; x++)
                    for (int y = room.yMin; y < room.yMax; y++)
                        map[x, y] = 0;
            }
        }

        // 방 중심 좌표 계산
        List<Vector2Int> centers = rooms.Select(r =>
            new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y))
        ).ToList();

        // MST 연결 (Prim 알고리즘)
        List<Vector2Int> connected = new List<Vector2Int>();
        List<Vector2Int> remaining = new List<Vector2Int>(centers);
        if (remaining.Count > 0)
        {
            connected.Add(remaining[0]);
            remaining.RemoveAt(0);
        }

        while (remaining.Count > 0)
        {
            float minDist = float.MaxValue;
            Vector2Int a = Vector2Int.zero;
            Vector2Int b = Vector2Int.zero;

            foreach (var c in connected)
            {
                foreach (var r in remaining)
                {
                    float dist = Vector2Int.Distance(c, r);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        a = c;
                        b = r;
                    }
                }
            }

            CreateCorridor(a, b);
            connected.Add(b);
            remaining.Remove(b);
        }

        // 플레이어 방과 가장 가까운 방 연결 (통로 1개, 항상 위쪽)
        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );

        if (centers.Count > 0)
        {
            Vector2Int nearestRoomCenter = centers
                .OrderBy(c => Vector2Int.Distance(c, playerCenter))
                .First();

            // 통로 시작점: 플레이어 방 위쪽 중앙
            Vector2Int corridorStart = new Vector2Int(
                (playerRoom.xMin + playerRoom.xMax) / 2,
                playerRoom.yMax
            );

            // 플레이어 방 벽 뚫기
            map[corridorStart.x, corridorStart.y - 1] = 0; // 방 내부 위쪽 끝
            map[corridorStart.x, corridorStart.y] = 0;     // 벽 바로 위

            // MST 방과 연결
            CreateCorridor(corridorStart, nearestRoomCenter);
        }

        // 맵 생성 완료 이벤트 호출
        OnMapGenerated?.Invoke();
    }

    void SplitRoom(RectInt space, int depth, List<RectInt> rooms)
    {
        if (depth == 0 || space.width < minRoomSize * 2 || space.height < minRoomSize * 2)
        {
            int roomWidth = Random.Range(minRoomSize, Mathf.Min(space.width, maxRoomSize));
            int roomHeight = Random.Range(minRoomSize, Mathf.Min(space.height, maxRoomSize));
            int roomX = Random.Range(space.xMin + 1, space.xMax - roomWidth - 1);
            int roomY = Random.Range(space.yMin + 1, space.yMax - roomHeight - 1);
            RectInt newRoom = new RectInt(roomX, roomY, roomWidth, roomHeight);

            if (!newRoom.Overlaps(playerRoom))
                rooms.Add(newRoom);
            return;
        }

        bool splitHorizontally = Random.value > 0.5f;
        if (splitHorizontally)
        {
            int splitY = Random.Range(space.yMin + minRoomSize, space.yMax - minRoomSize);
            SplitRoom(new RectInt(space.xMin, space.yMin, space.width, splitY - space.yMin), depth - 1, rooms);
            SplitRoom(new RectInt(space.xMin, splitY, space.width, space.yMax - splitY), depth - 1, rooms);
        }
        else
        {
            int splitX = Random.Range(space.xMin + minRoomSize, space.xMax - minRoomSize);
            SplitRoom(new RectInt(space.xMin, space.yMin, splitX - space.xMin, space.height), depth - 1, rooms);
            SplitRoom(new RectInt(splitX, space.yMin, space.xMax - splitX, space.height), depth - 1, rooms);
        }
    }

    void CreateCorridor(Vector2Int a, Vector2Int b)
    {
        if (Random.value > 0.5f)
        {
            DrawHorizontalCorridor(a.x, b.x, a.y);
            DrawVerticalCorridor(a.y, b.y, b.x);
        }
        else
        {
            DrawVerticalCorridor(a.y, b.y, a.x);
            DrawHorizontalCorridor(a.x, b.x, b.y);
        }
    }

    void DrawHorizontalCorridor(int xStart, int xEnd, int y)
    {
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            for (int w = 0; w < corridorWidth; w++)
            {
                int yy = y + w;
                if (yy < height && !playerRoom.Contains(new Vector2Int(x, yy)))
                    map[x, yy] = 0;
            }
    }

    void DrawVerticalCorridor(int yStart, int yEnd, int x)
    {
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            for (int w = 0; w < corridorWidth; w++)
            {
                int xx = x + w;
                if (xx < width && !playerRoom.Contains(new Vector2Int(xx, y)))
                    map[xx, y] = 0;
            }
    }

    void RenderMap()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = (map[x, y] == 1) ? wallPrefab : floorPrefab;
                Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity, transform);
            }
    }

    public bool IsFloor(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return map[x, y] == 0;
    }

    public List<RectInt> GetRooms()
    {
        return rooms.Where(r => r != playerRoom).ToList();
    }

    public RectInt GetPlayerRoom()
    {
        return playerRoom;
    }

    public void ReloadMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        GenerateMap();
        RenderMap();
    }
}
