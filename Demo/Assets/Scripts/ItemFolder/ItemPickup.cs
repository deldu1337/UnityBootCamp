using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // ������ ����

    [Header("���� ����")]
    public float showDistance = 5f; // �÷��̾�� �� �Ÿ� �ȿ����� ���� ǥ��

    private bool isMouseOver = false;
    private Transform player;

    private void Start()
    {
        // �÷��̾� ���̾� ������� ������ ã��
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
    }

    private void Update()
    {
        if (isMouseOver && player != null)
        {
            // �÷��̾�� ������ ���� �Ÿ� ���
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= showDistance)
                ItemTooltip.Instance.Show(itemInfo);
            else
                ItemTooltip.Instance.Hide();
        }
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        ItemTooltip.Instance.Show(itemInfo);
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        ItemTooltip.Instance.Hide();
    }
}
