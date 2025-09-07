using UnityEngine;

// 아이템 픽업 스크립트
public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // 아이템 설명 (Inspector에서 여러 줄 입력 가능)
    [TextArea] public string id;       // 아이템 ID (DataManager에 등록된 int ID, 문자열로 저장됨)
    public Sprite icon;                 // 인벤토리에 표시될 아이콘 이미지

    [Header("툴팁 설정")]
    public float showDistance = 7f;     // 플레이어와 이 거리 안에서만 툴팁 표시

    private bool isMouseOver = false;   // 마우스가 아이템 위에 있는지 여부
    private Transform player;           // 플레이어 Transform 참조
    private DataManager dataManager;    // 게임 내 아이템 데이터 매니저
    private PlayerInventory playerInventory; // 플레이어 인벤토리 참조

    // 초기화
    private void Start()
    {
        // DataManager 싱글톤 인스턴스 가져오기 및 데이터 로드
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // 씬에서 플레이어 오브젝트 찾기 (Player 레이어 기준)
        int playerLayer = LayerMask.NameToLayer("Player");
        GameObject[] players = FindObjectsOfType<GameObject>();
        foreach (var obj in players)
        {
            if (obj.layer == playerLayer)
            {
                player = obj.transform;
                break;
            }
        }

        // PlayerInventory 자동 참조
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
            Debug.LogError("PlayerInventory를 찾을 수 없습니다! Player 오브젝트에 PlayerInventory 스크립트를 붙여주세요.");
    }

    private void Update()
    {
        // 마우스 오버 상태 && 플레이어 존재 시
        if (isMouseOver && player != null)
        {
            // 플레이어와 아이템 사이 거리 계산
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= showDistance) // 지정 거리 안이면
            {
                // 아이템 툴팁 표시
                ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);

                // 좌클릭 시 아이템 인벤토리에 추가
                if (Input.GetMouseButtonDown(0))
                {
                    // 아이템 추가: 아이템 ID와 아이콘 전달
                    playerInventory.AddItemToInventory(int.Parse(id), icon);

                    // 아이템 오브젝트 제거
                    Destroy(gameObject);

                    // 툴팁 숨김
                    ItemTooltip.Instance.Hide();
                }
            }
            else
            {
                // 거리 벗어나면 툴팁 숨김
                ItemTooltip.Instance.Hide();
            }
        }
    }

    // 마우스 커서가 아이템 위에 올라갔을 때
    void OnMouseEnter()
    {
        isMouseOver = true;
        ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);
    }

    // 마우스 커서가 아이템에서 벗어났을 때
    void OnMouseExit()
    {
        isMouseOver = false;
        ItemTooltip.Instance.Hide();
    }
}
