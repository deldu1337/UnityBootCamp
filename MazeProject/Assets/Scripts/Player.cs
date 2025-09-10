using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Board;

class Pos
{
    public Pos(int y, int x) { Y = y; X = x; }

    public int Y;
    public int X;
}

public class Player : MonoBehaviour
{
    public int PosY { get; set; }
    public int PosX { get; set; }

    private Board _board;
    private bool _isBoardCreated = false;

    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3,
    }

    int _dir = (int)Dir.Up; // 0

    List<Pos> _points = new List<Pos>();

    public void Initialze(int posY, int posX, Board board)
    {
        PosY = posY;
        PosX = posX;
        _board = board;

        transform.position = new Vector3(posX, 0, -posY);

        _points.Clear();
        _lastIndex = 0;

        AStar();
        //BFS();

        _isBoardCreated = true;
    }

    // ��Ʈ��Ʈ�� ���� ����
    // 1. ������ �̶� �� �Ҵ� / GC �δ��� ����
    // class�� ���� �����̶� new �Ҷ����� ��(heap) �� ��ü�� �����, GC�� �����ؾ� ��
    // struct�� �� �����̶� ����(stack) �̳� �迭 ���ο� �ٷ� ���� ��
    // ��, �켱���� ť���� ��û ���� ��带 Push/Pop �Ҷ� GC Alloc �� �ٰ� ������ ������ �� ����.

    // 2. ũ�Ⱑ ���� ������ �����̱� ������ struct �� ����
    // PQNode�� ��� �ִ� �ʵ尡 � �ȵɰŰ�
    // struct �� �� �׷� �������� ���� ��������� ����
    // .net�� ������ ���̵忡�� ����ü�� 16���� �̳���� Ŭ����(��ü)���� ���ɻ� �����ϳ� ��������.

    struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;

            return F < other.F ? 1 : -1;
        }
    }

    void AStar()
    {
        int[] dY = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
        int[] dX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
        int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };

        // ���� �ű��
        // F = G + H
        // F = �������� (���� ���� ����, ��ο� ���� �޶���)
        // G = ���������� �ش� ��ǥ���� �̵��ϴµ� ��� ���(���� ���� ����, ��ο� ���� �޶���)
        // H = ���������� �󸶳� ������� (�������� ����, ����)
        // [][][][][][][]
        // S       []  []
        // [][][]  []  []
        //              G
        // [][][][][][][]

        //[ ][ ][ ][ ][ ]
        //[ ][ ] * [ ][ ]
        //[ ] *  P  * [ ]
        //[ ][ ] * [ ][ ]
        //[ ][ ][ ][ ][ ]

        // (y, x) �̹� �湮�ߴ��� ���� ( �湮 = closed ����)
        bool[,] closed = new bool[_board.Size, _board.Size]; // CloseList
        // (y, x) ���� ���� �ѹ� �̶� �߰� �ߴ���
        // �߰� X => MaxValue
        // �߰� O => F = G + H
        int[,] open = new int[_board.Size, _board.Size]; // OpenList
        for (int y = 0; y < _board.Size; y++)
        {
            for (int x = 0; x < _board.Size; x++)
            {
                open[y, x] = Int32.MaxValue;
            }
        }

        Pos[,] parent = new Pos[_board.Size, _board.Size];

        // ���¸���Ʈ�� �ִ� ������ �߿���, ���� ���� �ĺ��� ������ �̾ƿ��� ���� �켱���� ť
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // ������ �߰� (���� ����)
        open[PosY, PosX] = /*G = 0*/ /*H =*/ 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX));
        pq.Push(new PQNode()
        {
            F = 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX)),
            G = 0,
            Y = PosY,
            X = PosX
        });
        parent[PosY, PosX] = new Pos(PosY, PosX);

        while (pq.Count > 0)
        {
            // ���� ���� �ĺ� ã��
            PQNode node = pq.Pop();

            // ������ ��ǥ�� ���� ��η� ã�Ƽ�, �� ���� ��η� ���ؼ� �̹� �湮(closed) �� ��� ��ŵ

            // �츮�� ���� PriorityQueue�� DecreaseKey �� ���� �ܼ� Push/Pop ��
            // ���� ť�� ���� Ű�� �������� ����Ű�� ����� �� ���� Ű�� ������ ����� ����
            // �ߺ� �Ǵ� Ű�� ������� �ְ� �װ� �����ϱ� ���� �ڵ�
            if (closed[node.Y, node.X])
                continue;

            // �湮 �Ѵ�
            closed[node.Y, node.X] = true;
            // �������� �����ϸ� �ٷ� ����
            if (node.Y == _board.DestY && node.X == _board.DestX)
                break;

            // �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open) �Ѵ�.
            for (int i = 0; i < dY.Length; i++)
            {
                int nextY = node.Y + dY[i];
                int nextX = node.X + dX[i];

                // ��ȿ���� ����� ��ŵ
                if (nextY < 0 || nextY >= _board.Size || nextX < 0 || nextX >= _board.Size)
                    continue;

                // ������ ���������� ��ŵ(���� ����� ��ŵ)
                if (_board.Tile[nextY, nextX] == TileType.Wall)
                    continue;

                // �̹� �湮������ ��ŵ
                if (closed[nextY, nextX] == true)
                    continue;

                // ��� ���
                int g = node.G + cost[i];
                int h = 10 * (Math.Abs(_board.DestY - nextY) + Math.Abs(_board.DestX - nextX));

                // �ٸ� ��Ͽ��� �� ���� �� �̹� ã������ ��ŵ
                if (open[nextY, nextX] < g + h)
                    continue;

                // ���� ����
                open[nextY, nextX] = g + h;
                // ť�� ����
                pq.Push(new PQNode() { F = g + h, G = g, Y = nextY, X = nextX });
                parent[nextY, nextX] = new Pos(node.Y, node.X);
            }
        }

        CalcPathFromParent(parent);
    }

    void BFS()
    {
        int[] deltaY = new int[] { -1, 0, 1, 0 };
        int[] deltaX = new int[] { 0, -1, 0, 1 };

        bool[,] found = new bool[_board.Size, _board.Size];
        Pos[,] parent = new Pos[_board.Size, _board.Size];

        Queue<Pos> queue = new Queue<Pos>();
        queue.Enqueue(new Pos(PosY, PosX));
        found[PosY, PosX] = true;
        parent[PosY, PosX] = new Pos(PosY, PosX);

        while (queue.Count > 0)
        {
            Pos pos = queue.Dequeue();
            int nowY = pos.Y;
            int nowX = pos.X;

            for (int i = 0; i < 4; i++)
            {
                int nextY = nowY + deltaY[i];
                int nextX = nowX + deltaX[i];

                // ������ �ʰ����� �ʰ� ����
                if (nextY < 0 || nextY >= _board.Size || nextX < 0 || nextX >= _board.Size)
                    continue;

                // üũ �Ϸ��� ���� ���� �ִ� ������
                if (_board.Tile[nextY, nextX] == TileType.Wall)
                    continue;

                // �̹� ã�������� Ȯ��
                if (found[nextY, nextX] == true)
                    continue;

                queue.Enqueue(new Pos(nextY, nextX));
                found[nextY, nextX] = true;
                parent[nextY, nextX] = new Pos(nowY, nowX);
            }
        }

        CalcPathFromParent(parent);
    }

    void CalcPathFromParent(Pos[,] parent)
    {
        int y = _board.DestY;
        int x = _board.DestX;

        while (parent[y, x].Y != y || parent[y, x].X != x)
        {
            // [0] => ������
            // [1] => ������ �θ�
            // ...
            // [�������ε���] => ���� ����
            _points.Add(new Pos(y, x));

            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }

        _points.Add(new Pos(y, x)); // �������� ���� �߰�
        // [0] => ���� ����
        // [1] => ���� ���� ����
        // ...
        // [�������ε���] => ������
        _points.Reverse();
    }


    void RightHand()
    {
        int[] _frontY = new int[] { -1, 0, 1, 0 };
        int[] _frontX = new int[] { 0, -1, 0, 1 };

        int[] _rightY = new int[] { 0, -1, 0, 1 };
        int[] _rightX = new int[] { 1, 0, -1, 0 };

        _points.Add(new Pos(PosY, PosX));

        while (PosY != _board.DestY || PosX != _board.DestX)
        {

            if (_board.Tile[PosY + _rightY[_dir], PosX + _rightX[_dir]] != TileType.Wall)
            {
                _dir = (_dir - 1 + 4) % 4;

                PosY = PosY + _frontY[_dir];
                PosX = PosX + _frontX[_dir];

                _points.Add(new Pos(PosY, PosX));
            }
            else if (_board.Tile[PosY + _frontY[_dir], PosX + _frontX[_dir]] != TileType.Wall)
            {
                PosY = PosY + _frontY[_dir];
                PosX = PosX + _frontX[_dir];

                _points.Add(new Pos(PosY, PosX));
            }
            else
            {
                _dir = (_dir + 1 + 4) % 4;
            }
        }
    }

    private const float MOVE_TICK = 0.1f;
    private float _sumTick = 0;
    private int _lastIndex = 0;

    private void Update()
    {
        if (_lastIndex >= _points.Count)
            return;

        if (_isBoardCreated == false)
            return;

        _sumTick += Time.deltaTime;
        if (_sumTick < MOVE_TICK)
            return;

        _sumTick = 0;

        PosY = _points[_lastIndex].Y;
        PosX = _points[_lastIndex].X;
        _lastIndex++;

        transform.position = new Vector3(PosX, 0, -PosY);
    }

}