using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class StageTheme
    {
        public string name;
        public Material wallMaterial;
        public Material floorMaterial;
    }

    [SerializeField] private StageManager stageManager;

    public int width = 100; // 맵 가로 크기
    public int height = 100; // 맵 세로 크기
    public GameObject wallPrefab;  // 벽 프리팹
    public GameObject floorPrefab; // 바닥 프리팹
    public int minRoomSize = 10;   // 방 최소 크기
    public int maxRoomSize = 24;   // 방 최대 크기
    public int maxDepth = 20;      // BSP 분할 최대 깊이
    public int corridorWidth = 5;  // 통로 폭
    public GameObject portalPrefab; // 포탈 프리팹
    public float portalYOffset = 0f; // 필요 시 Y오프셋(바닥이 0이 아니면 조정)
    public int bossRoomWidth = 28;
    public int bossRoomHeight = 28;

    private RectInt bossRoom;  // 보스 전용 방
    private int[,] map; // 2D 맵 데이터: 0=바닥, 1=벽
    private List<RectInt> rooms; // 일반 방 리스트
    private RectInt playerRoom;  // 플레이어 전용 방

    [Header("Stage Themes")]
    public int stagesPerTheme = 5;      // 예: 1~5, 6~10 ...
    public StageTheme[] themes;         // 인스펙터에서 a/b/c... 테마 등록
    public bool useBossOverrideTheme = true;
    public StageTheme bossOverrideTheme; // 보스 스테이지 테마(선택)

    // 현재 테마 반환
    private StageTheme GetActiveTheme()
    {
        // 보스 스테이지면 우선 보스 테마 사용(있으면)
        if (useBossOverrideTheme && stageManager != null && stageManager.IsBossStage() && bossOverrideTheme != null)
            return bossOverrideTheme;

        if (themes == null || themes.Length == 0) return null;

        int stage = (stageManager != null) ? stageManager.currentStage : 1;
        int idx = Mathf.FloorToInt((stage - 1) / Mathf.Max(1, stagesPerTheme));
        idx = Mathf.Clamp(idx, 0, themes.Length - 1);
        return themes[idx];
    }

    // 맵 생성 완료 이벤트
    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    private bool HasBossRoom => bossRoom.width > 0 && bossRoom.height > 0;

    void Start()
    {
        GenerateMap(); // 맵 데이터 생성
        RenderMap();   // 맵 오브젝트 생성
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();
        bossRoom = new RectInt(0, 0, 0, 0);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // 1) playerRoom 먼저
        playerRoom = new RectInt(2, 2, 10, 10);

        // 2) BSP로 일반 방 생성
        RectInt root = new RectInt(1, 1, width - 2, height - 2);
        SplitRoom(root, maxDepth, rooms);

        // 3) 플레이어 방 carve
        for (int x = playerRoom.xMin; x < playerRoom.xMax; x++)
            for (int y = playerRoom.yMin; y < playerRoom.yMax; y++)
                map[x, y] = 1;
        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
                map[x, y] = 0;

        // 4) 일반 방 carve (플레이어 방 제외)
        foreach (var room in rooms)
        {
            if (!room.Overlaps(playerRoom))
            {
                for (int x = room.xMin; x < room.xMax; x++)
                    for (int y = room.yMin; y < room.yMax; y++)
                        map[x, y] = 0;
            }
        }

        // 보스 스테이지 여부
        bool makeBossRoom = (stageManager != null && stageManager.IsBossStage());

        // 5) (보스 스테이지일 때만) 보스방 carve (정가운데 크게)
        if (makeBossRoom)
        {
            int bw = Mathf.Clamp(bossRoomWidth, minRoomSize, width - 4);
            int bh = Mathf.Clamp(bossRoomHeight, minRoomSize, height - 4);
            int bx = (width - bw) / 2;
            int by = (height - bh) / 2;
            bossRoom = new RectInt(bx, by, bw, bh);

            for (int x = bossRoom.xMin; x < bossRoom.xMax; x++)
                for (int y = bossRoom.yMin; y < bossRoom.yMax; y++)
                    map[x, y] = 0;
        }

        // 6) 방 중심 좌표(일반 방만)
        List<Vector2Int> centers = rooms
            .Where(r => !r.Overlaps(playerRoom) && !r.Overlaps(bossRoom))
            .Select(r => new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y)))
            .ToList();

        // 7) MST로 일반 방 연결
        List<Vector2Int> connected = new List<Vector2Int>();
        List<Vector2Int> remaining = new List<Vector2Int>(centers);
        if (remaining.Count > 0) { connected.Add(remaining[0]); remaining.RemoveAt(0); }

        while (remaining.Count > 0)
        {
            float minDist = float.MaxValue; Vector2Int a = Vector2Int.zero, b = Vector2Int.zero;
            foreach (var c in connected)
                foreach (var r in remaining)
                {
                    float dist = Vector2Int.Distance(c, r);
                    if (dist < minDist) { minDist = dist; a = c; b = r; }
                }
            CreateCorridor(a, b);
            connected.Add(b);
            remaining.Remove(b);
        }

        // 8) 플레이어 방 ↔ 가장 가까운 일반 방 연결
        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );
        if (centers.Count > 0)
        {
            Vector2Int nearestRoomCenter = centers.OrderBy(c => Vector2Int.Distance(c, playerCenter)).First();
            Vector2Int corridorStart = new Vector2Int((playerRoom.xMin + playerRoom.xMax) / 2, playerRoom.yMax);

            int offset = Mathf.Max(corridorWidth / 2, 1);
            for (int w = -offset; w <= offset; w++)
            {
                int x = corridorStart.x + w, y = corridorStart.y;
                if (x >= playerRoom.xMin && x < playerRoom.xMax && y < height)
                {
                    map[x, y] = 0; map[x, y - 1] = 0;
                }
            }
            CreatePlayerCorridor(corridorStart, nearestRoomCenter);
        }

        // 9) (보스 스테이지일 때만) 보스방 ↔ 일반 방 연결(입구 최소 1개, 최대 4개)
        if (makeBossRoom)
            ConnectBossRoomEntrances(centers);

        // 포탈은 기존 로직 유지(가장 먼 일반 방 중심)
        PlacePortal();

        OnMapGenerated?.Invoke();
    }

    void ConnectBossRoomEntrances(List<Vector2Int> normalCenters)
    {
        if (normalCenters == null || normalCenters.Count == 0) return;

        // 보스방 네 변의 중간 지점(방 내부에서 바로 바깥으로 나가는 에지)
        var edgePoints = new List<Vector2Int>
    {
        new Vector2Int(bossRoom.xMin, (bossRoom.yMin + bossRoom.yMax) / 2),          // Left
        new Vector2Int(bossRoom.xMax - 1, (bossRoom.yMin + bossRoom.yMax) / 2),     // Right
        new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMin),         // Bottom
        new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMax - 1)      // Top
    };

        // 입구 최소 1개 보장, 가능하면 여러 개 연결
        int made = 0;
        foreach (var ep in edgePoints)
        {
            // 가장 가까운 일반 방 중심 찾기
            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, ep)).FirstOrDefault();

            // 에지에서 바깥으로 한 칸 나가며 문 틈(폭 corridorWidth) 확보
            int offset = Mathf.Max(corridorWidth / 2, 1);
            // 방향 판정(보스방 밖으로 나가는 방향)
            Vector2Int dir = Vector2Int.zero;
            if (ep.x == bossRoom.xMin) dir = Vector2Int.left;
            else if (ep.x == bossRoom.xMax - 1) dir = Vector2Int.right;
            else if (ep.y == bossRoom.yMin) dir = Vector2Int.down;
            else if (ep.y == bossRoom.yMax - 1) dir = Vector2Int.up;

            // 문 carving
            for (int w = -offset; w <= offset; w++)
            {
                int x = ep.x + (dir.x == 0 ? w : 0);
                int y = ep.y + (dir.y == 0 ? w : 0);
                int dx = ep.x + dir.x;
                int dy = ep.y + dir.y;

                if (x >= 0 && x < width && y >= 0 && y < height) map[x, y] = 0;
                if (dx >= 0 && dx < width && dy >= 0 && dy < height) map[dx, dy] = 0;
            }

            // 입구에서 일반 방 중심까지 복도 파기
            CreatePlayerCorridor(ep, nearest);
            made++;
        }

        // 혹시라도 네 변 모두 실패했다면(이론상 거의 없음) 중앙에서 한 번 더 시도
        if (made == 0)
        {
            var bossC = new Vector2Int(Mathf.RoundToInt(bossRoom.center.x), Mathf.RoundToInt(bossRoom.center.y));
            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, bossC)).First();
            CreatePlayerCorridor(bossC, nearest);
        }
    }


    void PlacePortal()
    {
        // 프리팹 없거나 방이 없으면 종료
        if (portalPrefab == null || rooms == null || rooms.Count == 0) return;

        // 플레이어 방 기준 중심
        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );

        // 플레이어 방과 겹치지 않는 방 중 가장 먼 방 고르기
        RectInt farthestRoom = rooms
            .Where(r => !r.Overlaps(playerRoom))
            .OrderByDescending(r =>
            {
                var c = new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
                return Vector2Int.Distance(playerCenter, c);
            })
            .FirstOrDefault();

        if (farthestRoom.width == 0 || farthestRoom.height == 0) return;

        // 포탈 위치 (방 중심)
        Vector3 portalPos = new Vector3(farthestRoom.center.x, portalYOffset, farthestRoom.center.y);

        // 생성 및 부모 설정(리로드 시 함께 정리되도록)
        GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity, transform);

        // 콜라이더 보장 + isTrigger 설정
        Collider col = portal.GetComponent<Collider>();
        if (col == null) col = portal.AddComponent<BoxCollider>();
        col.isTrigger = true;

        // 트리거 스크립트 보장 + 타겟 제너레이터 전달
        PortalTrigger trigger = portal.GetComponent<PortalTrigger>();
        if (trigger == null) trigger = portal.AddComponent<PortalTrigger>();
        trigger.Setup(this); // 이 제너레이터의 ReloadMap을 사용
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
    void RenderMap()
    {
        float floorSize = 10f;
        var theme = GetActiveTheme(); // 현재 테마

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 바닥(10×10) 타일 배치
                if (x % (int)floorSize == 0 && y % (int)floorSize == 0)
                {
                    Vector3 floorPos = new Vector3(x + 5, 0, y + 5);
                    var floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

                    // 테마 바닥 머터리얼 적용
                    if (theme != null && theme.floorMaterial != null)
                    {
                        var rend = floor.GetComponentInChildren<Renderer>();
                        if (rend != null) rend.sharedMaterial = theme.floorMaterial;
                    }
                }

                // 벽(1×1) 배치
                if (map[x, y] == 1)
                {
                    float wallHeight = wallPrefab.transform.localScale.y;
                    Vector3 wallPos = new Vector3(x, wallHeight / 5f, y);
                    var wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);

                    // 테마 벽 머터리얼 적용
                    if (theme != null && theme.wallMaterial != null)
                    {
                        var rend = wall.GetComponentInChildren<Renderer>();
                        if (rend != null) rend.sharedMaterial = theme.wallMaterial;
                    }
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
        // 보스방이 있을 때만 제외
        if (HasBossRoom)
            return rooms.Where(r => r != playerRoom && !r.Overlaps(bossRoom)).ToList();
        else
            return rooms.Where(r => r != playerRoom).ToList();
    }

    public RectInt GetBossRoom() => bossRoom;


    // 플레이어 방 반환
    public RectInt GetPlayerRoom() => playerRoom;

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