using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab; // ��ӵ� ������ ������
    [Range(0f, 100f)] public float dropChance; // ��� Ȯ�� (0~100%)
    public int minAmount = 1;
    public int maxAmount = 1;
}

public class ItemDropManager : MonoBehaviour
{
    [Header("��� ����")]
    public DropItem[] dropTable;      // ����� ������ ���
    public Transform dropPoint;       // ��ӵ� ��ġ (������ �� ��ġ ���)
    public float dropRadius = 1f;     // ���� ��� ���� �ݰ�

    public void DropItems()
    {
        foreach (var drop in dropTable)
        {
            if (drop.itemPrefab == null) continue; // ���� üũ

            // Random.value (0~1) �� 0~100���� ��ȯ
            float randomValue = Random.value * 100f;

            // dropChance�� �������� �� ��ӵǰ�
            if (randomValue <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                for (int i = 0; i < amount; i++)
                {
                    Vector3 dropPos = (dropPoint != null ? dropPoint.position : transform.position)
                                      + Random.insideUnitSphere * dropRadius;
                    dropPos.y = transform.position.y; // TODO: �ʿ�� ����ĳ��Ʈ�� ���� ����

                    Instantiate(drop.itemPrefab, dropPos, Quaternion.identity, transform.parent);
                }
            }
        }
    }
}
