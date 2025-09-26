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

    public int width = 100; // �� ���� ũ��
    public int height = 100; // �� ���� ũ��
    public GameObject wallPrefab;  // �� ������
    public GameObject floorPrefab; // �ٴ� ������
    public int minRoomSize = 10;   // �� �ּ� ũ��
    public int maxRoomSize = 24;   // �� �ִ� ũ��
    public int maxDepth = 20;      // BSP ���� �ִ� ����
    public int corridorWidth = 5;  // ��� ��
    public GameObject portalPrefab; // ��Ż ������
    public float portalYOffset = 0f; // �ʿ� �� Y������(�ٴ��� 0�� �ƴϸ� ����)
    public int bossRoomWidth = 28;
    public int bossRoomHeight = 28;

    private RectInt bossRoom;  // ���� ���� ��
    private int[,] map; // 2D �� ������: 0=�ٴ�, 1=��
    private List<RectInt> rooms; // �Ϲ� �� ����Ʈ
    private RectInt playerRoom;  // �÷��̾� ���� ��

    [Header("Stage Themes")]
    public int stagesPerTheme = 5;      // ��: 1~5, 6~10 ...
    public StageTheme[] themes;         // �ν����Ϳ��� a/b/c... �׸� ���
    public bool useBossOverrideTheme = true;
    public StageTheme bossOverrideTheme; // ���� �������� �׸�(����)

    // ���� �׸� ��ȯ
    private StageTheme GetActiveTheme()
    {
        // ���� ���������� �켱 ���� �׸� ���(������)
        if (useBossOverrideTheme && stageManager != null && stageManager.IsBossStage() && bossOverrideTheme != null)
            return bossOverrideTheme;

        if (themes == null || themes.Length == 0) return null;

        int stage = (stageManager != null) ? stageManager.currentStage : 1;
        int idx = Mathf.FloorToInt((stage - 1) / Mathf.Max(1, stagesPerTheme));
        idx = Mathf.Clamp(idx, 0, themes.Length - 1);
        return themes[idx];
    }

    // �� ���� �Ϸ� �̺�Ʈ
    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    private bool HasBossRoom => bossRoom.width > 0 && bossRoom.height > 0;

    void Start()
    {
        GenerateMap(); // �� ������ ����
        RenderMap();   // �� ������Ʈ ����
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();
        bossRoom = new RectInt(0, 0, 0, 0);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // 1) playerRoom ����
        playerRoom = new RectInt(2, 2, 10, 10);

        // 2) BSP�� �Ϲ� �� ����
        RectInt root = new RectInt(1, 1, width - 2, height - 2);
        SplitRoom(root, maxDepth, rooms);

        // 3) �÷��̾� �� carve
        for (int x = playerRoom.xMin; x < playerRoom.xMax; x++)
            for (int y = playerRoom.yMin; y < playerRoom.yMax; y++)
                map[x, y] = 1;
        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
                map[x, y] = 0;

        // 4) �Ϲ� �� carve (�÷��̾� �� ����)
        foreach (var room in rooms)
        {
            if (!room.Overlaps(playerRoom))
            {
                for (int x = room.xMin; x < room.xMax; x++)
                    for (int y = room.yMin; y < room.yMax; y++)
                        map[x, y] = 0;
            }
        }

        // ���� �������� ����
        bool makeBossRoom = (stageManager != null && stageManager.IsBossStage());

        // 5) (���� ���������� ����) ������ carve (����� ũ��)
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

        // 6) �� �߽� ��ǥ(�Ϲ� �游)
        List<Vector2Int> centers = rooms
            .Where(r => !r.Overlaps(playerRoom) && !r.Overlaps(bossRoom))
            .Select(r => new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y)))
            .ToList();

        // 7) MST�� �Ϲ� �� ����
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

        // 8) �÷��̾� �� �� ���� ����� �Ϲ� �� ����
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

        // 9) (���� ���������� ����) ������ �� �Ϲ� �� ����(�Ա� �ּ� 1��, �ִ� 4��)
        if (makeBossRoom)
            ConnectBossRoomEntrances(centers);

        // ��Ż�� ���� ���� ����(���� �� �Ϲ� �� �߽�)
        PlacePortal();

        OnMapGenerated?.Invoke();
    }

    void ConnectBossRoomEntrances(List<Vector2Int> normalCenters)
    {
        if (normalCenters == null || normalCenters.Count == 0) return;

        // ������ �� ���� �߰� ����(�� ���ο��� �ٷ� �ٱ����� ������ ����)
        var edgePoints = new List<Vector2Int>
    {
        new Vector2Int(bossRoom.xMin, (bossRoom.yMin + bossRoom.yMax) / 2),          // Left
        new Vector2Int(bossRoom.xMax - 1, (bossRoom.yMin + bossRoom.yMax) / 2),     // Right
        new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMin),         // Bottom
        new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMax - 1)      // Top
    };

        // �Ա� �ּ� 1�� ����, �����ϸ� ���� �� ����
        int made = 0;
        foreach (var ep in edgePoints)
        {
            // ���� ����� �Ϲ� �� �߽� ã��
            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, ep)).FirstOrDefault();

            // �������� �ٱ����� �� ĭ ������ �� ƴ(�� corridorWidth) Ȯ��
            int offset = Mathf.Max(corridorWidth / 2, 1);
            // ���� ����(������ ������ ������ ����)
            Vector2Int dir = Vector2Int.zero;
            if (ep.x == bossRoom.xMin) dir = Vector2Int.left;
            else if (ep.x == bossRoom.xMax - 1) dir = Vector2Int.right;
            else if (ep.y == bossRoom.yMin) dir = Vector2Int.down;
            else if (ep.y == bossRoom.yMax - 1) dir = Vector2Int.up;

            // �� carving
            for (int w = -offset; w <= offset; w++)
            {
                int x = ep.x + (dir.x == 0 ? w : 0);
                int y = ep.y + (dir.y == 0 ? w : 0);
                int dx = ep.x + dir.x;
                int dy = ep.y + dir.y;

                if (x >= 0 && x < width && y >= 0 && y < height) map[x, y] = 0;
                if (dx >= 0 && dx < width && dy >= 0 && dy < height) map[dx, dy] = 0;
            }

            // �Ա����� �Ϲ� �� �߽ɱ��� ���� �ı�
            CreatePlayerCorridor(ep, nearest);
            made++;
        }

        // Ȥ�ö� �� �� ��� �����ߴٸ�(�̷л� ���� ����) �߾ӿ��� �� �� �� �õ�
        if (made == 0)
        {
            var bossC = new Vector2Int(Mathf.RoundToInt(bossRoom.center.x), Mathf.RoundToInt(bossRoom.center.y));
            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, bossC)).First();
            CreatePlayerCorridor(bossC, nearest);
        }
    }


    void PlacePortal()
    {
        // ������ ���ų� ���� ������ ����
        if (portalPrefab == null || rooms == null || rooms.Count == 0) return;

        // �÷��̾� �� ���� �߽�
        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );

        // �÷��̾� ��� ��ġ�� �ʴ� �� �� ���� �� �� ����
        RectInt farthestRoom = rooms
            .Where(r => !r.Overlaps(playerRoom))
            .OrderByDescending(r =>
            {
                var c = new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
                return Vector2Int.Distance(playerCenter, c);
            })
            .FirstOrDefault();

        if (farthestRoom.width == 0 || farthestRoom.height == 0) return;

        // ��Ż ��ġ (�� �߽�)
        Vector3 portalPos = new Vector3(farthestRoom.center.x, portalYOffset, farthestRoom.center.y);

        // ���� �� �θ� ����(���ε� �� �Բ� �����ǵ���)
        GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity, transform);

        // �ݶ��̴� ���� + isTrigger ����
        Collider col = portal.GetComponent<Collider>();
        if (col == null) col = portal.AddComponent<BoxCollider>();
        col.isTrigger = true;

        // Ʈ���� ��ũ��Ʈ ���� + Ÿ�� ���ʷ����� ����
        PortalTrigger trigger = portal.GetComponent<PortalTrigger>();
        if (trigger == null) trigger = portal.AddComponent<PortalTrigger>();
        trigger.Setup(this); // �� ���ʷ������� ReloadMap�� ���
    }


    // BSP ���� �Լ�
    void SplitRoom(RectInt space, int depth, List<RectInt> rooms)
    {
        // �ִ� ���� ���� �Ǵ� ������ �ʹ� ������ �� ����
        if (depth == 0 || space.width < minRoomSize * 2 || space.height < minRoomSize * 2)
        {
            int roomWidth = Random.Range(minRoomSize, Mathf.Min(space.width, maxRoomSize));
            int roomHeight = Random.Range(minRoomSize, Mathf.Min(space.height, maxRoomSize));
            int roomX = Random.Range(space.xMin + 1, space.xMax - roomWidth - 1);
            int roomY = Random.Range(space.yMin + 1, space.yMax - roomHeight - 1);
            RectInt newRoom = new RectInt(roomX, roomY, roomWidth, roomHeight);

            float minDistanceFromPlayer = 8f; // �÷��̾� �� �ּ� �Ÿ�
            Vector2Int newCenter = new Vector2Int(Mathf.RoundToInt(newRoom.center.x), Mathf.RoundToInt(newRoom.center.y));
            Vector2Int playerCenter = new Vector2Int(Mathf.RoundToInt(playerRoom.center.x), Mathf.RoundToInt(playerRoom.center.y));

            if (!newRoom.Overlaps(playerRoom) &&
                Vector2Int.Distance(newCenter, playerCenter) > minDistanceFromPlayer)
            {
                rooms.Add(newRoom);
            }
            return;
        }

        // ���� ���� (���� �Ǵ� ���� ����)
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

    // �� �� ���� ��� ����
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

    // �÷��̾� �� �� �� ��� ����
    void CreatePlayerCorridor(Vector2Int start, Vector2Int end)
    {
        int offset = Mathf.Max(corridorWidth / 2, 1);

        // �߰��� ���� (���� ����)
        Vector2Int mid = new Vector2Int(
            (start.x + end.x) / 2 + Random.Range(-2, 3),
            (start.y + end.y) / 2 + Random.Range(-2, 3)
        );

        // ���� �� �߰� ���
        for (int w = -offset; w <= offset; w++)
            DigCorridor(new Vector2Int(start.x + w, start.y), new Vector2Int(mid.x + w, mid.y));

        // �߰� �� �� ���
        for (int w = -offset; w <= offset; w++)
            DigCorridor(new Vector2Int(mid.x + w, mid.y), new Vector2Int(end.x + w, end.y));
    }

    // ���� ��� �ı�
    void DigCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        // x�� �̵�
        int xDir = (end.x > current.x) ? 1 : -1;
        while (current.x != end.x)
        {
            current.x += xDir;
            for (int w = -1; w <= 1; w++) // �� 3ĭ
            {
                int yy = current.y + w;
                if (!playerRoom.Contains(new Vector2Int(current.x, yy)))
                    map[current.x, yy] = 0;
            }
        }

        // y�� �̵�
        int yDir = (end.y > current.y) ? 1 : -1;
        while (current.y != end.y)
        {
            current.y += yDir;
            for (int w = -1; w <= 1; w++) // �� 3ĭ
            {
                int xx = current.x + w;
                if (!playerRoom.Contains(new Vector2Int(xx, current.y)))
                    map[xx, current.y] = 0;
            }
        }
    }

    // ���� ��� ����
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

    // ���� ��� ����
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

    // �� ������: ��/�ٴ� ������ ����
    void RenderMap()
    {
        float floorSize = 10f;
        var theme = GetActiveTheme(); // ���� �׸�

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // �ٴ�(10��10) Ÿ�� ��ġ
                if (x % (int)floorSize == 0 && y % (int)floorSize == 0)
                {
                    Vector3 floorPos = new Vector3(x + 5, 0, y + 5);
                    var floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

                    // �׸� �ٴ� ���͸��� ����
                    if (theme != null && theme.floorMaterial != null)
                    {
                        var rend = floor.GetComponentInChildren<Renderer>();
                        if (rend != null) rend.sharedMaterial = theme.floorMaterial;
                    }
                }

                // ��(1��1) ��ġ
                if (map[x, y] == 1)
                {
                    float wallHeight = wallPrefab.transform.localScale.y;
                    Vector3 wallPos = new Vector3(x, wallHeight / 5f, y);
                    var wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);

                    // �׸� �� ���͸��� ����
                    if (theme != null && theme.wallMaterial != null)
                    {
                        var rend = wall.GetComponentInChildren<Renderer>();
                        if (rend != null) rend.sharedMaterial = theme.wallMaterial;
                    }
                }
            }
        }
    }

    // ���� ��ǥ�� �ٴ����� Ȯ��
    public bool IsFloor(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return map[x, y] == 0;
    }

    // �Ϲ� �� ����Ʈ ��ȯ
    public List<RectInt> GetRooms()
    {
        // �������� ���� ���� ����
        if (HasBossRoom)
            return rooms.Where(r => r != playerRoom && !r.Overlaps(bossRoom)).ToList();
        else
            return rooms.Where(r => r != playerRoom).ToList();
    }

    public RectInt GetBossRoom() => bossRoom;


    // �÷��̾� �� ��ȯ
    public RectInt GetPlayerRoom() => playerRoom;

    // �� �����
    public void ReloadMap()
    {
        // ���� �� ������Ʈ ����
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        GenerateMap();
        RenderMap();
    }
}