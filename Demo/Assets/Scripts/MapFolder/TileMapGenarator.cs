//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class TileMapGenerator : MonoBehaviour
//{
//    [System.Serializable]
//    public class StageTheme
//    {
//        // �׸� �̸�. ������ ���п�
//        public string name;
//        // ���� ������ ��Ƽ����
//        public Material wallMaterial;
//        // �ٴڿ� ������ ��Ƽ����
//        public Material floorMaterial;
//    }

//    // ���� �������� ��ȣ�� ���� �������� ���θ� �˷��ִ� �Ŵ���
//    [SerializeField] private StageManager stageManager;

//    // ��ü �� ũ��� ������ �� ���� �Ķ����
//    public int width = 100;          // �� ���� �� ��
//    public int height = 100;         // �� ���� �� ��
//    public GameObject wallPrefab;    // �� ������. 1 x 1 ũ�� �������� ��ġ
//    public GameObject floorPrefab;   // �ٴ� ������. 10 x 10 ũ�� ���׸�Ʈ�� ��ġ
//    public int minRoomSize = 10;     // �� �ּ� ũ��. BSP ���� �� ���
//    public int maxRoomSize = 24;     // �� �ִ� ũ��. BSP ���� �� ���
//    public int maxDepth = 20;        // BSP ��� ���� �ִ� ����
//    public int corridorWidth = 5;    // ��� ��. Ȧ�� ����
//    public GameObject portalPrefab;  // ��Ż ������. ���� �������� Ŭ���� ����
//    public float portalYOffset = 0f; // ��Ż�� Y ��ġ ������
//    public int bossRoomWidth = 28;   // ������ ���� ũ��. ���� �������������� ���
//    public int bossRoomHeight = 28;  // ������ ���� ũ��. ���� �������������� ���

//    // ���� ���°���
//    private RectInt bossRoom;        // ���� ���� ���� ����. ������ �ʺ�� ���̰� 0
//    private int[,] map;              // �� ��� �� ������. 0�� �ٴ�. 1�� ��
//    private List<RectInt> rooms;     // BSP�� ������ �Ϲ� �� ���
//    private RectInt playerRoom;      // �÷��̾� ���� ���� ��

//    // �������� �׸� ���� ����
//    [Header("Stage Themes")]
//    public int stagesPerTheme = 5;           // ������ ���� ������������ ���� �׸��� ���
//    public StageTheme[] themes;              // �׸� �迭
//    public bool useBossOverrideTheme = true; // ���� ������������ ���� �׸� ��� ����
//    public StageTheme bossOverrideTheme;     // ���� �������� ���� �׸�

//    // ���� ���������� ������ �׸��� ��ȯ
//    private StageTheme GetActiveTheme()
//    {
//        // ���� ���������̰� ���� �׸��� �ִٸ� ���� �׸��� �켱 ���
//        if (useBossOverrideTheme && stageManager != null && stageManager.IsBossStage() && bossOverrideTheme != null)
//            return bossOverrideTheme;

//        // �Ϲ� �׸��� �ϳ��� ���ٸ� null ��ȯ
//        if (themes == null || themes.Length == 0) return null;

//        // ���� ���������� �������� �׸� �ε����� ���
//        int stage = (stageManager != null) ? stageManager.currentStage : 1;
//        int idx = Mathf.FloorToInt((stage - 1) / Mathf.Max(1, stagesPerTheme));
//        idx = Mathf.Clamp(idx, 0, themes.Length - 1);
//        return themes[idx];
//    }

//    // �� ���� �Ϸ�� �ܺο� �˸��� ���� �̺�Ʈ. ���� �� ��ó���� ������ �� �ִ�
//    public delegate void MapGeneratedHandler();
//    public event MapGeneratedHandler OnMapGenerated;

//    // �������� ��ȿ�ϰ� �����Ǿ����� �Ǵ��ϴ� ���� �Ӽ�
//    private bool HasBossRoom => bossRoom.width > 0 && bossRoom.height > 0;

//    // ����Ƽ ����������Ŭ. ���� �� ���� �����ϰ� �����Ѵ�
//    void Start()
//    {
//        GenerateMap(); // ������ ���� �ܰ�
//        RenderMap();   // ������ ��ġ �ܰ�
//    }

//    // �� ������ ���� ����. �� ����. ��� ����. ������� ��Ż ��ġ���� ���
//    public void GenerateMap()
//    {
//        // �� �迭�� �÷��� �ʱ�ȭ
//        map = new int[width, height];
//        rooms = new List<RectInt>();
//        bossRoom = new RectInt(0, 0, 0, 0);

//        // ��ü�� ������ ä���
//        for (int x = 0; x < width; x++)
//            for (int y = 0; y < height; y++)
//                map[x, y] = 1;

//        // �÷��̾� ���� ���� ���� ���� �����Ѵ�
//        playerRoom = new RectInt(2, 2, 25, 25);

//        // BSP ���ҷ� �Ϲ� �� �ĺ����� �����. ��迡�� �� ĭ ������ �д�
//        RectInt root = new RectInt(1, 1, width - 2, height - 2);
//        SplitRoom(root, maxDepth, rooms);

//        // �÷��̾� ���� �� �׵θ��� �ٴ� ���η� ĳ���Ѵ�
//        for (int x = playerRoom.xMin; x < playerRoom.xMax; x++)
//            for (int y = playerRoom.yMin; y < playerRoom.yMax; y++)
//                map[x, y] = 1; // �׵θ� ���� �켱 �� ó��
//        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
//            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
//                map[x, y] = 0; // ���δ� �ٴ� ó��

//        // �Ϲ� ����� ĳ���Ѵ�. �÷��̾� ��� ��ġ�� ���� �����Ѵ�
//        foreach (var room in rooms)
//        {
//            if (!room.Overlaps(playerRoom))
//            {
//                for (int x = room.xMin; x < room.xMax; x++)
//                    for (int y = room.yMin; y < room.yMax; y++)
//                        map[x, y] = 0;
//            }
//        }

//        // ���� ���������� ���� ������������ Ȯ���Ѵ�
//        bool makeBossRoom = (stageManager != null && stageManager.IsBossStage());

//        // ���� ����������� �� �߾ӿ� �������� ����� ���θ� �ٴ����� �����
//        if (makeBossRoom)
//        {
//            int bw = Mathf.Clamp(bossRoomWidth, minRoomSize, width - 4);
//            int bh = Mathf.Clamp(bossRoomHeight, minRoomSize, height - 4);
//            int bx = (width - bw) / 2;
//            int by = (height - bh) / 2;
//            bossRoom = new RectInt(bx, by, bw, bh);

//            for (int x = bossRoom.xMin; x < bossRoom.xMax; x++)
//                for (int y = bossRoom.yMin; y < bossRoom.yMax; y++)
//                    map[x, y] = 0;
//        }

//        // �Ϲ� ����� �߽� ��ǥ ����� �����. �÷��̾� ��� �������� �����Ѵ�
//        List<Vector2Int> centers = rooms
//            .Where(r => !r.Overlaps(playerRoom) && !r.Overlaps(bossRoom))
//            .Select(r => new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y)))
//            .ToList();

//        // �ּ� ���� Ʈ�� ������� �Ϲ� ����� ������ �����Ѵ�
//        List<Vector2Int> connected = new List<Vector2Int>();
//        List<Vector2Int> remaining = new List<Vector2Int>(centers);
//        if (remaining.Count > 0) { connected.Add(remaining[0]); remaining.RemoveAt(0); }

//        while (remaining.Count > 0)
//        {
//            float minDist = float.MaxValue; Vector2Int a = Vector2Int.zero, b = Vector2Int.zero;
//            foreach (var c in connected)
//                foreach (var r in remaining)
//                {
//                    float dist = Vector2Int.Distance(c, r);
//                    if (dist < minDist) { minDist = dist; a = c; b = r; }
//                }
//            CreateCorridor(a, b);
//            connected.Add(b);
//            remaining.Remove(b);
//        }

//        // �÷��̾� �濡�� ���� ����� �Ϲ� ����� ���� ������ �����
//        Vector2Int playerCenter = new Vector2Int(
//            Mathf.RoundToInt(playerRoom.center.x),
//            Mathf.RoundToInt(playerRoom.center.y)
//        );
//        if (centers.Count > 0)
//        {
//            Vector2Int nearestRoomCenter = centers.OrderBy(c => Vector2Int.Distance(c, playerCenter)).First();
//            Vector2Int corridorStart = new Vector2Int((playerRoom.xMin + playerRoom.xMax) / 2, playerRoom.yMax);

//            int offset = Mathf.Max(corridorWidth / 2, 1);
//            for (int w = -offset; w <= offset; w++)
//            {
//                int x = corridorStart.x + w, y = corridorStart.y;
//                if (x >= playerRoom.xMin && x < playerRoom.xMax && y < height)
//                {
//                    map[x, y] = 0; map[x, y - 1] = 0;
//                }
//            }
//            CreatePlayerCorridor(corridorStart, nearestRoomCenter);
//        }

//        // ���� ����������� ������� �Ϲ� ����� �����ϴ� �Ա��� �����
//        if (makeBossRoom)
//            ConnectBossRoomEntrances(centers);

//        // ��Ż�� ��ġ�Ѵ�. �÷��̾� �濡�� ���� �� �Ϲ� ���� �߽ɿ� �д�
//        PlacePortal();

//        // �� ������ �������� �̺�Ʈ�� �˸���
//        OnMapGenerated?.Invoke();
//    }

//    // ������ �����ڸ����� ����� �Ϲ� ������ �̾����� �Ա��� ������ �����
//    void ConnectBossRoomEntrances(List<Vector2Int> normalCenters)
//    {
//        if (normalCenters == null || normalCenters.Count == 0) return;

//        // ������ �� ���� �߾� �������� �ĺ��� ��´�
//        var edgePoints = new List<Vector2Int>
//        {
//            new Vector2Int(bossRoom.xMin, (bossRoom.yMin + bossRoom.yMax) / 2),
//            new Vector2Int(bossRoom.xMax - 1, (bossRoom.yMin + bossRoom.yMax) / 2),
//            new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMin),
//            new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMax - 1)
//        };

//        int made = 0;
//        foreach (var ep in edgePoints)
//        {
//            // ���� ����� �Ϲ� �� �߽��� ã�´�
//            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, ep)).FirstOrDefault();

//            // �Ա� ���� corridorWidth�� ���� ������
//            int offset = Mathf.Max(corridorWidth / 2, 1);

//            // ������ ������ ������ ������ ����Ѵ�
//            Vector2Int dir = Vector2Int.zero;
//            if (ep.x == bossRoom.xMin) dir = Vector2Int.left;
//            else if (ep.x == bossRoom.xMax - 1) dir = Vector2Int.right;
//            else if (ep.y == bossRoom.yMin) dir = Vector2Int.down;
//            else if (ep.y == bossRoom.yMax - 1) dir = Vector2Int.up;

//            // ���� ĳ���Ѵ�. �Ա��� �� �ٱ� �� ĭ�� �ٴ����� �����
//            for (int w = -offset; w <= offset; w++)
//            {
//                int x = ep.x + (dir.x == 0 ? w : 0);
//                int y = ep.y + (dir.y == 0 ? w : 0);
//                int dx = ep.x + dir.x;
//                int dy = ep.y + dir.y;

//                if (x >= 0 && x < width && y >= 0 && y < height) map[x, y] = 0;
//                if (dx >= 0 && dx < width && dy >= 0 && dy < height) map[dx, dy] = 0;
//            }

//            // �Ա����� �Ϲ� �� �߽ɱ��� ������ �����
//            CreatePlayerCorridor(ep, nearest);
//            made++;
//        }

//        // ��� ������ ������ ��츦 ����Ͽ� ������ �߾ӿ��� �ѹ� �� �����Ѵ�
//        if (made == 0)
//        {
//            var bossC = new Vector2Int(Mathf.RoundToInt(bossRoom.center.x), Mathf.RoundToInt(bossRoom.center.y));
//            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, bossC)).First();
//            CreatePlayerCorridor(bossC, nearest);
//        }
//    }

//    // ��Ż�� �����Ѵ�. �÷��̾� ��� ���� �ָ� ������ �Ϲ� �� �߽ɿ� �д�
//    void PlacePortal()
//    {
//        if (portalPrefab == null || rooms == null || rooms.Count == 0) return;

//        Vector2Int playerCenter = new Vector2Int(
//            Mathf.RoundToInt(playerRoom.center.x),
//            Mathf.RoundToInt(playerRoom.center.y)
//        );

//        RectInt farthestRoom = rooms
//            .Where(r => !r.Overlaps(playerRoom))
//            .OrderByDescending(r =>
//            {
//                var c = new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
//                return Vector2Int.Distance(playerCenter, c);
//            })
//            .FirstOrDefault();

//        if (farthestRoom.width == 0 || farthestRoom.height == 0) return;

//        Vector3 portalPos = new Vector3(farthestRoom.center.x, portalYOffset, farthestRoom.center.y);

//        GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.identity, transform);

//        Collider col = portal.GetComponent<Collider>();
//        if (col == null) col = portal.AddComponent<BoxCollider>();
//        col.isTrigger = true;

//        PortalTrigger trigger = portal.GetComponent<PortalTrigger>();
//        if (trigger == null) trigger = portal.AddComponent<PortalTrigger>();
//        trigger.Setup(this);
//    }

//    // BSP ����. ������ �ɰ��� ���� �����
//    void SplitRoom(RectInt space, int depth, List<RectInt> rooms)
//    {
//        // �� �ɰ� �� ���ų� ������ �ʹ� �۴ٸ� �� ���� �ȿ��� �ϳ��� ���� �����
//        if (depth == 0 || space.width < minRoomSize * 2 || space.height < minRoomSize * 2)
//        {
//            int roomWidth = Random.Range(minRoomSize, Mathf.Min(space.width, maxRoomSize));
//            int roomHeight = Random.Range(minRoomSize, Mathf.Min(space.height, maxRoomSize));
//            int roomX = Random.Range(space.xMin + 1, space.xMax - roomWidth - 1);
//            int roomY = Random.Range(space.yMin + 1, space.yMax - roomHeight - 1);
//            RectInt newRoom = new RectInt(roomX, roomY, roomWidth, roomHeight);

//            float minDistanceFromPlayer = 8f; // �÷��̾� ����� �ּ� �Ÿ�
//            Vector2Int newCenter = new Vector2Int(Mathf.RoundToInt(newRoom.center.x), Mathf.RoundToInt(newRoom.center.y));
//            Vector2Int playerCenter = new Vector2Int(Mathf.RoundToInt(playerRoom.center.x), Mathf.RoundToInt(playerRoom.center.y));

//            if (!newRoom.Overlaps(playerRoom) && Vector2Int.Distance(newCenter, playerCenter) > minDistanceFromPlayer)
//            {
//                rooms.Add(newRoom);
//            }
//            return;
//        }

//        // ���� �Ǵ� �������� ���� �����Ѵ�
//        bool splitHorizontally = Random.value > 0.5f;
//        if (splitHorizontally)
//        {
//            int splitY = Random.Range(space.yMin + minRoomSize, space.yMax - minRoomSize);
//            SplitRoom(new RectInt(space.xMin, space.yMin, space.width, splitY - space.yMin), depth - 1, rooms);
//            SplitRoom(new RectInt(space.xMin, splitY, space.width, space.yMax - splitY), depth - 1, rooms);
//        }
//        else
//        {
//            int splitX = Random.Range(space.xMin + minRoomSize, space.xMax - minRoomSize);
//            SplitRoom(new RectInt(space.xMin, space.yMin, splitX - space.xMin, space.height), depth - 1, rooms);
//            SplitRoom(new RectInt(splitX, space.yMin, space.xMax - splitX, space.height), depth - 1, rooms);
//        }
//    }

//    // �� �� �߽��� ���� ���� ������׷� �����ϴ� ������ �����
//    void CreateCorridor(Vector2Int a, Vector2Int b)
//    {
//        if (Random.value > 0.5f)
//        {
//            DrawHorizontalCorridor(a.x, b.x, a.y);
//            DrawVerticalCorridor(a.y, b.y, b.x);
//        }
//        else
//        {
//            DrawVerticalCorridor(a.y, b.y, a.x);
//            DrawHorizontalCorridor(a.x, b.x, b.y);
//        }
//    }

//    // �÷��̾� �濡�� Ư�� ����� ���� �ڿ������� ��θ� �����. �߰� ������ �ణ �����ϰ� �д�
//    void CreatePlayerCorridor(Vector2Int start, Vector2Int end)
//    {
//        int offset = Mathf.Max(corridorWidth / 2, 1);

//        Vector2Int mid = new Vector2Int(
//            (start.x + end.x) / 2 + Random.Range(-2, 3),
//            (start.y + end.y) / 2 + Random.Range(-2, 3)
//        );

//        for (int w = -offset; w <= offset; w++)
//            DigCorridor(new Vector2Int(start.x + w, start.y), new Vector2Int(mid.x + w, mid.y));

//        for (int w = -offset; w <= offset; w++)
//            DigCorridor(new Vector2Int(mid.x + w, mid.y), new Vector2Int(end.x + w, end.y));
//    }

//    // �� ���� �������� �մ� ������ ���� ���� �ĳ���
//    void DigCorridor(Vector2Int start, Vector2Int end)
//    {
//        Vector2Int current = start;

//        int xDir = (end.x > current.x) ? 1 : -1;
//        while (current.x != end.x)
//        {
//            current.x += xDir;
//            for (int w = -1; w <= 1; w++)
//            {
//                int yy = current.y + w;
//                if (!playerRoom.Contains(new Vector2Int(current.x, yy)))
//                    map[current.x, yy] = 0;
//            }
//        }

//        int yDir = (end.y > current.y) ? 1 : -1;
//        while (current.y != end.y)
//        {
//            current.y += yDir;
//            for (int w = -1; w <= 1; w++)
//            {
//                int xx = current.x + w;
//                if (!playerRoom.Contains(new Vector2Int(xx, current.y)))
//                    map[xx, current.y] = 0;
//            }
//        }
//    }

//    // ���� ������ ���� ������ �ĳ���
//    void DrawHorizontalCorridor(int xStart, int xEnd, int y)
//    {
//        int offset = corridorWidth / 2;
//        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
//            for (int w = -offset; w <= offset; w++)
//            {
//                int yy = y + w;
//                if (yy >= 0 && yy < height && !playerRoom.Contains(new Vector2Int(x, yy)))
//                    map[x, yy] = 0;
//            }
//    }

//    // ���� ������ ���� ������ �ĳ���
//    void DrawVerticalCorridor(int yStart, int yEnd, int x)
//    {
//        int offset = corridorWidth / 2;
//        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
//            for (int w = -offset; w <= offset; w++)
//            {
//                int xx = x + w;
//                if (xx >= 0 && xx < width && !playerRoom.Contains(new Vector2Int(xx, y)))
//                    map[xx, y] = 0;
//            }
//    }

//    // �� �����͸� ���� ���� ������Ʈ�� �������Ѵ�. �ٴ� Ÿ�ϰ� �� Ÿ���� ��ġ�ϰ� �׸� ��Ƽ������ �����Ѵ�
//    void RenderMap()
//    {
//        float floorSize = 10f;
//        var theme = GetActiveTheme();

//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                // �ٴ� Ÿ���� 10 �������� ��ġ�Ͽ� ������ �ν��Ͻ��� ���δ�
//                if (x % (int)floorSize == 0 && y % (int)floorSize == 0)
//                {
//                    Vector3 floorPos = new Vector3(x + 5, 0, y + 5);
//                    var floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);

//                    // �׸� �ٴ� ��Ƽ���� ����
//                    if (theme != null && theme.floorMaterial != null)
//                    {
//                        var rend = floor.GetComponentInChildren<Renderer>();
//                        if (rend != null) rend.sharedMaterial = theme.floorMaterial;
//                    }
//                }

//                // �� �����Ͱ� ���� ������ �� �������� 1 x 1�� ��ġ�Ѵ�
//                if (map[x, y] == 1)
//                {
//                    float wallHeight = wallPrefab.transform.localScale.y;
//                    Vector3 wallPos = new Vector3(x, wallHeight / 5f, y);
//                    var wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);

//                    // �׸� �� ��Ƽ���� ����
//                    if (theme != null && theme.wallMaterial != null)
//                    {
//                        var rend = wall.GetComponentInChildren<Renderer>();
//                        if (rend != null) rend.sharedMaterial = theme.wallMaterial;
//                    }
//                }
//            }
//        }
//    }

//    // ���� ��ǥ�� �� �����̰� �ٴ����� Ȯ���Ѵ�
//    public bool IsFloor(int x, int y)
//    {
//        if (x < 0 || x >= width || y < 0 || y >= height) return false;
//        return map[x, y] == 0;
//    }

//    // �Ϲ� �� ����� ��ȯ�Ѵ�. �������� ���� ��� ��ġ�� ���� �����Ѵ�
//    public List<RectInt> GetRooms()
//    {
//        if (HasBossRoom)
//            return rooms.Where(r => r != playerRoom && !r.Overlaps(bossRoom)).ToList();
//        else
//            return rooms.Where(r => r != playerRoom).ToList();
//    }

//    // ������ ���� ��ȯ
//    public RectInt GetBossRoom() => bossRoom;

//    // �÷��̾� �� ���� ��ȯ
//    public RectInt GetPlayerRoom() => playerRoom;

//    // ���� �ٽ� �����. ���� �ڽ� ������Ʈ�� �����ϰ� ������ ������ ������ �ٽ� �����Ѵ�
//    public void ReloadMap()
//    {
//        for (int i = transform.childCount - 1; i >= 0; i--)
//            Destroy(transform.GetChild(i).gameObject);

//        GenerateMap();
//        RenderMap();
//    }
//}
using System.Collections.Generic;
using System.Linq;
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

    public int width = 100;
    public int height = 100;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public int minRoomSize = 10;
    public int maxRoomSize = 24;
    // �ν����Ϳ��� ���� ����
    [Range(0.1f, 0.9f)] public float minimumDevideRate = 0.45f;
    [Range(0.1f, 0.9f)] public float maximumDivideRate = 0.55f;
    [Range(1, 12)] public int maxDepth = 6;
    [Range(1, 9)] public int corridorWidth = 5;

    public GameObject portalPrefab;
    public float portalYOffset = 0f;
    public int bossRoomWidth = 28;
    public int bossRoomHeight = 28;

    private int[,] map;                 // 0 �ٴ� 1 ��
    private List<RectInt> rooms;        // �������� ���� ��
    private RectInt playerRoom;         // ���� ��
    private RectInt bossRoom;           // ���� ��

    [Header("Stage Themes")]
    public int stagesPerTheme = 5;
    public StageTheme[] themes;
    public bool useBossOverrideTheme = true;
    public StageTheme bossOverrideTheme;

    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    private bool HasBossRoom => bossRoom.width > 0 && bossRoom.height > 0;

    void Start()
    {
        GenerateMap();
        RenderMap();
    }

    private StageTheme GetActiveTheme()
    {
        if (useBossOverrideTheme && stageManager != null && stageManager.IsBossStage() && bossOverrideTheme != null)
            return bossOverrideTheme;

        if (themes == null || themes.Length == 0) return null;

        int stage = (stageManager != null) ? stageManager.currentStage : 1;
        int idx = Mathf.FloorToInt((stage - 1) / Mathf.Max(1, stagesPerTheme));
        idx = Mathf.Clamp(idx, 0, themes.Length - 1);
        return themes[idx];
    }

    public void GenerateMap()
    {
        InitMap();

        // 1 �÷��̾� �� ���� ĳ��
        CarvePlayerRoom();

        // 2 BSP ����
        Node root = new Node(new RectInt(1, 1, width - 2, height - 2));
        SplitRoom(root, 0);

        // 3 �������� �� ���� ĳ��
        GenerateRooms(root);

        // 4 ���� ���������� �߾ӿ� ���� �� ĳ��
        bool isBoss = stageManager != null && stageManager.IsBossStage();
        if (isBoss) CarveBossRoomCenter();

        // 5 �÷��̾� ��� ���� ����� �Ϲ� ���� �ݵ�� ����
        ConnectPlayerRoomToNearestRoom();

        // 6 BSP Ʈ���� ���� ���� ���� ���� �� ���� ����
        GenerateTreeCorridors(root);

        // 7 ������ �Ա� ����
        if (isBoss)
        {
            var centers = rooms.Where(r => !r.Overlaps(playerRoom) && !r.Overlaps(bossRoom))
                               .Select(r => new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y)))
                               .ToList();
            ConnectBossRoomEntrances(centers);
        }

        // 8 ��Ż ��ġ
        PlacePortal();

        OnMapGenerated?.Invoke();
    }

    void InitMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();
        bossRoom = new RectInt(0, 0, 0, 0);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        playerRoom = new RectInt(2, 2, 25, 25);
    }

    void CarvePlayerRoom()
    {
        // �׵θ��� �� ���� ���θ� �ٴ�
        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
                map[x, y] = 0;
    }

    //bool CanSplit(RectInt r)
    //{
    //    return (r.width >= minRoomSize * 2) || (r.height >= minRoomSize * 2);
    //}

    // �� ���� ��������
    bool CanSplit(RectInt r)
    {
        const int minLeaf = 10;  // ���� �ּ� ��/����
        return (r.width >= minLeaf * 2) || (r.height >= minLeaf * 2);
    }

    //void SplitRoom(Node node, int depth)
    //{
    //    if (depth >= maxDepth || !CanSplit(node.nodeRect))
    //        return;

    //    bool splitHoriz = node.nodeRect.width >= node.nodeRect.height;
    //    if (splitHoriz)
    //    {
    //        int minY = node.nodeRect.yMin + minRoomSize;
    //        int maxY = node.nodeRect.yMax - minRoomSize;
    //        if (minY >= maxY) return;

    //        int splitY = Random.Range(minY, maxY);
    //        node.leftNode = new Node(new RectInt(node.nodeRect.xMin, node.nodeRect.yMin, node.nodeRect.width, splitY - node.nodeRect.yMin));
    //        node.rightNode = new Node(new RectInt(node.nodeRect.xMin, splitY, node.nodeRect.width, node.nodeRect.yMax - splitY));
    //    }
    //    else
    //    {
    //        int minX = node.nodeRect.xMin + minRoomSize;
    //        int maxX = node.nodeRect.xMax - minRoomSize;
    //        if (minX >= maxX) return;

    //        int splitX = Random.Range(minX, maxX);
    //        node.leftNode = new Node(new RectInt(node.nodeRect.xMin, node.nodeRect.yMin, splitX - node.nodeRect.xMin, node.nodeRect.height));
    //        node.rightNode = new Node(new RectInt(splitX, node.nodeRect.yMin, node.nodeRect.xMax - splitX, node.nodeRect.height));
    //    }

    //    node.leftNode.parNode = node;
    //    node.rightNode.parNode = node;

    //    SplitRoom(node.leftNode, depth + 1);
    //    SplitRoom(node.rightNode, depth + 1);
    //}

    // ���Ҹ� ����, map�� �ǵ帮�� ����
    void SplitRoom(Node tree, int depth)
    {
        if (depth >= maxDepth || !CanSplit(tree.nodeRect))
            return;

        bool splitHoriz = tree.nodeRect.width >= tree.nodeRect.height;
        int axisLen = splitHoriz ? tree.nodeRect.width : tree.nodeRect.height;

        // ���� ������ 1..axisLen-1�� ����
        int minSplit = Mathf.Clamp(Mathf.RoundToInt(axisLen * minimumDevideRate), 1, axisLen - 1);
        int maxSplit = Mathf.Clamp(Mathf.RoundToInt(axisLen * maximumDivideRate), minSplit, axisLen - 1);
        if (minSplit >= maxSplit) return;

        int split = Random.Range(minSplit, maxSplit + 1);

        if (splitHoriz)
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
        }

        tree.leftNode.parNode = tree;
        tree.rightNode.parNode = tree;

        SplitRoom(tree.leftNode, depth + 1);
        SplitRoom(tree.rightNode, depth + 1);
    }

    RectInt GenerateRooms(Node node)
    {
        // ����
        if (node.leftNode == null && node.rightNode == null)
        {
            RectInt r = node.nodeRect;

            int minW = Mathf.Clamp(r.width / 2, 4, Mathf.Max(4, r.width - 1));
            int minH = Mathf.Clamp(r.height / 2, 4, Mathf.Max(4, r.height - 1));
            int maxW = Mathf.Max(minW + 1, Mathf.Min(maxRoomSize, r.width - 1));
            int maxH = Mathf.Max(minH + 1, Mathf.Min(maxRoomSize, r.height - 1));

            if (minW >= maxW || minH >= maxH)
            {
                node.roomRect = r;
            }
            else
            {
                int w = Random.Range(minW, maxW);
                int h = Random.Range(minH, maxH);
                int x = r.x + Random.Range(1, Mathf.Max(1, r.width - w));
                int y = r.y + Random.Range(1, Mathf.Max(1, r.height - h));
                node.roomRect = new RectInt(x, y, w, h);
            }

            // �÷��̾� ��� ��ġ�� ����
            if (!node.roomRect.Overlaps(playerRoom))
            {
                rooms.Add(node.roomRect);
                for (int x = node.roomRect.xMin; x < node.roomRect.xMax; x++)
                    for (int y = node.roomRect.yMin; y < node.roomRect.yMax; y++)
                        map[x, y] = 0;
            }

            return node.roomRect;
        }

        // ���� ���� �ڽ� ó��
        RectInt left = (node.leftNode != null) ? GenerateRooms(node.leftNode) : new RectInt();
        RectInt right = (node.rightNode != null) ? GenerateRooms(node.rightNode) : new RectInt();
        node.roomRect = (left.width > 0) ? left : right;
        return node.roomRect;
    }

    void ConnectPlayerRoomToNearestRoom()
    {
        if (rooms == null || rooms.Count == 0) return;

        Vector2Int p = new Vector2Int(Mathf.RoundToInt(playerRoom.center.x), Mathf.RoundToInt(playerRoom.center.y));

        RectInt nearest = default;
        float best = float.MaxValue;
        foreach (var r in rooms)
        {
            var c = new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
            float d = Vector2Int.Distance(p, c);
            if (d < best) { best = d; nearest = r; }
        }
        if (nearest.width == 0 || nearest.height == 0) return;

        // �÷��̾� �� ��� �߾ӿ� �� �ձ�
        Vector2Int doorway = new Vector2Int((playerRoom.xMin + playerRoom.xMax) / 2, playerRoom.yMax - 1);
        int half = Mathf.Max(1, corridorWidth / 2);
        for (int w = -half; w <= half; w++)
        {
            int dx = doorway.x + w;
            int dy = doorway.y + 1;
            if (Inside(dx, doorway.y)) map[dx, doorway.y] = 0;
            if (Inside(dx, dy)) map[dx, dy] = 0;
        }

        Vector2Int target = new Vector2Int(Mathf.RoundToInt(nearest.center.x), Mathf.RoundToInt(nearest.center.y));
        DigCorridor(doorway, target);
    }

    void GenerateTreeCorridors(Node node)
    {
        if (node.leftNode == null || node.rightNode == null)
            return;

        Vector2Int a = node.leftNode.center;
        Vector2Int b = node.rightNode.center;
        DigCorridor(a, b);

        GenerateTreeCorridors(node.leftNode);
        GenerateTreeCorridors(node.rightNode);
    }

    void DigCorridor(Vector2Int a, Vector2Int b)
    {
        int half = Mathf.Max(1, corridorWidth / 2);

        // ����
        int x0 = Mathf.Min(a.x, b.x);
        int x1 = Mathf.Max(a.x, b.x);
        for (int x = x0; x <= x1; x++)
            for (int w = -half; w <= half; w++)
            {
                int yy = a.y + w;
                if (Inside(x, yy)) map[x, yy] = 0;
            }

        // ����
        int y0 = Mathf.Min(a.y, b.y);
        int y1 = Mathf.Max(a.y, b.y);
        for (int y = y0; y <= y1; y++)
            for (int w = -half; w <= half; w++)
            {
                int xx = b.x + w;
                if (Inside(xx, y)) map[xx, y] = 0;
            }
    }

    bool Inside(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    void CarveBossRoomCenter()
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

    void ConnectBossRoomEntrances(List<Vector2Int> normalCenters)
    {
        if (normalCenters == null || normalCenters.Count == 0 || !HasBossRoom) return;

        var edgePoints = new List<Vector2Int>
        {
            new Vector2Int(bossRoom.xMin, (bossRoom.yMin + bossRoom.yMax) / 2),
            new Vector2Int(bossRoom.xMax - 1, (bossRoom.yMin + bossRoom.yMax) / 2),
            new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMin),
            new Vector2Int((bossRoom.xMin + bossRoom.xMax) / 2, bossRoom.yMax - 1)
        };

        int half = Mathf.Max(1, corridorWidth / 2);
        int made = 0;

        foreach (var ep in edgePoints)
        {
            var nearest = normalCenters.OrderBy(c => Vector2Int.Distance(c, ep)).FirstOrDefault();

            Vector2Int dir = Vector2Int.zero;
            if (ep.x == bossRoom.xMin) dir = Vector2Int.left;
            else if (ep.x == bossRoom.xMax - 1) dir = Vector2Int.right;
            else if (ep.y == bossRoom.yMin) dir = Vector2Int.down;
            else if (ep.y == bossRoom.yMax - 1) dir = Vector2Int.up;

            for (int w = -half; w <= half; w++)
            {
                int x = ep.x + (dir.x == 0 ? w : 0);
                int y = ep.y + (dir.y == 0 ? w : 0);
                int dx = ep.x + dir.x;
                int dy = ep.y + dir.y;

                if (Inside(x, y)) map[x, y] = 0;
                if (Inside(dx, dy)) map[dx, dy] = 0;
            }

            DigCorridor(ep, nearest);
            made++;
        }

        if (made == 0)
        {
            var c = new Vector2Int(Mathf.RoundToInt(bossRoom.center.x), Mathf.RoundToInt(bossRoom.center.y));
            var nearest = normalCenters.OrderBy(v => Vector2Int.Distance(v, c)).First();
            DigCorridor(c, nearest);
        }
    }

    void PlacePortal()
    {
        if (portalPrefab == null || rooms == null || rooms.Count == 0) return;

        Vector2Int pc = new Vector2Int(Mathf.RoundToInt(playerRoom.center.x), Mathf.RoundToInt(playerRoom.center.y));

        RectInt farthestRoom = rooms
            .Where(r => !r.Overlaps(playerRoom))
            .OrderByDescending(r =>
            {
                var c = new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
                return Vector2Int.Distance(pc, c);
            })
            .FirstOrDefault();

        if (farthestRoom.width == 0 || farthestRoom.height == 0) return;

        Vector3 pos = new Vector3(farthestRoom.center.x, portalYOffset, farthestRoom.center.y);
        GameObject portal = Instantiate(portalPrefab, pos, Quaternion.identity, transform);

        Collider col = portal.GetComponent<Collider>();
        if (col == null) col = portal.AddComponent<BoxCollider>();
        col.isTrigger = true;

        PortalTrigger trigger = portal.GetComponent<PortalTrigger>();
        if (trigger == null) trigger = portal.AddComponent<PortalTrigger>();
        trigger.Setup(this);
    }

    void RenderMap()
    {
        float floorStep = 10f;
        var theme = GetActiveTheme();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (floorPrefab != null && x % (int)floorStep == 0 && y % (int)floorStep == 0)
                {
                    var floorPos = new Vector3(x + 5, 0f, y + 5);
                    var floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);
                    if (theme != null && theme.floorMaterial != null)
                    {
                        var rend = floor.GetComponentInChildren<Renderer>();
                        if (rend != null) rend.sharedMaterial = theme.floorMaterial;
                    }
                }

                if (map[x, y] == 1 && wallPrefab != null)
                {
                    float wallH = wallPrefab.transform.localScale.y;
                    var wallPos = new Vector3(x, wallH / 5f, y);
                    var wall = Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                    if (theme != null && theme.wallMaterial != null)
                    {
                        var rend = wall.GetComponentInChildren<Renderer>();
                        if (rend != null) rend.sharedMaterial = theme.wallMaterial;
                    }
                }
            }
        }
    }

    public bool IsFloor(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return map[x, y] == 0;
    }

    public List<RectInt> GetRooms()
    {
        if (HasBossRoom)
            return rooms.Where(r => !r.Overlaps(bossRoom)).ToList();
        return new List<RectInt>(rooms);
    }

    public RectInt GetBossRoom() => bossRoom;
    public RectInt GetPlayerRoom() => playerRoom;

    public void ReloadMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        GenerateMap();
        RenderMap();
    }
}
