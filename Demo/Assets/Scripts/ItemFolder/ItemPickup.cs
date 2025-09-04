using UnityEngine;

// ������ �Ⱦ� ��ũ��Ʈ
public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // ������ ���� (Inspector���� ���� �� �Է� ����)
    [TextArea] public string id;       // ������ ID (DataManager�� ��ϵ� int ID, ���ڿ��� �����)
    public Sprite icon;                 // �κ��丮�� ǥ�õ� ������ �̹���

    [Header("���� ����")]
    public float showDistance = 7f;     // �÷��̾�� �� �Ÿ� �ȿ����� ���� ǥ��

    private bool isMouseOver = false;   // ���콺�� ������ ���� �ִ��� ����
    private Transform player;           // �÷��̾� Transform ����
    private DataManager dataManager;    // ���� �� ������ ������ �Ŵ���
    private PlayerInventory playerInventory; // �÷��̾� �κ��丮 ����

    // �ʱ�ȭ
    private void Start()
    {
        // DataManager �̱��� �ν��Ͻ� �������� �� ������ �ε�
        dataManager = DataManager.GetInstance();
        dataManager.LoadDatas();

        // ������ �÷��̾� ������Ʈ ã�� (Player ���̾� ����)
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

        // PlayerInventory �ڵ� ����
        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
            Debug.LogError("PlayerInventory�� ã�� �� �����ϴ�! Player ������Ʈ�� PlayerInventory ��ũ��Ʈ�� �ٿ��ּ���.");
    }

    private void Update()
    {
        // ���콺 ���� ���� && �÷��̾� ���� ��
        if (isMouseOver && player != null)
        {
            // �÷��̾�� ������ ���� �Ÿ� ���
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= showDistance) // ���� �Ÿ� ���̸�
            {
                // ������ ���� ǥ��
                ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);

                // ��Ŭ�� �� ������ �κ��丮�� �߰�
                if (Input.GetMouseButtonDown(0))
                {
                    // ������ �߰�: ������ ID�� ������ ����
                    playerInventory.AddItemToInventory(int.Parse(id), icon);

                    // ������ ������Ʈ ����
                    Destroy(gameObject);

                    // ���� ����
                    ItemTooltip.Instance.Hide();
                }
            }
            else
            {
                // �Ÿ� ����� ���� ����
                ItemTooltip.Instance.Hide();
            }
        }
    }

    // ���콺 Ŀ���� ������ ���� �ö��� ��
    void OnMouseEnter()
    {
        isMouseOver = true;
        ItemTooltip.Instance.Show(dataManager.dicItemDatas[int.Parse(id)].name);
    }

    // ���콺 Ŀ���� �����ۿ��� ����� ��
    void OnMouseExit()
    {
        isMouseOver = false;
        ItemTooltip.Instance.Hide();
    }
}
