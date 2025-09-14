using UnityEngine;

[RequireComponent(typeof(Rigidbody))]  // Rigidbody �ʼ�
[RequireComponent(typeof(Animation))]  // Animation �ʼ�
public class EnemyMove : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 5f;             // �̵� �ӵ�
    public float rotationSpeed = 10f;        // ȸ�� �ӵ�
    public float detectRadius = 10f;         // �÷��̾� ���� ����

    public Transform TargetPlayer { get; private set; } // ���� ���

    private TileMapGenerator mapGenerator;
    private Rigidbody rb;
    private Animation animationComponent;     // Animator �� Animation���� ��ü
    private Transform target;
    private Vector3 spawnPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� �����ϴ�!");

        if (mapGenerator == null)
        {
            mapGenerator = FindAnyObjectByType<TileMapGenerator>();
            if (mapGenerator == null)
                Debug.LogError("���� TileMapGenerator�� �����ϴ�!");
        }

        spawnPosition = transform.position;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }

    void FixedUpdate()
    {
        DetectPlayer();
        MoveOrReturnToSpawn();
    }

    /// <summary>�÷��̾� Ž��</summary>
    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.GetMask("Player"));
        if (hits.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                Vector3 playerPos = hit.transform.position;

                // Ư�� �� ���� �÷��̾� ����
                if (mapGenerator != null && mapGenerator.GetPlayerRoom().Contains(
                    new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.z))))
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

            target = closest;
            TargetPlayer = closest;
        }
        else
        {
            target = null;
            TargetPlayer = null;
        }
    }

    /// <summary>�÷��̾ ���� ��ġ�� �̵�</summary>
    private void MoveOrReturnToSpawn()
    {
        Vector3 destination = target != null ? target.position : spawnPosition;
        Vector3 direction = destination - rb.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance > 1f)
        {
            Vector3 moveDir = direction.normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // �̵� �ִϸ��̼� ���
            PlayAnimation("Run (ID 5 variation 0)");
        }
        else
        {
            // ��� �ִϸ��̼� ���
            PlayAnimation("Stand (ID 0 variation 0)");
        }
    }

    /// <summary>�ִϸ��̼� ��� (�ߺ� ����)</summary>
    private void PlayAnimation(string animName)
    {
        if (animationComponent != null && !animationComponent.IsPlaying(animName))
        {
            animationComponent.CrossFade(animName, 0.2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
