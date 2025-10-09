using System.Collections;
using UnityEngine;
using static DamageTextManager;

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
    [SerializeField] private float damageDelay = 0.3f; // Ÿ�� Ÿ�̹�(��)

    // �߰�: ����/�ڷ�ƾ �ڵ�
    private bool isAttacking = false;
    private Coroutine attackRoutine;

    private void Awake()
    {
        stats = GetComponent<EnemyStatsManager>();
        enemyMove = GetComponent<EnemyMove>();
        anim = GetComponent<Animation>();

        if (!anim) Debug.LogWarning($"{name}: Animation ������Ʈ�� �����ϴ�!");
    }

    private void OnEnable()
    {
        PlayerStatsManager.OnPlayerDied += InterruptAttackOnPlayerDeath;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnPlayerDied -= InterruptAttackOnPlayerDeath;
    }

    private void InterruptAttackOnPlayerDeath()
    {
        // ���� ���̸� ��� �ߴ�
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        // ���� �ִ� ���̸� ����
        if (anim && anim.IsPlaying("AttackUnarmed (ID 16 variation 0)"))
            anim.Stop();

        isAttacking = false;

        // ���� ���� ������ �ʱ�ȭ (��� �簳���� �ʵ��� �ణ�� ���� �� ���� ����)
        lastAttackTime = Time.time;
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
        // ���� �߿� ����� ����
        if (isAttacking) return;
        if (!CanAttackTarget()) return;

        lastAttackTime = Time.time + GetAttackCooldown();
        attackRoutine = StartCoroutine(AttackSequence());
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
        isAttacking = true;

        string attackAnimName = "AttackUnarmed (ID 16 variation 0)";
        float speed = Mathf.Max(stats.Data.As, 0.1f);

        // ���� �ִϸ��̼� ���
        if (anim && anim.GetClip(attackAnimName))
        {
            anim.Stop();
            anim[attackAnimName].speed = speed;
            anim[attackAnimName].wrapMode = WrapMode.Once;
            anim.Play(attackAnimName);
            // Debug.Log($"{name} : {attackAnimName} ���");
        }
        else
        {
            Debug.LogWarning($"{name}: {attackAnimName} Ŭ���� ã�� �� ����!");
        }

        // ����Ʈ���� �� ������ �Ÿ�/��ȿ�� üũ
        float impactWait = damageDelay / speed;
        float t = 0f;
        while (t < impactWait)
        {
            if (OutOfRangeOrInvalid())
            {
                StopAttackAnimIfPlaying(attackAnimName);
                isAttacking = false;
                attackRoutine = null;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        // Ÿ��: ������ ���� ��/��ȿ�� ����
        if (!OutOfRangeOrInvalid())
        {
            float damage = Mathf.Max(stats.Data.atk - targetPlayer.Data.Def, 1f);
            targetPlayer.TakeDamage(damage);

            // �÷��̾� �ǰ� �ؽ�Ʈ
            DamageTextManager.Instance.ShowDamage(
                targetPlayer.transform,
                Mathf.RoundToInt(damage),
                new Color(1f, 0.5f, 0f),
                DamageTextManager.DamageTextTarget.Player);
        }

        // ���� ��� ���ȵ� ��Ż ��� �ߴ�
        float totalDur = GetAnimDuration(attackAnimName, speed);
        float remain = Mathf.Max(0f, totalDur - impactWait);
        t = 0f;
        while (t < remain)
        {
            if (OutOfRangeOrInvalid())
            {
                StopAttackAnimIfPlaying(attackAnimName);
                isAttacking = false;
                attackRoutine = null;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        // ����
        isAttacking = false;
        attackRoutine = null;
    }

    private bool OutOfRangeOrInvalid()
    {
        return targetPlayer == null ||
               targetPlayer.Data.CurrentHP <= 0 ||
               Vector3.Distance(transform.position, targetPlayer.transform.position) > attackRange;
    }

    private void StopAttackAnimIfPlaying(string attackAnimName)
    {
        if (anim && anim.IsPlaying(attackAnimName))
            anim.Stop();
        // EnemyMove�� ���� �ִϰ� �ƴ� ���� Run/Stand�� ����ϹǷ�,
        // ���⼭ Stop()�ϸ� ���� FixedUpdate���� �ڵ����� Run/Stand�� ��ȯ�˴ϴ�.
    }

    private float GetAttackCooldown()
    {
        return 1f / Mathf.Max(stats.Data.As, 0.1f);
    }

    private float GetAnimDuration(string clipName, float speed)
    {
        if (anim && anim.GetClip(clipName))
        {
            var st = anim[clipName];
            return st.length / Mathf.Max(speed, 0.0001f);
        }
        return 0.5f; // �⺻��
    }
}
