using System.Collections;
using UnityEngine;

public class ProjectileSkill : ISkill
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public float Cooldown { get; private set; }
    public float MpCost { get; private set; }
    public float Range { get; private set; }
    public float ImpactDelay { get; private set; }

    private float damage;
    private string animationName;

    public ProjectileSkill(SkillData data)
    {
        Id = data.id;
        Name = data.name;
        Cooldown = data.cooldown;
        MpCost = data.mpCost;
        Range = data.range;
        damage = data.damage;
        ImpactDelay = data.impactDelay;
        animationName = data.animation;
    }

    public bool Execute(GameObject user, PlayerStatsManager stats)
    {
        if (!stats.UseMana(MpCost))
        {
            Debug.LogWarning($"{Name} ��� ����: MP ����");
            return false;
        }

        Animation anim = user.GetComponent<Animation>();
        PlayerAttacks attackComp = user.GetComponent<PlayerAttacks>();
        PlayerMove moveComp = user.GetComponent<PlayerMove>();

        if (attackComp != null)
        {
            attackComp.ForceStopAttack(); // �Ϲ� ���� ��� �ߴ�
            attackComp.isCastingSkill = true; // ��ų �켱 ���
        }

        float animDuration = 0.5f; // �⺻�� (�ִ� ���� ���)
        if (anim && !string.IsNullOrEmpty(animationName))
        {
            anim.CrossFade(animationName, 0.1f);
            AnimationState state = anim[animationName];
            if (state != null)
                animDuration = state.length / Mathf.Max(state.speed, 0.0001f);
        }

        // �̵�/���� ���
        if (attackComp != null) attackComp.isAttacking = true;
        if (moveComp != null) moveComp.SetMovementLocked(true);

        // ����Ʈ Ÿ�̹�: ActiveSkill�� �����ϰ� ������ ���
        float impactDelay = animDuration * 0.5f;
        // ����) �ִ� ���� ������ ���� �ʹٸ� �Ʒ��� ��ü:

        // ����Ʈ ������ AoE ����
        var host = user.GetComponent<MonoBehaviour>();
        if (host != null)
        {
            host.StartCoroutine(ApplyAoEAfterDelay(user.transform, stats, impactDelay));
            host.StartCoroutine(UnlockAfterDelay(attackComp, moveComp, animDuration));
        }
        return true;
    }

    private IEnumerator ApplyAoEAfterDelay(Transform userTf, PlayerStatsManager stats, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ����Ʈ ������ ���� ��ġ�� �������� ���� �� �� Ž��
        Collider[] hits = Physics.OverlapSphere(userTf.position, Range, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            EnemyStatsManager enemy = hit.GetComponent<EnemyStatsManager>();
            if (enemy != null && enemy.CurrentHP > 0)
            {
                float finalDamage = stats.CalculateDamage() * damage;
                enemy.TakeDamage(finalDamage);
                Debug.Log($"{enemy.name}���� {finalDamage} ����! (����)");
            }
        }
    }

    private IEnumerator UnlockAfterDelay(PlayerAttacks attack, PlayerMove move, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (attack != null)
        {
            attack.isCastingSkill = false;  // ��ų ����
            attack.isAttacking = false;
            if (attack.targetEnemy != null && attack.targetEnemy.CurrentHP > 0)
                attack.ChangeState(new AttackingStates());
            else
                attack.ChangeState(new IdleStates());
        }

        if (move != null)
            move.SetMovementLocked(false);
    }
}