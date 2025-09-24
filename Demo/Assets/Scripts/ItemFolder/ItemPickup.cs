using UnityEngine;

// ������ �Ⱦ� ��ũ��Ʈ (���� ���� ���� + Ŭ�� ȹ��)
public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo;
    [TextArea] public string id;
    public Sprite icon;

    [Header("����/���� ����")]
    public float showDistance = 7f;
    public bool hideWhenFar = true;

    private Transform player;
    private DataManager dataManager;
    private InventoryPresenter inventoryPresenter;
    private bool isShowing;

    private void Start()
    {
        // ���� ��(EquippedMarker) �Ʒ��� ������ �ڽ� ��Ȱ��ȭ
        if (GetComponentInParent<EquippedMarker>() != null)
        {
            enabled = false;         // Update ��Ȱ��ȭ
            return;
        }

        dataManager = DataManager.Instance;
        dataManager.LoadDatas();

        // Player ã��(���̾�)
        int playerLayer = LayerMask.NameToLayer("Player");
        foreach (var obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.layer == playerLayer) { player = obj.transform; break; }
        }

        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        if (inventoryPresenter == null)
            Debug.LogError("InventoryPresenter�� ã�� �� �����ϴ�!");
    }

    private void Update()
    {
        if (!player || ItemTooltipManager.Instance == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        bool inRange = dist <= showDistance;

        if (inRange && !isShowing)
        {
            isShowing = true;

            var (name, tier) = GetItemInfoSafe();

            ItemTooltipManager.Instance.ShowFor(
                transform,
                name,
                tier,
                onClick: Pickup
            );
        }
        else if (!inRange && isShowing && hideWhenFar)
        {
            isShowing = false;
            ItemTooltipManager.Instance.HideFor(transform);
        }
    }

    private (string, string) GetItemInfoSafe()
    {
        if (!int.TryParse(id, out int parsedId)) return (itemInfo, null);
        if (dataManager?.dicItemDatas == null || !dataManager.dicItemDatas.ContainsKey(parsedId))
            return (itemInfo, null);

        var data = dataManager.dicItemDatas[parsedId];
        return (data.name, data.tier);
    }


    private string GetDisplayNameSafe()
    {
        if (!int.TryParse(id, out int parsedId)) return itemInfo;
        if (dataManager?.dicItemDatas == null || !dataManager.dicItemDatas.ContainsKey(parsedId)) return itemInfo;
        return dataManager.dicItemDatas[parsedId].name;
    }

    private void Pickup()
    {
        if (inventoryPresenter == null) return;

        if (!int.TryParse(id, out int parsedId))
        {
            Debug.LogError($"[ItemPickup] id �Ľ� ����: '{id}'");
            return;
        }
        if (dataManager?.dicItemDatas == null || !dataManager.dicItemDatas.ContainsKey(parsedId))
        {
            Debug.LogError($"[ItemPickup] DataManager�� id={parsedId} ����");
            return;
        }

        string prefabPath = $"Prefabs/{dataManager.dicItemDatas[parsedId].uniqueName}";
        inventoryPresenter.AddItem(parsedId, icon, prefabPath);

        // �� ���� �ݰ� �ڽ� ����
        ItemTooltipManager.Instance?.HideFor(transform);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        // �� ��ȯ/�ı� �ÿ��� ���� ����
        if (ItemTooltipManager.Instance != null)
            ItemTooltipManager.Instance.HideFor(transform);
    }
}
