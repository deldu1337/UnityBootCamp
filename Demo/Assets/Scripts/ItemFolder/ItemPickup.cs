using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [TextArea] public string itemInfo; // 아이템 설명

    [Header("툴팁 설정")]
    public float showDistance = 5f; // 플레이어와 이 거리 안에서만 툴팁 표시

    private bool isMouseOver = false;
    private Transform player;

    private void Start()
    {
        // 플레이어 레이어 기반으로 씬에서 찾기
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
            // 플레이어와 아이템 사이 거리 계산
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
