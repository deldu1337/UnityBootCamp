using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float detectRadius = 10f; // 플레이어 감지 범위
    public Transform TargetPlayer { get; private set; }
    private TileMapGenerator mapGenerator; // 에디터에서 할당

    private Rigidbody rb;
    private Animator animator;
    private Transform target; // 추적 대상

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator 컴포넌트가 Enemy에 없습니다!");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // TileMapGenerator 자동 할당
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<TileMapGenerator>();
            if (mapGenerator == null)
                Debug.LogError("씬에 TileMapGenerator가 없습니다!");
        }
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

            target = closest; // 플레이어 방 밖에 있는 플레이어만 추적
        }
        else
        {
            target = null; // 범위 밖이면 추적 안함
        }


        if (target == null)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 direction = target.position - rb.position;
        direction.y = 0; // 높이 무시
        float distance = direction.magnitude;

        if (distance > 2f)
        {
            Vector3 moveDir = direction.normalized;

            // 이동
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

            // 이동 속도에 따라 애니메이션 전환
            animator.SetFloat("Speed", moveSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    // 디버그용 감지 범위 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
