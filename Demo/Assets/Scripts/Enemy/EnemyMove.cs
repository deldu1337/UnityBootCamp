using UnityEngine;

/// <summary>
/// �� �̵��� �÷��̾� ����/����, �ִϸ��̼� ����
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(EnemyStatsManager))]
public class EnemyMove : MonoBehaviour
{
    [Header("�̵� ����")]
    [SerializeField] private float baseMoveSpeed = 3f;      // �⺻ �̵� �ӵ�
    [SerializeField] private float baseRotationSpeed = 10f; // �⺻ ȸ�� �ӵ�
    [SerializeField] private float detectRadius = 10f;      // �÷��̾� Ž�� ����

    public Transform TargetPlayer { get; private set; }     // ���� ���

    private TileMapGenerator mapGenerator;
    private Rigidbody rb;
    private Animation anim;
    private EnemyStatsManager stats;
    private Vector3 spawnPosition;

    private int playerLayerMask; // Awake���� �ʱ�ȭ�� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        anim = GetComponent<Animation>();
        stats = GetComponent<EnemyStatsManager>();

        if (!anim) Debug.LogError($"{name}: Animation ������Ʈ�� �����ϴ�!");
        if (!stats) Debug.LogError($"{name}: EnemyStatsManager�� �����ϴ�!");

        mapGenerator = FindAnyObjectByType<TileMapGenerator>();
        if (!mapGenerator) Debug.LogWarning($"{name}: TileMapGenerator�� ã�� ���߽��ϴ�. �� ���� ���� Ž���մϴ�.");

        spawnPosition = transform.position;

        // ���⼭ ���̾� ����ũ �ʱ�ȭ
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
    }

    private void OnEnable()
    {
        PlayerStatsManager.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnPlayerDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        // �÷��̾ �״� ��� Ÿ�� ���� �� ���� FixedUpdate���� ���� ����
        TargetPlayer = null;

        // �̵� �ִ�/���¸� ��� ��ȯ�ϰ� �ʹٸ�(����)
        // Run/Stand�� MoveTowardsTarget�� ó���ϹǷ� ���⼭�� ���� ����
    }

    /// <summary>�� ���� ��ġ ����</summary>
    public void SetSpawnPosition(Vector3 position) => spawnPosition = position;

    private void FixedUpdate()
    {
        DetectPlayer();
        MoveTowardsTarget();
    }

    /// <summary>�÷��̾� Ž��</summary>
    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayerMask);
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var pStats = hit.GetComponent<PlayerStatsManager>();
            if (pStats == null) continue;

            // ���� �÷��̾�� ����
            if (pStats.CurrentHP <= 0f) continue;

            Vector3 playerPos = hit.transform.position;

            // ���� �� �÷��̾� ����
            if (mapGenerator && mapGenerator.GetPlayerRoom().Contains(
                new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.z))))
                continue;

            float dist = Vector3.Distance(transform.position, playerPos);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        TargetPlayer = closest;
    }

    /// <summary>Ÿ�� �Ǵ� ���� ��ġ�� �̵�</summary>
    private void MoveTowardsTarget()
    {
        Vector3 destination = TargetPlayer ? TargetPlayer.position : spawnPosition;
        Vector3 direction = (destination - rb.position);
        direction.y = 0f;

        float distance = direction.magnitude;
        float moveSpeed = baseMoveSpeed + stats.Data.dex;                 // ��ø ��� �̵��ӵ�
        float rotationSpeed = baseRotationSpeed + stats.Data.dex * 0.5f;  // ��ø ��� ȸ���ӵ�

        if (distance > 1f)
        {
            Vector3 moveDir = direction.normalized;
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));

            PlayAnimation("Run (ID 5 variation 0)");
        }
        else
        {
            PlayAnimation("Stand (ID 0 variation 0)");
        }
    }

    /// <summary>�ִϸ��̼� ��� (���� ���̸� �̵�/��� �ִϸ��̼� ���� ����)</summary>
    private void PlayAnimation(string animName)
    {
        if (!anim) return;

        // ���� �ִϸ��̼��� ��� ���̸� �ٸ� �ִϸ��̼� ��� �� ��
        if (anim.IsPlaying("AttackUnarmed (ID 16 variation 0)"))
            return;

        if (!anim.IsPlaying(animName))
            anim.CrossFade(animName, 0.2f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}


