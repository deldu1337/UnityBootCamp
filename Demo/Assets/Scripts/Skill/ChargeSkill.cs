using System.Collections;
using UnityEngine;
using static DamageTextManager;

public class ChargeSkill : ISkill
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public float Cooldown { get; private set; }
    public float MpCost { get; private set; }
    public float Range { get; private set; }
    public float ImpactDelay { get; private set; } // ���� �� ����Ʈ �����(���ϸ� ���)
    private float damageMul;
    private string animationName;

    // Ʃ�� �Ķ����
    private const float DashSpeed = 70f;             // ���� �ӵ�(����/��)
    private const float MinStopPadding = 0.25f;      // �� �տ� ���� ���� �Ÿ�
    private const float MaxDashTimePerMeter = 0.12f; // ���� Ÿ�Ӿƿ�(��ֹ� ��)
    private const float fallbackHitWindup = 0.1f;   // �ִ� ���̸� �� �� ���� �� ���

    public ChargeSkill(SkillData data)
    {
        Id = data.id;
        Name = data.name;
        Cooldown = data.cooldown;
        MpCost = data.mpCost;
        Range = data.range;
        ImpactDelay = data.impactDelay;
        damageMul = data.damage;
        animationName = data.animation;
    }

    public bool Execute(GameObject user, PlayerStatsManager stats)
    {
        var anim = user.GetComponent<Animation>();
        var attack = user.GetComponent<PlayerAttacks>();
        var mover = user.GetComponent<PlayerMove>();
        var rb = user.GetComponent<Rigidbody>();

        // 1) Ÿ�� Ȯ��
        EnemyStatsManager target = attack != null ? attack.targetEnemy : null;
        if (target == null || target.CurrentHP <= 0)
            if (attack != null && attack.TryPickEnemyUnderMouse(out var picked)) target = picked;

        if (target == null || target.CurrentHP <= 0)
        {
            Debug.LogWarning($"{Name} ����: ��ȿ�� Ÿ�� ����");
            return false;
        }

        // 2) ��Ÿ� üũ
        float dist = Vector3.Distance(user.transform.position, target.transform.position);
        if (dist > Range)
        {
            Debug.LogWarning($"{Name} ����: ��Ÿ�({Range}) �� (���� {dist:F2})");
            return false;
        }

        // 2.5) ���� �� ��� ��ֹ� üũ
        // �� �տ� �� ��ǥ ����(desired) �̸� ����ؼ�, �� �������� ��ü ĳ��Ʈ�� ���� Ȯ��
        Vector3 startPos = user.transform.position;
        Vector3 targetPos = target.transform.position;

        Vector3 dir = targetPos - startPos; dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = user.transform.forward; else dir.Normalize();

        float enemyR = EstimateRadius(target.GetComponent<Collider>());
        float selfR = EstimateRadius(user.GetComponent<Collider>());
        float stopDist = Mathf.Max(enemyR + selfR + MinStopPadding, 0.25f);
        Vector3 desired = targetPos - dir * stopDist;

        // �� ���̾ �ִٸ� ���⿡ ���� (������ Default�ε� ����)
        int wallMask = LayerMask.GetMask("Wall", "Obstacle"); // ������Ʈ�� �°�
        if (PathBlocked(startPos, desired, selfR * 0.9f, wallMask))
        {
            Debug.LogWarning($"{Name} ����: ���濡 ��ֹ��� ���� ���� �Ұ�");
            return false;
        }

        // 3) MP ���� (��ֹ� ������� ����)
        if (!stats.UseMana(MpCost))
        {
            Debug.LogWarning($"{Name} ����: MP ����");
            return false;
        }

        // 4) ��/���� ����
        if (attack != null)
        {
            attack.ForceStopAttack();
            attack.isCastingSkill = true;
            attack.isAttacking = true;
        }
        if (mover != null) mover.SetMovementLocked(true);

        // 5) �ִϸ��̼��� ���� �� ���

        // 6) �ڷ�ƾ ����
        MonoBehaviour runner = (MonoBehaviour)attack ?? (MonoBehaviour)mover ?? user.GetComponent<MonoBehaviour>();
        if (runner == null)
        {
            Debug.LogError($"{Name}: �ڷ�ƾ ���� ��ü ����");
            if (attack != null) { attack.isCastingSkill = false; attack.isAttacking = false; }
            if (mover != null) mover.SetMovementLocked(false);
            return false;
        }

        runner.StartCoroutine(DashAndHit(user.transform, rb, stats, target, attack, mover, anim));
        return true;
    }

    private static bool PathBlocked(Vector3 from, Vector3 to, float radius, int layerMask)
    {
        Vector3 start = from + Vector3.up * 0.2f;   // �ٴ� ƨ�� ����
        Vector3 dir = to - from; dir.y = 0f;
        float dist = dir.magnitude;
        if (dist < 0.001f) return false; // ���ڸ��� ���� �ƴ�
        dir /= dist;

        // ���Ǿ�ĳ��Ʈ�� ��� �� �浹 üũ
        // ��Ʈ�� ������ ����
        return Physics.SphereCast(start, Mathf.Max(0.05f, radius), dir, out _, dist, layerMask, QueryTriggerInteraction.Ignore);
    }



    private IEnumerator DashAndHit(Transform self, Rigidbody rb, PlayerStatsManager stats,
                                   EnemyStatsManager target, PlayerAttacks attack, PlayerMove mover,
                                   Animation anim)
    {
        // ���� ��ǥ ���� ���(�� �ٷ� ��)
        Vector3 startPos = self.position;
        Vector3 targetPos = target.transform.position;

        // ���� ����
        Vector3 dir = targetPos - startPos;
        dir.y = 0f;
        float dirMag = dir.magnitude;
        if (dirMag < 0.0001f) dir = self.forward; else dir /= dirMag;

        // ��/�÷��̾��� �ݰ� ���� �� �ʹ� ���� ���� �տ� ���߱�
        float enemyR = EstimateRadius(target.GetComponent<Collider>());
        float selfR = EstimateRadius(self.GetComponent<Collider>());
        float stopDist = Mathf.Max(enemyR + selfR + MinStopPadding, 0.25f);

        Vector3 desired = targetPos - dir * stopDist;

        // ��ֹ��� ������ �� �տ��� ���ߵ��� ����ĳ��Ʈ
        if (Physics.Raycast(startPos + Vector3.up * 0.2f, dir, out RaycastHit hit, Vector3.Distance(startPos, desired)))
        {
            // ��/��ֹ� �浹 �� �ణ �ڿ��� ���߱�
            desired = hit.point - dir * 0.2f;
        }

        // ===== ���� ���� =====
        float totalDist = Vector3.Distance(self.position, desired);
        float timeout = Mathf.Max(totalDist * MaxDashTimePerMeter, 0.25f);
        float t = 0f;

        // ��ǥ ���� ���� �÷���/�Ӱ谪
        bool lockFinalApproach = false;          // ������ ���������� desired ����
        const float lockDistance = 0.35f;        // �� �Ÿ� �̳��� ������ desired ����
        const float snapEpsilon = 0.12f;       // ����� �����ٸ� �ٷ� ����

        Face(self, target.transform.position);

        while (true)
        {
            t += Time.deltaTime;
            if (t > timeout) break;

            // 1) ������ ���� �������� desired�� Ÿ�ٿ� ���� ����
            if (!lockFinalApproach && target != null && target.CurrentHP > 0)
            {
                Vector3 curDir = (target.transform.position - self.position);
                curDir.y = 0f;
                float mag = curDir.magnitude;

                // �ʹ� ������ ���� forward ���
                if (mag > 0.0001f) curDir /= mag; else curDir = self.forward;

                float curEnemyR = EstimateRadius(target.GetComponent<Collider>());
                float curSelfR = EstimateRadius(self.GetComponent<Collider>());
                float curStop = Mathf.Max(curEnemyR + curSelfR + MinStopPadding, 0.25f);

                Vector3 newDesired = target.transform.position - curDir * curStop;
                desired = newDesired;

                // ������ ���� ����: desired�� �� �̻� �������� ����(������ ����)
                float distToDesiredNow = Vector3.Distance(self.position, desired);
                if (distToDesiredNow <= lockDistance)
                    lockFinalApproach = true;

                Face(self, target.transform.position);
            }

            // 2) �̵�
            Vector3 to = desired - self.position; to.y = 0f;
            float d = to.magnitude;

            // ����� ������ �ٷ� �����ؼ� ����
            if (d <= snapEpsilon)
            {
                // rb ��� ���̸� MovePosition���� ����
                if (rb != null && !rb.isKinematic) rb.MovePosition(desired);
                else self.position = desired;
                break;
            }

            Vector3 step = to.normalized * DashSpeed * Time.deltaTime;
            if (step.magnitude > d) step = to; // �������� ����

            if (rb != null && !rb.isKinematic)
                rb.MovePosition(self.position + step);
            else
                self.position += step;

            yield return null;
        }

        // ���� ���� �ܼ� ����(�ɼ�)
        if (rb != null && !rb.isKinematic) rb.linearVelocity = Vector3.zero;


        // ===== ���� ���� =====
        // 1) ���� �� �ִϸ��̼� ���� & ������� ���
        if (anim && !string.IsNullOrEmpty(animationName))
        {
            anim.CrossFade(animationName, 0.1f);

            float waitTime = fallbackHitWindup;
            // Ŭ�� ���� �� �� ������ �װ͸�ŭ ���
            //if (anim[animationName] != null && anim[animationName].length > 0f)
            //    waitTime = anim[animationName].length;

            // ��� �߿��� Ÿ�� �ٶ󺸱� ����(����)
            float elapsed = 0f;
            while (elapsed < waitTime)
            {
                if (target != null && target.CurrentHP > 0)
                    Face(self, target.transform.position);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        // 2) �ִ� ���� �� ������ ����
        if (target != null && target.CurrentHP > 0)
        {
            bool isCrit;
            float baseDmg = stats.CalculateDamage(out isCrit);
            float finalDamage = baseDmg * damageMul;

            target.TakeDamage(finalDamage);

            DamageTextManager.Instance.ShowDamage(
                target.transform,
                Mathf.RoundToInt(finalDamage),
                isCrit ? Color.red : Color.white,
                DamageTextTarget.Enemy
            );

            if (ImpactDelay > 0f)
                yield return new WaitForSeconds(ImpactDelay);
        }

        // 3) ����/�� ����
        if (attack != null)
        {
            attack.isCastingSkill = false;
            attack.isAttacking = false;
            if (attack.targetEnemy != null && attack.targetEnemy.CurrentHP > 0)
                attack.ChangeState(new AttackingStates());
            else
                attack.ChangeState(new IdleStates());
        }
        if (mover != null) mover.SetMovementLocked(false);
    }

    private static void Face(Transform self, Vector3 worldTarget)
    {
        Vector3 d = worldTarget - self.position; d.y = 0f;
        if (d.sqrMagnitude > 0.0001f)
            self.rotation = Quaternion.LookRotation(d);
    }

    private static float EstimateRadius(Collider col)
    {
        if (!col) return 0.4f; // ���� �⺻��
        var b = col.bounds;
        // X/Z �� ū ���� ������ �ݰ����� ����
        return Mathf.Max(b.extents.x, b.extents.z);
    }
}
