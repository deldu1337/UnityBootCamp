using UnityEngine;

// ��� ������ ������ ���� Ŭ����
[System.Serializable] // Unity Inspector���� ���� ����, JSON �� ����ȭ ����
public class DropItem
{
    public GameObject itemPrefab;          // ��ӵ� �������� ������
    [Range(0f, 100f)] public float dropChance; // ��� Ȯ�� (0~100%)
    public int minAmount = 1;              // �ּ� ��� ����
    public int maxAmount = 1;              // �ִ� ��� ����
}

// ������ ��� ���� Ŭ����
public class ItemDropManager : MonoBehaviour
{
    [Header("��� ����")]
    public DropItem[] dropTable;      // ����� ������ ��� (DropItem �迭)
    public Transform dropPoint;       // ��ӵ� ��ġ, ���� ���ϸ� ���� ������Ʈ ��ġ
    public float dropRadius = 1f;     // ���� ��� ���� �ݰ� (���� ���� ���� �ȿ��� ���� ��ġ)

    // ������ ��� �Լ�: �� ��� �� ȣ�� ����
    public void DropItems()
    {
        foreach (var drop in dropTable) // ��� ���̺� ��ȸ
        {
            if (drop.itemPrefab == null) continue; // ���� üũ: �������� ������ �ǳʶ�

            // ���� �� ���� (0~1) �� 0~100 ����
            float randomValue = Random.value * 100f;

            // dropChance Ȯ���� ���� ������ ��� ����
            if (randomValue <= drop.dropChance)
            {
                // ��� ���� ���� (minAmount �̻�, maxAmount ����)
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                // ���� ������ ���� �ݺ�
                for (int i = 0; i < amount; i++)
                {
                    // ��� ��ġ ���: ������ dropPoint�� ������ ���� ������Ʈ ��ġ ���
                    // Random.insideUnitSphere * dropRadius �� �ݰ� �� ���� ��ġ
                    Vector3 dropPos = (dropPoint != null ? dropPoint.position : transform.position)
                                      + Random.insideUnitSphere * dropRadius;

                    dropPos.y = transform.position.y; // TODO: �ʿ�� ����ĳ��Ʈ�� ���� ���� ����

                    // ������ ������ ����, �θ�� ���� ������Ʈ �θ�� ����
                    Instantiate(drop.itemPrefab, dropPos, Quaternion.identity, transform.parent);
                }
            }
        }
    }
}
