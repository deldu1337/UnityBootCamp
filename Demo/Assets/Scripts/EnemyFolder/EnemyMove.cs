using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float detectRadius = 10f; // 플레이어 감지 범위
    public Transform TargetPlayer { get; private set; }
    private TileMapGenerator mapGenerator;

    private Rigidbody rb;
    private Transform target; // 추적 대상
    private Vector3 spawnPosition; // 원래 스폰된 위치

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // TileMapGenerator 자동 할당
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<TileMapGenerator>();
            if (mapGenerator == null)
                Debug.LogError("씬에 TileMapGenerator가 없습니다!");
        }

        // 초기 스폰 위치 저장
        spawnPosition = transform.position;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
    }

    void FixedUpdate()
    {
        // 일정 범위 내 플레이어 감지
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.GetMask("Player"));
        if (hits.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                Vector3 playerPos = hit.transform.position;

                // 플레이어 방 안이면 탐지 무시
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

            target = closest;
        }
        else
        {
            target = null; // 플레이어가 없으면 스폰 위치로
        }

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
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
