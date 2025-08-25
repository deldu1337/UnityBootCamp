using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float detectRadius = 10f; // �÷��̾� ���� ����
    public Transform TargetPlayer { get; private set; }
    private TileMapGenerator mapGenerator; // �����Ϳ��� �Ҵ�

    private Rigidbody rb;
    private Animator animator;
    private Transform target; // ���� ���

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator ������Ʈ�� Enemy�� �����ϴ�!");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // TileMapGenerator �ڵ� �Ҵ�
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<TileMapGenerator>();
            if (mapGenerator == null)
                Debug.LogError("���� TileMapGenerator�� �����ϴ�!");
        }
    }

    void FixedUpdate()
    {
        // ���� ���� �� �÷��̾� ����
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.GetMask("Player"));
        if (hits.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                Vector3 playerPos = hit.transform.position;

                // �÷��̾� �� ���̸� Ž�� ����
                if (mapGenerator != null && mapGenerator.GetPlayerRoom().Contains(new Vector2Int(
                        Mathf.FloorToInt(playerPos.x),
                        Mathf.FloorToInt(playerPos.z))))
                {
                    continue;
                }

                float dist = Vector3.Distance(transform.position, playerPos);
                if (dist < minDistance)
                {
                    closest = hit.transform;
                    minDistance = dist;
                }
            }

            target = closest; // �÷��̾� �� �ۿ� �ִ� �÷��̾ ����
        }
        else
        {
            target = null; // ���� ���̸� ���� ����
        }


        if (target == null)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 direction = target.position - rb.position;
        direction.y = 0; // ���� ����
        float distance = direction.magnitude;

        if (distance > 2f)
        {
            Vector3 moveDir = direction.normalized;

            // �̵�
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // �̵� �ӵ��� ���� �ִϸ��̼� ��ȯ
            animator.SetFloat("Speed", moveSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    // ����׿� ���� ���� ǥ��
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
