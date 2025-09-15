//using UnityEngine;

//[RequireComponent(typeof(EnemyMove))]
//public class EnemyAttack : MonoBehaviour
//{
//    private EnemyStatsManager stats;
//    private EnemyMove enemyMove;
//    private PlayerStatsManager targetPlayer;

//    private float lastAttackTime;

//    [Header("공격 범위 설정")]
//    public float attackRange = 2f;

//    void Awake()
//    {
//        stats = GetComponent<EnemyStatsManager>();
//        enemyMove = GetComponent<EnemyMove>();
//    }

//    void Update()
//    {
//        if (enemyMove.TargetPlayer != null)
//        {
//            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStatsManager>();
//            if (targetPlayer != null && targetPlayer.Data.CurrentHP > 0)
//            {
//                float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
//                if (distance <= attackRange && Time.time >= lastAttackTime)
//                {
//                    PerformAttack();
//                    lastAttackTime = Time.time + 1f / stats.Data.As; // 공격속도 반영
//                }
//            }
//        }
//    }

//    private void PerformAttack()
//    {
//        if (targetPlayer != null)
//        {
//            targetPlayer.TakeDamage(stats.Data.atk);
//            Debug.Log($"{stats.Data.name}이 {targetPlayer.name}을 공격 ({stats.Data.atk} 데미지)");
//        }
//    }
//}
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMove))]
[RequireComponent(typeof(EnemyStatsManager))]
public class EnemyAttack : MonoBehaviour
{
    private EnemyStatsManager stats;
    private EnemyMove enemyMove;
    private PlayerStatsManager targetPlayer;
    private Animation anim;
    private float lastAttackTime;

    [Header("공격 설정")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damageDelay = 0.3f;

    private void Awake()
    {
        stats = GetComponent<EnemyStatsManager>();
        enemyMove = GetComponent<EnemyMove>();
        anim = GetComponent<Animation>();

        if (!anim) Debug.LogWarning($"{name}: Animation 컴포넌트가 없습니다!");
    }

    private void Update()
    {
        UpdateTarget();
        TryAttack();
    }

    private void UpdateTarget()
    {
        if (enemyMove?.TargetPlayer == null)
        {
            targetPlayer = null;
            return;
        }

        if (targetPlayer == null || targetPlayer.gameObject != enemyMove.TargetPlayer)
            targetPlayer = enemyMove.TargetPlayer.GetComponent<PlayerStatsManager>();
    }

    private void TryAttack()
    {
        if (!CanAttackTarget()) return;

        lastAttackTime = Time.time + GetAttackCooldown();
        StartCoroutine(AttackSequence());
    }

    private bool CanAttackTarget()
    {
        if (!targetPlayer || targetPlayer.Data.CurrentHP <= 0)
            return false;

        float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);
        return distance <= attackRange && Time.time >= lastAttackTime;
    }

    private IEnumerator AttackSequence()
    {
        string attackAnimName = "AttackUnarmed (ID 16 variation 0)";
        // 공격 애니메이션 강제 재생
        if (anim && anim.GetClip(attackAnimName))
        {
            anim.Stop();
            anim[attackAnimName].speed = stats.Data.As;
            anim.Play(attackAnimName);
            Debug.Log($"{name} : {attackAnimName} 재생");
        }
        else
        {
            Debug.LogWarning($"{name}: {attackAnimName} 클립을 찾을 수 없음!");
            Debug.Log(attackAnimName);
        }

        // 타격 타이밍까지 대기
        yield return new WaitForSeconds(damageDelay / Mathf.Max(stats.Data.As, 0.1f));

        // 타격
        if (targetPlayer && targetPlayer.Data.CurrentHP > 0)
        {
            float damage = Mathf.Max(stats.Data.atk - targetPlayer.Data.Def, 1f);
            targetPlayer.TakeDamage(damage);
            Debug.Log($"{stats.Data.name}이 {targetPlayer.name}에게 {damage} 데미지!");
        }
    }

    private float GetAttackCooldown()
    {
        return 1f / Mathf.Max(stats.Data.As, 0.1f);
    }
}


