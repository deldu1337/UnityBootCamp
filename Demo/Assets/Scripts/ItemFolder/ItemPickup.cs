using UnityEngine;

// ������ �Ⱦ� ��ũ��Ʈ (MVP ���)
public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // ������ ����
    [TextArea] public string id;       // ������ ID (DataManager int ID, ���ڿ��� �����)
    public Sprite icon;                 // �κ��丮�� ǥ�õ� ������ �̹���

    [Header("���� ����")]
    public float showDistance = 7f;     // �÷��̾���� �Ÿ� ����

    private bool isMouseOver = false;   // ���콺 ���� ����
    private Transform player;           // �÷��̾� Transform
    private DataManager dataManager;
    private InventoryPresenter inventoryPresenter; // MVP Presenter

    private void Start()
    {
        // DataManager �̱��� ����
        dataManager = DataManager.Instance;
        dataManager.LoadDatas();

        // ������ �÷��̾� ã�� (Player ���̾� ����)
        int playerLayer = LayerMask.NameToLayer("Player");
        foreach (var obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.layer == playerLayer)
            {
                player = obj.transform;
                break;
            }
        }

        // InventoryPresenter ����
        inventoryPresenter = FindAnyObjectByType<InventoryPresenter>();
        if (inventoryPresenter == null)
            Debug.LogError("InventoryPresenter�� ã�� �� �����ϴ�! ���� InventoryPresenter�� �־�� �մϴ�.");
    }

    private void Update()
    {
        if (!isMouseOver || player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= showDistance)
        {
            // ���� ǥ��
            ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);

            // ��Ŭ�� �� ������ �߰�
            if (Input.GetMouseButtonDown(0))
            {
                if (inventoryPresenter != null)
                {
                    string prefabPath = $"Prefabs/{id}"; // Resources ���� ���� ���
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
