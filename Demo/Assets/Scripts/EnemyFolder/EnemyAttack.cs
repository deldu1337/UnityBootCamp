//using UnityEngine;

//[RequireComponent(typeof(EnemyMove))]
//public class EnemyAttack : MonoBehaviour
//{
//    private EnemyStatsManager stats;
//    private EnemyMove enemyMove;
//    private PlayerStatsManager targetPlayer;

//    private float lastAttackTime;

//    [Header("���� ���� ����")]
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
//                    lastAttackTime = Time.time + 1f / stats.Data.As; // ���ݼӵ� �ݿ�
//                }
//            }
//        }
//    }

//    private void PerformAttack()
//    {
//        if (targetPlayer != null)
//        {
//            targetPlayer.TakeDamage(stats.Data.atk);
//            Debug.Log($"{stats.Data.name}�� {targetPlayer.name}�� ���� ({stats.Data.atk} ������)");
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

    [Header("���� ����")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damageDelay = 0.3f;

    private void Awake()
    {
        stats = GetComponent<EnemyStatsManager>();
        enemyMove = GetComponent<EnemyMove>();
        anim = GetComponent<Animation>();

        if (!anim) Debug.LogWarning($"{name}: Animation ������Ʈ�� �����ϴ�!");
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
        // ���� �ִϸ��̼� ���� ���
        if (anim && anim.GetClip(attackAnimName))
        {
            anim.Stop();
            anim[attackAnimName].speed = stats.Data.As;
            anim.Play(attackAnimName);
            Debug.Log($"{name} : {attackAnimName} ���");
        }
        else
        {
            Debug.LogWarning($"{name}: {attackAnimName} Ŭ���� ã�� �� ����!");
            Debug.Log(attackAnimName);
        }

        // Ÿ�� Ÿ�ֱ̹��� ���
        yield return new WaitForSeconds(damageDelay / Mathf.Max(stats.Data.As, 0.1f));

        // Ÿ��
        if (targetPlayer && targetPlayer.Data.CurrentHP > 0)
        {
            float damage = Mathf.Max(stats.Data.atk - targetPlayer.Data.Def, 1f);
            targetPlayer.TakeDamage(damage);
            Debug.Log($"{stats.Data.name}�� {targetPlayer.name}���� {damage} ������!");
        }
    }

    private float GetAttackCooldown()
    {
        return 1f / Mathf.Max(stats.Data.As, 0.1f);
    }
}


