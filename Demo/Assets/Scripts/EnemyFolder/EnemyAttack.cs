using UnityEngine;

[RequireComponent(typeof(EnemyMove))] // EnemyMove 컴포넌트가 반드시 필요함
public class EnemyAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackPower = 5f;        // 적 공격력
    public float attackRange = 2f;        // 근접 공격 범위
    public float attackCooldown = 1.5f;   // 공격 간격 (초)

    private float lastAttackTime = 0f;    // 마지막 공격 시간 기록
    private EnemyMove enemyMove;          // 이동 및 추적 관련 컴포넌트
    private PlayerStats targetPlayer;     // 공격 대상 플레이어 정보

    void Awake()
    {
        // EnemyMove 컴포넌트 가져오기
        enemyMove = GetComponent<EnemyMove>();
        if (enemyMove == null)
            Debug.LogError("EnemyMove 컴포넌트가 필요합니다.");
    }

    void Update()
    {
        // EnemyMove가 추적 중인 플레이어가 있는지 확인
        if (enemyMove.TargetPlayer != null)
        {
            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStats>();

            // 플레이어가 존재하고 HP가 0 이상이면 공격 가능
            if (targetPlayer != null && targetPlayer.currentHP > 0)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

                // 공격 범위 내이고 쿨타임이 지났으면 공격 수행
                if (distance <= attackRange && Time.time >= lastAttackTime)
                {
                    PerformAttack();                      // 공격 실행
                    lastAttackTime = Time.time + attackCooldown; // 쿨타임 갱신
                }
            }
        }
    }

    // 실제 공격 처리
    private void PerformAttack()
    {
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(attackPower); // 플레이어 체력 감소
            Debug.Log($"{name} attacked {targetPlayer.name} for {attackPower} damage"); // 로그 출력
        }
    }
}