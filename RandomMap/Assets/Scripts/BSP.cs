using System.Collections.Generic;
using UnityEngine;

public class BSP : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public GameObject wallPrefab;
    public Transform Trans;
    [Range(1, 12)] public int maxDepth = 6;

    // �ν����Ϳ��� ���� ����
    [Range(0.1f, 0.9f)] public float minimumDevideRate = 0.45f;
    [Range(0.1f, 0.9f)] public float maximumDivideRate = 0.55f;

    private int[,] map;              // 0 �ٴ�, 1 ��
    private List<RectInt> rooms;
    private RectInt playerRoom;      // �÷��̾� ���� ���� ��
    private int corridorWidth = 3;   // ��� ����

    private void Start()
    {
        if (Trans == null) Trans = transform;
        InitMap();

        // 1) �÷��̾� �� ���� ĳ��
        CarvePlayerRoom();

        // 2) BSP�� �Ϲ� �� ����
        Node root = new Node(new RectInt(0, 0, width, height));
        SplitRoom(root, 0);
        GenerateRooms(root);      // ���� ���� �ٴ����� ĳ���Ͽ� rooms�� ����

        // 3) �÷��̾� ���� ���� ����� �Ϲ� ��� ����
        ConnectPlayerRoomToNearestRoom();

        // 4) BSP Ʈ�� ��� ���� ����
        GenerateLoad(root);

        // 5) ����
        RenderMap();
    }


    void InitMap()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;   // �⺻�� ���� ��

        // �÷��̾� ���� ���� ���� ���� �����Ѵ�
        playerRoom = new RectInt(2, 2, 25, 25);
    }

    void CarvePlayerRoom()
    {
        // �׵θ��� ��, ���δ� �ٴ�
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

        // �÷��̾� ��� ��ġ�� �ʴ� �ĺ� �� �ִ� �Ÿ�
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

        // �÷��̾� �� ��� �߾��� ���Ա��� ���
        Vector2Int doorway = new Vector2Int((playerRoom.xMin + playerRoom.xMax) / 2, playerRoom.yMax - 1);

        // �׵θ� ���� ������ ���� �վ��ش�
        int half = Mathf.Max(1, corridorWidth / 2);
        for (int w = -half; w <= half; w++)
        {
            int dx = doorway.x + w;
            int dy = doorway.y + 1; // �� ������ �� ĭ
            if (Inside(dx, doorway.y)) map[dx, doorway.y] = 0;
            if (Inside(dx, dy)) map[dx, dy] = 0;
        }

        // ��� �� �߽ɱ��� ���� ����
        Vector2Int target = new Vector2Int(Mathf.RoundToInt(nearest.center.x), Mathf.RoundToInt(nearest.center.y));
        DigCorridor(doorway, target);
    }


    // �� ���� ��������
    bool CanSplit(RectInt r)
    {
        const int minLeaf = 10;  // ���� �ּ� ��/����
        return (r.width >= minLeaf * 2) || (r.height >= minLeaf * 2);
    }

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

    // �������� ���� ����� map�� �ٴ����� ĳ��
    RectInt GenerateRooms(Node tree)
    {
        if (tree.leftNode == null && tree.rightNode == null)
        {
            RectInt r = tree.nodeRect;

            // �� ũ�� ���� ����
            int minW = Mathf.Max(4, r.width / 2);
            int minH = Mathf.Max(4, r.height / 2);

            int maxW = Mathf.Max(minW + 1, r.width - 1);
            int maxH = Mathf.Max(minH + 1, r.height - 1);
            if (minW >= maxW || minH >= maxH)
            {
                // �ʹ� ���� ������ �׳� ���� ��ü�� ������ ���
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

            // GenerateRooms(...) ���� ���� ó�� ��������
            if (!tree.roomRect.Overlaps(playerRoom))
            {
                rooms.Add(tree.roomRect);
                for (int x = tree.roomRect.xMin; x < tree.roomRect.xMax; x++)
                    for (int y = tree.roomRect.yMin; y < tree.roomRect.yMax; y++)
                        map[x, y] = 0;
            }

            return tree.roomRect;
        }

        // ���� ���� �ڽ� ó��
        RectInt left = (tree.leftNode != null) ? GenerateRooms(tree.leftNode) : new RectInt();
        RectInt right = (tree.rightNode != null) ? GenerateRooms(tree.rightNode) : new RectInt();
        tree.roomRect = (left.width > 0) ? left : right;
        return tree.roomRect;
    }

    private void GenerateLoad(Node tree)
    {
        // �ڽ��� �ϳ��� ������ ����
        if (tree.leftNode == null || tree.rightNode == null)
            return;

        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;

        // ���� ĳ��
        DigCorridor(leftNodeCenter, rightNodeCenter);

        //���� ������ rightnode�� ���缭 ���� ������ ��������.
        GenerateLoad(tree.leftNode); //�ڽ� ���鵵 Ž��
        GenerateLoad(tree.rightNode);
    }

    void DigCorridor(Vector2Int a, Vector2Int b)
    {
        int half = Mathf.Max(1, corridorWidth / 2);

        // 1�ܰ�: a.y���� ���� �̵�
        int x0 = Mathf.Min(a.x, b.x);
        int x1 = Mathf.Max(a.x, b.x);
        for (int x = x0; x <= x1; x++)
            for (int w = -half; w <= half; w++)
            {
                int yy = a.y + w;
                if (Inside(x, yy)) map[x, yy] = 0;
            }

        // 2�ܰ�: b.x���� ���� �̵�
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

    // �÷��̾� �� ���� ��ȯ
    public RectInt GetPlayerRoom() => playerRoom;
}
