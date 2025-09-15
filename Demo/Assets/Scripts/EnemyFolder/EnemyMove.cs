//using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]  // Rigidbody 필수
//[RequireComponent(typeof(Animation))]  // Animation 필수
//public class EnemyMove : MonoBehaviour
//{
//    [Header("이동 설정")]
//    public float moveSpeed = 5f;             // 이동 속도
//    public float rotationSpeed = 10f;        // 회전 속도
//    public float detectRadius = 10f;         // 플레이어 감지 범위

//    public Transform TargetPlayer { get; private set; } // 추적 대상

//    private TileMapGenerator mapGenerator;
//    private Rigidbody rb;
//    private Animation animationComponent;     // Animator → Animation으로 교체
//    private Transform target;
//    private Vector3 spawnPosition;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

//        animationComponent = GetComponent<Animation>();
//        if (animationComponent == null)
//            Debug.LogError("Animation 컴포넌트가 없습니다!");

//        if (mapGenerator == null)
//        {
//            mapGenerator = FindAnyObjectByType<TileMapGenerator>();
//            if (mapGenerator == null)
//                Debug.LogError("씬에 TileMapGenerator가 없습니다!");
//        }

//        spawnPosition = transform.position;
//    }

//    public void SetSpawnPosition(Vector3 position)
//    {
//        spawnPosition = position;
//    }

//    void FixedUpdate()
//    {
//        DetectPlayer();
//        MoveOrReturnToSpawn();
//    }

//    /// <summary>플레이어 탐지</summary>
//    private void DetectPlayer()
//    {
//        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, LayerMask.GetMask("Player"));
//        if (hits.Length > 0)
//        {
//            Transform closest = null;
//            float minDistance = float.MaxValue;

//            foreach (var hit in hits)
//            {
//                Vector3 playerPos = hit.transform.position;

//                // 특정 방 내부 플레이어 무시
//                if (mapGenerator != null && mapGenerator.GetPlayerRoom().Contains(
//                    new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.z))))
//                {
//                    continue;
//                }

//                float dist = Vector3.Distance(transform.position, playerPos);
//                if (dist < minDistance)
//                {
//                    closest = hit.transform;
//                    minDistance = dist;
//                }
//            }

//            target = closest;
//            TargetPlayer = closest;
//        }
//        else
//        {
//            target = null;
//            TargetPlayer = null;
//        }
//    }

//    /// <summary>플레이어나 스폰 위치로 이동</summary>
//    private void MoveOrReturnToSpawn()
//    {
//        Vector3 destination = target != null ? target.position : spawnPosition;
//        Vector3 direction = destination - rb.position;
//        direction.y = 0;

//        float distance = direction.magnitude;

//        if (distance > 1f)
//        {
//            Vector3 moveDir = direction.normalized;
//            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

//            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
//            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

//            // 이동 애니메이션 재생
//            PlayAnimation("Run (ID 5 variation 0)");
//        }
//        else
//        {
//            // 대기 애니메이션 재생
//            PlayAnimation("Stand (ID 0 variation 0)");
//        }
//    }

//    /// <summary>애니메이션 재생 (중복 방지)</summary>
//    private void PlayAnimation(string animName)
//    {
//        if (animationComponent != null && !animationComponent.IsPlaying(animName))
//        {
//            animationComponent.CrossFade(animName, 0.2f);
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, detectRadius);
//    }
//}
using UnityEngine;

/// <summary>
/// 적 이동과 플레이어 추적/복귀, 애니메이션 제어
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(EnemyStatsManager))]
public class EnemyMove : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float baseMoveSpeed = 3f;      // 기본 이동 속도
    [SerializeField] private float baseRotationSpeed = 10f; // 기본 회전 속도
    [SerializeField] private float detectRadius = 10f;      // 플레이어 탐지 범위

    public Transform TargetPlayer { get; private set; }     // 추적 대상

    private TileMapGenerator mapGenerator;
    private Rigidbody rb;
    private Animation anim;
    private EnemyStatsManager stats;
    private Vector3 spawnPosition;

    private int playerLayerMask; // Awake에서 초기화할 변수

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        anim = GetComponent<Animation>();
        stats = GetComponent<EnemyStatsManager>();

        if (!anim) Debug.LogError($"{name}: Animation 컴포넌트가 없습니다!");
        if (!stats) Debug.LogError($"{name}: EnemyStatsManager가 없습니다!");

        mapGenerator = FindAnyObjectByType<TileMapGenerator>();
        if (!mapGenerator) Debug.LogWarning($"{name}: TileMapGenerator를 찾지 못했습니다. 방 제약 없이 탐지합니다.");

        spawnPosition = transform.position;

        // 여기서 레이어 마스크 초기화
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
    }

    /// <summary>적 스폰 위치 설정</summary>
    public void SetSpawnPosition(Vector3 position) => spawnPosition = position;

    private void FixedUpdate()
    {
        DetectPlayer();
        MoveTowardsTarget();
    }

    /// <summary>플레이어 탐지</summary>
    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayerMask);
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector3 playerPos = hit.transform.position;

            // 같은 방 플레이어 제외
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

    /// <summary>타겟 또는 스폰 위치로 이동</summary>
    private void MoveTowardsTarget()
    {
        Vector3 destination = TargetPlayer ? TargetPlayer.position : spawnPosition;
        Vector3 direction = (destination - rb.position);
        direction.y = 0f;

        float distance = direction.magnitude;
        float moveSpeed = baseMoveSpeed + stats.Data.dex;                 // 민첩 기반 이동속도
        float rotationSpeed = baseRotationSpeed + stats.Data.dex * 0.5f;  // 민첩 기반 회전속도

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

    /// <summary>애니메이션 재생 (공격 중이면 이동/대기 애니메이션 덮지 않음)</summary>
    private void PlayAnimation(string animName)
    {
        if (!anim) return;

        // 공격 애니메이션이 재생 중이면 다른 애니메이션 재생 안 함
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


