using UnityEngine;

[RequireComponent(typeof(Rigidbody))]  // Rigidbody ������Ʈ �ʿ�
[RequireComponent(typeof(Animator))]   // Animator ������Ʈ �ʿ�
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;             // �̵� �ӵ�
    public float rotationSpeed = 10f;        // ȸ�� �ӵ�
    public float detectRadius = 10f;         // �÷��̾� ���� ����
    public Transform TargetPlayer { get; private set; } // �ܺο��� ���� ������ ���� ���� ���

    private TileMapGenerator mapGenerator;   // �� ������ �������� ���� TileMapGenerator
    private Rigidbody rb;                    // ���� ��� �̵��� ���� Rigidbody
    private Transform target;                // ���� ���� ��� �÷��̾�
    private Vector3 spawnPosition;           // ���� ������ ���� ��ġ

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // ���� �浹 �� ȸ�� ����

        // TileMapGenerator �ڵ� �Ҵ�
        if (mapGenerator == null)
        {
            mapGenerator = FindAnyObjectByType<TileMapGenerator>();
            if (mapGenerator == null)
                Debug.LogError("���� TileMapGenerator�� �����ϴ�!");
        }

        // �ʱ� ���� ��ġ ����
        spawnPosition = transform.position;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position; // ���� ��ġ�� �ܺο��� ���� ����
    }

    void FixedUpdate()
    {
        // detectRadius �� �÷��̾� ����
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.GetMask("Player"));
        if (hits.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                Vector3 playerPos = hit.transform.position;

                // �÷��̾ Ư�� �� �ȿ� ������ ���� ����
                if (mapGenerator != null && mapGenerator.GetPlayerRoom().Contains(new Vector2Int(
                        Mathf.FloorToInt(playerPos.x),
                        Mathf.FloorToInt(playerPos.z))))
                {
                    continue;
                }

                // ���� ����� �÷��̾� ����
                float dist = Vector3.Distance(transform.position, playerPos);
                if (dist < minDistance)
                {
                    closest = hit.transform;
                    minDistance = dist;
                }
            }

            target = closest; // ���� ��� ����
        }
        else
        {
            target = null; // �÷��̾� ������ ���� ��ġ�� �̵�
        }

        // �̵� ������ ����
        Vector3 destination = target != null ? target.position : spawnPosition;

        Vector3 direction = destination - rb.position;
        direction.y = 0; // y�� ����
        float distance = direction.magnitude;

        if (distance > 1f) // �ּ� �̵� �Ÿ� üũ
        {
            Vector3 moveDir = direction.normalized;

            // Rigidbody�� �̿��� �̵�
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // �ε巯�� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    // �����Ϳ��� ���� ���� �ð�ȭ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}