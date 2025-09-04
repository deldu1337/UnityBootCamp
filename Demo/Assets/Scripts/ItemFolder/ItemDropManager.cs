using UnityEngine;

// 드롭 가능한 아이템 정보 클래스
[System.Serializable] // Unity Inspector에서 수정 가능, JSON 등 직렬화 가능
public class DropItem
{
    public GameObject itemPrefab;          // 드롭될 아이템의 프리팹
    [Range(0f, 100f)] public float dropChance; // 드롭 확률 (0~100%)
    public int minAmount = 1;              // 최소 드롭 개수
    public int maxAmount = 1;              // 최대 드롭 개수
}

// 아이템 드롭 관리 클래스
public class ItemDropManager : MonoBehaviour
{
    [Header("드롭 설정")]
    public DropItem[] dropTable;      // 드롭할 아이템 목록 (DropItem 배열)
    public Transform dropPoint;       // 드롭될 위치, 지정 안하면 현재 오브젝트 위치
    public float dropRadius = 1f;     // 랜덤 드롭 범위 반경 (주위 일정 범위 안에서 랜덤 배치)

    // 아이템 드롭 함수: 적 사망 시 호출 가능
    public void DropItems()
    {
        foreach (var drop in dropTable) // 드롭 테이블 순회
        {
            if (drop.itemPrefab == null) continue; // 안전 체크: 프리팹이 없으면 건너뜀

            // 랜덤 값 생성 (0~1) → 0~100 범위
            float randomValue = Random.value * 100f;

            // dropChance 확률에 따라 아이템 드롭 결정
            if (randomValue <= drop.dropChance)
            {
                // 드롭 수량 결정 (minAmount 이상, maxAmount 이하)
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                // 실제 아이템 생성 반복
                for (int i = 0; i < amount; i++)
                {
                    // 드롭 위치 계산: 지정된 dropPoint가 없으면 현재 오브젝트 위치 사용
                    // Random.insideUnitSphere * dropRadius → 반경 내 랜덤 위치
                    Vector3 dropPos = (dropPoint != null ? dropPoint.position : transform.position)
                                      + Random.insideUnitSphere * dropRadius;

                    dropPos.y = transform.position.y; // TODO: 필요시 레이캐스트로 지면 높이 보정

                    // 아이템 프리팹 생성, 부모는 현재 오브젝트 부모로 설정
                    Instantiate(drop.itemPrefab, dropPos, Quaternion.identity, transform.parent);
                }
            }
        }
    }
}
