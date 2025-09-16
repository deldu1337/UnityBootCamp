using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileMapGenerator : MonoBehaviour
{
    public int width = 100; // �� ���� ũ��
    public int height = 100; // �� ���� ũ��
    public GameObject wallPrefab;  // �� ������
    public GameObject floorPrefab; // �ٴ� ������
    public int minRoomSize = 10;   // �� �ּ� ũ��
    public int maxRoomSize = 24;   // �� �ִ� ũ��
    public int maxDepth = 20;      // BSP ���� �ִ� ����
    public int corridorWidth = 5;  // ��� ��

    private int[,] map; // 2D �� ������: 0=�ٴ�, 1=��
    private List<RectInt> rooms; // �Ϲ� �� ����Ʈ
    private RectInt playerRoom;  // �÷��̾� ���� ��

    // �� ���� �Ϸ� �̺�Ʈ
    public delegate void MapGeneratedHandler();
    public event MapGeneratedHandler OnMapGenerated;

    void Start()
    {
        GenerateMap(); // �� ������ ����
        RenderMap();   // �� ������Ʈ ����
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();

        // ��ü ������ �ʱ�ȭ
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // BSP�� �� ����
        RectInt root = new RectInt(1, 1, width - 2, height - 2);
        SplitRoom(root, maxDepth, rooms);

        // �÷��̾� ���� �� ���� (10x10)
        playerRoom = new RectInt(2, 2, 10, 10);

        // �÷��̾� �� �� �ʱ�ȭ
        for (int x = playerRoom.xMin; x < playerRoom.xMax; x++)
            for (int y = playerRoom.yMin; y < playerRoom.yMax; y++)
                map[x, y] = 1;

        // �÷��̾� �� ���� �ٴ� ���� (�����ڸ� ����)
        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
                map[x, y] = 0;

        // �Ϲ� �� �ٴ� ���� (�÷��̾� �� ����)
        foreach (var room in rooms)
        {
            if (!room.Overlaps(playerRoom))
            {
                for (int x = room.xMin; x < room.xMax; x++)
                    for (int y = room.yMin; y < room.yMax; y++)
                        map[x, y] = 0;
            }
        }

        // �� �߽� ��ǥ ���
        List<Vector2Int> centers = rooms.Select(r =>
            new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y))
        ).ToList();

        // MST ���� (Prim �˰���) - ����� �ּ� ���� ��η� ����
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

            // ����� ��� ���� �� ������ �ּ� �Ÿ� �� ����
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

            CreateCorridor(a, b); // ��� ����
            connected.Add(b);
            remaining.Remove(b);
        }

        // �÷��̾� ��� ���� ����� �� ���� (�׻� ���� ���� �켱)
        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );

        if (centers.Count > 0)
        {
            Vector2Int nearestRoomCenter = centers
                .OrderBy(c => Vector2Int.Distance(c, playerCenter))
                .First();

            // ��� ������: �÷��̾� �� ���� �߾�
            Vector2Int corridorStart = new Vector2Int(
                (playerRoom.xMin + playerRoom.xMax) / 2,
                playerRoom.yMax
            );

            // �÷��̾� �� �� �ձ� (�ּ� corridorWidth ����)
            int offset = Mathf.Max(corridorWidth / 2, 1);
            for (int w = -offset; w <= offset; w++)
            {
                int x = corridorStart.x + w;
                int y = corridorStart.y;
                if (x >= playerRoom.xMin && x < playerRoom.xMax && y < height)
                {
                    map[x, y] = 0;
                    map[x, y - 1] = 0; // �Ա� �� Ȯ��
                }
            }

            // �÷��̾� �� �� ���� ����� �� ��� ����
            CreatePlayerCorridor(corridorStart, nearestRoomCenter);
        }

        // �� ���� �Ϸ� �̺�Ʈ ȣ��
        OnMapGenerated?.Invoke();
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
        float floorSize = 10f; // �ٴ� ������ ũ�� (10��10)

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // === �ٴ� ���� (10ĭ���� �� ����) ===
                if (x % (int)floorSize == 0 && y % (int)floorSize == 0)
                {
                    Vector3 floorPos = new Vector3(x + 5, 0, y + 5);
                    Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);
                }

                // === �� ���� (1��1 ����) ===
                if (map[x, y] == 1)
                {
                    float wallHeight = wallPrefab.transform.localScale.y;
                    Vector3 wallPos = new Vector3(x, wallHeight / 5f, y);
                    Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
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
        return rooms.Where(r => r != playerRoom).ToList();
    }

    // �÷��̾� �� ��ȯ
    public RectInt GetPlayerRoom()
    {
        return playerRoom;
    }

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