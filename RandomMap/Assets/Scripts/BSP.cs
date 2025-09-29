using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public GameObject wallPrefab;
    public Transform Trans;
    [Range(1, 12)] public int maxDepth = 6;

    // 인스펙터에서 조절 가능
    [Range(0.1f, 0.9f)] public float minimumDevideRate = 0.45f;
    [Range(0.1f, 0.9f)] public float maximumDivideRate = 0.55f;

    private int[,] map;              // 0 바닥, 1 벽
    private List<RectInt> rooms;
    private RectInt playerRoom;      // 플레이어 전용 시작 방
    private int corridorWidth = 3;   // 통로 넓이

    private void Start()
    {
        if (Trans == null) Trans = transform;
        InitMap();

        // 1) 플레이어 방 먼저 캐브
        CarvePlayerRoom();

        // 2) BSP로 일반 방 생성
        Node root = new Node(new RectInt(0, 0, width, height));
        SplitRoom(root, 0);
        GenerateRooms(root);      // 리프 방을 바닥으로 캐브하여 rooms에 쌓임

        // 3) 플레이어 방을 가장 가까운 일반 방과 연결
        ConnectPlayerRoomToNearestRoom();

        // 4) BSP 트리 기반 복도 생성
        GenerateLoad(root);

        // 5) 렌더
        RenderMap();
    }


    void InitMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;   // 기본은 전부 벽

        // 플레이어 시작 방을 먼저 고정 생성한다
        playerRoom = new RectInt(2, 2, 25, 25);
    }

    void CarvePlayerRoom()
    {
        // 테두리는 벽, 내부는 바닥
        for (int x = playerRoom.xMin; x < playerRoom.xMax; x++)
            for (int y = playerRoom.yMin; y < playerRoom.yMax; y++)
                map[x, y] = 1;

        for (int x = playerRoom.xMin + 1; x < playerRoom.xMax - 1; x++)
            for (int y = playerRoom.yMin + 1; y < playerRoom.yMax - 1; y++)
                map[x, y] = 0;
    }

    void ConnectPlayerRoomToNearestRoom()
    {
        if (rooms == null || rooms.Count == 0) return;

        Vector2Int playerCenter = new Vector2Int(
            Mathf.RoundToInt(playerRoom.center.x),
            Mathf.RoundToInt(playerRoom.center.y)
        );

        // 플레이어 방과 겹치지 않는 후보 중 최단 거리
        RectInt nearest = default;
        float best = float.MaxValue;
        foreach (var r in rooms)
        {
            if (r.Overlaps(playerRoom)) continue;
            var c = new Vector2Int(Mathf.RoundToInt(r.center.x), Mathf.RoundToInt(r.center.y));
            float d = Vector2Int.Distance(playerCenter, c);
            if (d < best) { best = d; nearest = r; }
        }
        if (nearest.width == 0 || nearest.height == 0) return;

        // 플레이어 방 상단 중앙을 출입구로 사용
        Vector2Int doorway = new Vector2Int((playerRoom.xMin + playerRoom.xMax) / 2, playerRoom.yMax - 1);

        // 테두리 벽에 실제로 문을 뚫어준다
        int half = Mathf.Max(1, corridorWidth / 2);
        for (int w = -half; w <= half; w++)
        {
            int dx = doorway.x + w;
            int dy = doorway.y + 1; // 방 밖으로 한 칸
            if (Inside(dx, doorway.y)) map[dx, doorway.y] = 0;
            if (Inside(dx, dy)) map[dx, dy] = 0;
        }

        // 대상 방 중심까지 복도 생성
        Vector2Int target = new Vector2Int(Mathf.RoundToInt(nearest.center.x), Mathf.RoundToInt(nearest.center.y));
        DigCorridor(doorway, target);
    }


    // 더 분할 가능한지
    bool CanSplit(RectInt r)
    {
        const int minLeaf = 10;  // 리프 최소 폭/높이
        return (r.width >= minLeaf * 2) || (r.height >= minLeaf * 2);
    }

    // 분할만 수행, map은 건드리지 않음
    void SplitRoom(Node tree, int depth)
    {
        if (depth >= maxDepth || !CanSplit(tree.nodeRect))
            return;

        bool splitHoriz = tree.nodeRect.width >= tree.nodeRect.height;
        int axisLen = splitHoriz ? tree.nodeRect.width : tree.nodeRect.height;

        // 분할 구간을 1..axisLen-1로 강제
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

    // 리프에서 방을 만들고 map을 바닥으로 캐브
    RectInt GenerateRooms(Node tree)
    {
        if (tree.leftNode == null && tree.rightNode == null)
        {
            RectInt r = tree.nodeRect;

            // 방 크기 안전 범위
            int minW = Mathf.Max(4, r.width / 2);
            int minH = Mathf.Max(4, r.height / 2);

            int maxW = Mathf.Max(minW + 1, r.width - 1);
            int maxH = Mathf.Max(minH + 1, r.height - 1);
            if (minW >= maxW || minH >= maxH)
            {
                // 너무 작은 리프는 그냥 리프 전체를 방으로 사용
                tree.roomRect = r;
            }
            else
            {
                int roomW = Random.Range(minW, maxW);
                int roomH = Random.Range(minH, maxH);
                int roomX = r.x + Random.Range(1, Mathf.Max(1, r.width - roomW));
                int roomY = r.y + Random.Range(1, Mathf.Max(1, r.height - roomH));
                tree.roomRect = new RectInt(roomX, roomY, roomW, roomH);
            }

            rooms.Add(tree.roomRect);

            // GenerateRooms(...) 내부 리프 처리 마지막에
            if (!tree.roomRect.Overlaps(playerRoom))
            {
                rooms.Add(tree.roomRect);
                for (int x = tree.roomRect.xMin; x < tree.roomRect.xMax; x++)
                    for (int y = tree.roomRect.yMin; y < tree.roomRect.yMax; y++)
                        map[x, y] = 0;
            }

            return tree.roomRect;
        }

        // 내부 노드면 자식 처리
        RectInt left = (tree.leftNode != null) ? GenerateRooms(tree.leftNode) : new RectInt();
        RectInt right = (tree.rightNode != null) ? GenerateRooms(tree.rightNode) : new RectInt();
        tree.roomRect = (left.width > 0) ? left : right;
        return tree.roomRect;
    }

    private void GenerateLoad(Node tree)
    {
        // 자식이 하나라도 없으면 리프
        if (tree.leftNode == null || tree.rightNode == null)
            return;

        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;

        // 복도 캐브
        DigCorridor(leftNodeCenter, rightNodeCenter);

        //가로 기준을 rightnode에 맞춰서 세로 선으로 연결해줌.
        GenerateLoad(tree.leftNode); //자식 노드들도 탐색
        GenerateLoad(tree.rightNode);
    }

    void DigCorridor(Vector2Int a, Vector2Int b)
    {
        int half = Mathf.Max(1, corridorWidth / 2);

        // 1단계: a.y에서 수평 이동
        int x0 = Mathf.Min(a.x, b.x);
        int x1 = Mathf.Max(a.x, b.x);
        for (int x = x0; x <= x1; x++)
            for (int w = -half; w <= half; w++)
            {
                int yy = a.y + w;
                if (Inside(x, yy)) map[x, yy] = 0;
            }

        // 2단계: b.x에서 수직 이동
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


    void RenderMap()
    {
        if (Trans == null) Trans = transform;
        for (int i = Trans.childCount - 1; i >= 0; i--)
            Destroy(Trans.GetChild(i).gameObject);

        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
                if (map[x, y] == 1)
                    Instantiate(wallPrefab, new Vector3(x, 0f, y), Quaternion.identity, Trans);
    }

    public void ReloadMap()
    {
        InitMap();
        CarvePlayerRoom();
        Node root = new Node(new RectInt(0, 0, width, height));
        SplitRoom(root, 0);
        GenerateRooms(root);
        ConnectPlayerRoomToNearestRoom();
        GenerateLoad(root);
        RenderMap();
    }

    // 플레이어 방 영역 반환
    public RectInt GetPlayerRoom() => playerRoom;
}
