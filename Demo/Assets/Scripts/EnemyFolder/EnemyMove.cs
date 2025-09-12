using UnityEngine;

[RequireComponent(typeof(Rigidbody))]  // Rigidbody 컴포넌트 필요
[RequireComponent(typeof(Animator))]   // Animator 컴포넌트 필요
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;             // 이동 속도
    public float rotationSpeed = 10f;        // 회전 속도
    public float detectRadius = 10f;         // 플레이어 감지 범위
    public Transform TargetPlayer { get; private set; } // 외부에서 접근 가능한 현재 추적 대상

    private TileMapGenerator mapGenerator;   // 맵 정보를 가져오기 위한 TileMapGenerator
    private Rigidbody rb;                    // 물리 기반 이동을 위한 Rigidbody
    private Transform target;                // 실제 추적 대상 플레이어
    private Vector3 spawnPosition;           // 적이 스폰된 원래 위치

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 물리 충돌 시 회전 제한

        // TileMapGenerator 자동 할당
        if (mapGenerator == null)
        {
            mapGenerator = FindAnyObjectByType<TileMapGenerator>();
            if (mapGenerator == null)
                Debug.LogError("씬에 TileMapGenerator가 없습니다!");
        }

        // 초기 스폰 위치 저장
        spawnPosition = transform.position;
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position; // 스폰 위치를 외부에서 변경 가능
    }

    void FixedUpdate()
    {
        // detectRadius 내 플레이어 감지
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.GetMask("Player"));
        if (hits.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                Vector3 playerPos = hit.transform.position;

                // 플레이어가 특정 방 안에 있으면 추적 무시
                if (mapGenerator != null && mapGenerator.GetPlayerRoom().Contains(new Vector2Int(
                        Mathf.FloorToInt(playerPos.x),
                        Mathf.FloorToInt(playerPos.z))))
                {
                    continue;
                }

                // 가장 가까운 플레이어 선택
                float dist = Vector3.Distance(transform.position, playerPos);
                if (dist < minDistance)
                {
                    closest = hit.transform;
                    minDistance = dist;
                }
            }

            target = closest; // 추적 대상 지정
        }
        else
        {
            target = null; // 플레이어 없으면 스폰 위치로 이동
        }

        // 이동 목적지 결정
        Vector3 destination = target != null ? target.position : spawnPosition;

        Vector3 direction = destination - rb.position;
        direction.y = 0; // y축 무시
        float distance = direction.magnitude;

        if (distance > 1f) // 최소 이동 거리 체크
        {
            Vector3 moveDir = direction.normalized;

            // Rigidbody를 이용한 이동
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // 부드러운 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    // 에디터에서 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}