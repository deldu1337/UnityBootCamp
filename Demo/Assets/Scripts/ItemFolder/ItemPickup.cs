using UnityEngine;

// 아이템 픽업 스크립트 (MVP 기반)
public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // 아이템 설명
    [TextArea] public string id;       // 아이템 ID (DataManager int ID, 문자열로 저장됨)
    public Sprite icon;                 // 인벤토리에 표시될 아이콘 이미지

    [Header("툴팁 설정")]
    public float showDistance = 7f;     // 플레이어와의 거리 제한

    private bool isMouseOver = false;   // 마우스 오버 여부
    private Transform player;           // 플레이어 Transform
    private DataManager dataManager;
    private InventoryPresenter inventoryPresenter; // MVP Presenter

    private void Start()
    {
        // DataManager 싱글톤 참조
        dataManager = DataManager.Instance;
        dataManager.LoadDatas();

        // 씬에서 플레이어 찾기 (Player 레이어 기준)
        int playerLayer = LayerMask.NameToLayer("Player");
        foreach (var obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.layer == playerLayer)
            {
                player = obj.transform;
                break;
            }
        }

        // InventoryPresenter 참조
        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        if (inventoryPresenter == null)
            Debug.LogError("InventoryPresenter를 찾을 수 없습니다! 씬에 InventoryPresenter가 있어야 합니다.");
    }

    private void Update()
    {
        if (!isMouseOver || player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= showDistance)
        {
            // 툴팁 표시
            ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);

            // 좌클릭 시 아이템 추가
            if (Input.GetMouseButtonDown(0))
            {
                if (inventoryPresenter != null)
                {
                    string prefabPath = $"Prefabs/{id}"; // Resources 폴더 기준 경로
                    inventoryPresenter.AddItem(int.Parse(id), icon, prefabPath);
                }

                Destroy(gameObject);
                ItemTooltip.Instance.Hide();
            }
        }
        else
        {
            ItemTooltip.Instance.Hide();
        }
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
        if (dataManager != null)
            ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        ItemTooltip.Instance.Hide();
    }
}
