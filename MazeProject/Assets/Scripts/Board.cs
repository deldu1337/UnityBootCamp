using UnityEngine;

public enum TileType
{
    Empty,
    Wall,
    Goal
}

public class Board : MonoBehaviour
{
    public TileType[,] Tile;
    public int Size { get; set; }

    public int DestY { get; set; }
    public int DestX { get; set; }

    private Material emptyMat;
    private Material wallMat;
    private Material goalMat;

    public void Initialze()
    {
        DestY = Size - 2;
        DestX = Size - 2;

        emptyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        emptyMat.color = Color.gray;

        wallMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        wallMat.color = Color.white;

        goalMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        goalMat.color = Color.green;

        Tile = new TileType[Size, Size];
        Size = Size;

        GenerateBySideWinder();

        Camera.main.transform.position = new Vector3(Size / 2, Size, -Size / 2);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.black;
    }

    private void GenerateBySideWinder()
    {
        // 일단 길 다 막음
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (x % 2 == 0 || y % 2 == 0)
                    Tile[y, x] = TileType.Wall;
                else if (x == DestX && y == DestY)
                    Tile[y, x] = TileType.Goal;
                else
                    Tile[y, x] = TileType.Empty;
            }
        }

        // 랜덤으로 우측 혹은 아래로 길을 뚫음
        for (int y = 0; y < Size; y++)
        {
            int count = 1;
            for (int x = 0; x < Size; x++)
            {
                // 아까 벽으로 막은 녀석은 건너뜀
                if (x % 2 == 0 || y % 2 == 0)
                    continue;

                // 가장 오른쪽 끝 이고 가장 아래쪽 끝 인 애는 건너뜀
                if (y == Size - 2 && x == Size - 2)
                    continue;

                // 가장 오른쪽 끝이면 무조건 벽 풀고 길로
                if (y == Size - 2)
                {
                    Tile[y, x + 1] = TileType.Empty;
                    continue;
                }

                // 가장 아래쪽 끝이면 무조건 벽 풀고 길로
                if (x == Size - 2)
                {
                    Tile[y + 1, x] = TileType.Empty;
                    continue;
                }

                // 결국 x, y 좌표가 모두 홀수인 애들을 대상으로
                // 0 과 1 중 랜덤 뽑음
                // 0 이면 오른쪽 길뚫고 카운트 1증가
                if (Random.Range(0, 2) == 0)
                {
                    Tile[y, x + 1] = TileType.Empty;
                    count++;
                }
                // 1 이면 아래쪽 길 뚫는데 여태까지 오른쪽으로 뚫었던 X 중 한개를 골라서 뚫음
                else
                {
                    //           x - 라는건 지금 진행이 왼쪽에서 오른쪽으로 뚫고 있었기 때문
                    //                                      * 2 해준 이유는 짝수좌표 애들이 있기 때문에 2를 곱해야 함
                    //               즉 오른쪽으로 3번 뚫은 상태라면 0, 1, 2 중에 하나 나올거고
                    //                                            만약 2가 나오면 곱하기 2해서 x - 4 즉 맨 왼쪽에서 아래로 길이 남
                    Tile[y + 1, x - Random.Range(0, count) * 2] = TileType.Empty;
                    // 그리고 카운트 를 초기화 시켜줌
                    count = 1;
                }
            }
        }
    }

    public void Spawn()
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(x, 0, -y);
                go.transform.parent = transform;

                if (Tile[y, x] == TileType.Empty || Tile[y, x] == TileType.Goal)
                    go.transform.localScale = new Vector3(1f, 0.1f, 1f);
                else
                    go.transform.localScale = new Vector3(1f, 2f, 1f);

                go.GetComponent<MeshRenderer>().sharedMaterial = GetTileColor(Tile[y, x]);
            }
        }
    }

    private Material GetTileColor(TileType type)
    {
        switch (type)
        {
            case TileType.Empty:
                return emptyMat;
            case TileType.Wall:
                return wallMat;
            case TileType.Goal:
                return goalMat;
        }

        return wallMat;
    }
}
