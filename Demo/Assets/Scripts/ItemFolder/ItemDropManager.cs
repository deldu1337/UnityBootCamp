using UnityEngine;

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab;
    [Range(0f, 100f)] public float dropChance = 100f;
    public int minAmount = 1;
    public int maxAmount = 1;
}

public class ItemDropManager : MonoBehaviour
{
    [Header("��� ����")]
    public DropItem[] dropTable;
    public Transform dropPoint;
    public float dropRadius = 1f;

    public void DropItems()
    {
        if (dropTable == null || dropTable.Length == 0)
        {
            Debug.LogWarning("��� ���̺��� ��� �ֽ��ϴ�!");
            return;
        }

        foreach (var drop in dropTable)
        {
            if (drop.itemPrefab == null)
            {
                Debug.LogWarning("DropItem�� itemPrefab�� ��� ����!");
                continue;
            }

            float randomValue = Random.value * 100f;
            Debug.Log($"[{drop.itemPrefab.name}] ��� Ȯ�� üũ: {randomValue} <= {drop.dropChance}");

            if (randomValue > drop.dropChance)
            {
                Debug.Log($"[{drop.itemPrefab.name}] ��� ���� (Ȯ�� �̴�)");
                continue;
            }

            int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
            Debug.Log($"[{drop.itemPrefab.name}] ��� Ȯ��! ����: {amount}");

            for (int i = 0; i < amount; i++)
            {
                Vector3 basePos = dropPoint != null ? dropPoint.position : transform.position;
                Vector3 offset = Random.insideUnitSphere * dropRadius;
                offset.y = 0;
                Vector3 dropPos = basePos + offset;

                GameObject instance = Instantiate(drop.itemPrefab, dropPos, Quaternion.identity, transform.parent);
                Debug.Log($"[{drop.itemPrefab.name}] ���� �Ϸ� at {dropPos}");
            }
        }
    }
}
