using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMapGenerator : MonoBehaviour
{
    public int width = 100; // 맵 가로 크기
    public int height = 100; // 맵 세로 크기
    public GameObject wallPrefab;  // 벽 프리팹
    public GameObject floorPrefab; // 바닥 프리팹
    public int minRoomSize = 10;   // 방 최소 크기
    public int maxRoomSize = 24;   // 방 최대 크기
    public int maxDepth = 20;      // BSP 분할 최대 깊이
    public int corridorWidth = 5;  // 통로 폭

    private int[,] map; // 2D 맵 데이터: 0=바닥, 1=벽
    private List<RectInt> rooms; // 일반 방 리스트
    private RectInt playerRoom;  // 플레이어 전용 방

    // 맵 생성 완료 이벤트
    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    void Start()
    {
        GenerateMap(); // 맵 데이터 생성
        RenderMap();   // 맵 오브젝트 생성
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();

        // 전체 벽으로 초기화
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // BSP로 방 분할
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

        // 일반 방 바닥 생성 (플레이어 방 제외)
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

        // MST 연결 (Prim 알고리즘) - 방들을 최소 연결 통로로 연결
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

            // 연결된 방과 남은 방 사이의 최소 거리 방 선택
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

            CreateCorridor(a, b); // 통로 생성
            connected.Add(b);
            remaining.Remove(b);
        }

        // 플레이어 방과 가장 가까운 방 연결 (항상 위쪽 방향 우선)
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

            // 플레이어 방 벽 뚫기 (최소 corridorWidth 적용)
            int offset = Mathf.Max(corridorWidth / 2, 1);
            for (int w = -offset; w <= offset; w++)
            {
                int x = corridorStart.x + w;
                int y = corridorStart.y;
                if (x >= playerRoom.xMin && x < playerRoom.xMax && y < height)
                {
                    map[x, y] = 0;
                    map[x, y - 1] = 0; // 입구 폭 확보
                }
            }

            // 플레이어 방 → 가장 가까운 방 통로 생성
            CreatePlayerCorridor(corridorStart, nearestRoomCenter);
        }

        // 맵 생성 완료 이벤트 호출
        OnMapGenerated?.Invoke();
    }

    // BSP 분할 함수
    void SplitRoom(RectInt space, int depth, List<RectInt> rooms)
    {
        // 최대 깊이 도달 또는 공간이 너무 작으면 방 생성
        if (depth == 0 || space.width < minRoomSize * 2 || space.height < minRoomSize * 2)
        {
            int roomWidth = Random.Range(minRoomSize, Mathf.Min(space.width, maxRoomSize));
            int roomHeight = Random.Range(minRoomSize, Mathf.Min(space.height, maxRoomSize));
            int roomX = Random.Range(space.xMin + 1, space.xMax - roomWidth - 1);
            int roomY = Random.Range(space.yMin + 1, space.yMax - roomHeight - 1);
            RectInt newRoom = new RectInt(roomX, roomY, roomWidth, roomHeight);

            float minDistanceFromPlayer = 8f; // 플레이어 방 최소 거리
            Vector2Int newCenter = new Vector2Int(Mathf.RoundToInt(newRoom.center.x), Mathf.RoundToInt(newRoom.center.y));
            Vector2Int playerCenter = new Vector2Int(Mathf.RoundToInt(playerRoom.center.x), Mathf.RoundToInt(playerRoom.center.y));

            if (!newRoom.Overlaps(playerRoom) &&
                Vector2Int.Distance(newCenter, playerCenter) > minDistanceFromPlayer)
            {
                rooms.Add(newRoom);
            }
            return;
        }

        // 공간 분할 (수평 또는 수직 랜덤)
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

    // 두 방 사이 통로 생성
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

    // 플레이어 방 → 방 통로 생성
    void CreatePlayerCorridor(Vector2Int start, Vector2Int end)
    {
        int offset = Mathf.Max(corridorWidth / 2, 1);

        // 중간점 생성 (조금 랜덤)
        Vector2Int mid = new Vector2Int(
            (start.x + end.x) / 2 + Random.Range(-2, 3),
            (start.y + end.y) / 2 + Random.Range(-2, 3)
        );

        // 시작 → 중간 통로
        for (int w = -offset; w <= offset; w++)
            DigCorridor(new Vector2Int(start.x + w, start.y), new Vector2Int(mid.x + w, mid.y));

        // 중간 → 끝 통로
        for (int w = -offset; w <= offset; w++)
            DigCorridor(new Vector2Int(mid.x + w, mid.y), new Vector2Int(end.x + w, end.y));
    }

    // 직선 통로 파기
    void DigCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        // x축 이동
        int xDir = (end.x > current.x) ? 1 : -1;
        while (current.x != end.x)
        {
            current.x += xDir;
            for (int w = -1; w <= 1; w++) // 폭 3칸
            {
                int yy = current.y + w;
                if (!playerRoom.Contains(new Vector2Int(current.x, yy)))
                    map[current.x, yy] = 0;
            }
        }

        // y축 이동
        int yDir = (end.y > current.y) ? 1 : -1;
        while (current.y != end.y)
        {
            current.y += yDir;
            for (int w = -1; w <= 1; w++) // 폭 3칸
            {
                int xx = current.x + w;
                if (!playerRoom.Contains(new Vector2Int(xx, current.y)))
                    map[xx, current.y] = 0;
            }
        }
    }

    // 수평 통로 생성
    void DrawHorizontalCorridor(int xStart, int xEnd, int y)
    {
        int offset = corridorWidth / 2;
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            for (int w = -offset; w <= offset; w++)
            {
                int yy = y + w;
                if (yy >= 0 && yy < height && !playerRoom.Contains(new Vector2Int(x, yy)))
                    map[x, yy] = 0;
            }
    }

    // 수직 통로 생성
    void DrawVerticalCorridor(int yStart, int yEnd, int x)
    {
        int offset = corridorWidth / 2;
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            for (int w = -offset; w <= offset; w++)
            {
                int xx = x + w;
                if (xx >= 0 && xx < width && !playerRoom.Contains(new Vector2Int(xx, y)))
                    map[xx, y] = 0;
            }
    }

    // 맵 렌더링: 벽/바닥 프리팹 생성
    //void RenderMap()
    //{
    //    for (int x = 0; x < width; x++)
    //        for (int y = 0; y < height; y++)
    //        {
    //            GameObject prefab = (map[x, y] == 1) ? wallPrefab : floorPrefab;
    //            Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity, transform);
    //        }
    //}
    void RenderMap()
    {
        float floorSize = 10f; // 바닥 프리팹 크기 (10×10)

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // === 바닥 생성 (10칸마다 한 번만) ===
                if (x % (int)floorSize == 0 && y % (int)floorSize == 0)
                {
                    Vector3 floorPos = new Vector3(x + 5, 0, y + 5);
                    Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);
                }

                // === 벽 생성 (1×1 단위) ===
                if (map[x, y] == 1)
                {
                    float wallHeight = wallPrefab.transform.localScale.y;
                    Vector3 wallPos = new Vector3(x, wallHeight / 5f, y);
                    Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                }
            }
        }
    }





    // 지정 좌표가 바닥인지 확인
    public bool IsFloor(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return map[x, y] == 0;
    }

    // 일반 방 리스트 반환
    public List<RectInt> GetRooms()
    {
        return rooms.Where(r => r != playerRoom).ToList();
    }

    // 플레이어 방 반환
    public RectInt GetPlayerRoom()
    {
        return playerRoom;
    }

    // 맵 재생성
    public void ReloadMap()
    {
        // 기존 맵 오브젝트 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        GenerateMap();
        RenderMap();
    }
}