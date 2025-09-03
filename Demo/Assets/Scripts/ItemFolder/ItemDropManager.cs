using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab; // 드롭될 아이템 프리팹
    [Range(0f, 100f)] public float dropChance; // 드롭 확률 (0~100%)
    public int minAmount = 1;
    public int maxAmount = 1;
}

public class ItemDropManager : MonoBehaviour
{
    [Header("드롭 설정")]
    public DropItem[] dropTable;      // 드롭할 아이템 목록
    public Transform dropPoint;       // 드롭될 위치 (없으면 적 위치 사용)
    public float dropRadius = 1f;     // 랜덤 드롭 범위 반경

    public void DropItems()
    {
        foreach (var drop in dropTable)
        {
            if (drop.itemPrefab == null) continue; // 안전 체크

            // Random.value (0~1) → 0~100으로 변환
            float randomValue = Random.value * 100f;

            // dropChance가 높을수록 잘 드롭되게
            if (randomValue <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                for (int i = 0; i < amount; i++)
                {
                    Vector3 dropPos = (dropPoint != null ? dropPoint.position : transform.position)
                                      + Random.insideUnitSphere * dropRadius;
                    dropPos.y = transform.position.y; // TODO: 필요시 레이캐스트로 지면 보정

                    Instantiate(drop.itemPrefab, dropPos, Quaternion.identity, transform.parent);
                }
            }
        }
    }
}
