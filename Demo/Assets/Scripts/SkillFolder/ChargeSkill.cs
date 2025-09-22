//using System.Collections;
//using UnityEngine;
//using static DamageTextManager;

//public class ChargeSkill : ISkill
//{
//    public string Id { get; private set; }
//    public string Name { get; private set; }
//    public float Cooldown { get; private set; }
//    public float MpCost { get; private set; }
//    public float Range { get; private set; }
//    public float ImpactDelay { get; private set; } // ���� �� ����Ʈ �����(���ϸ� ���)
//    private float damageMul;
//    private string animationName;

//    // Ʃ�� �Ķ����
//    private const float DashSpeed = 50f;          // ���� �ӵ�(����/��)
//    private const float MinStopPadding = 0.25f;   // �� �տ� ���� ���� �Ÿ�
//    private const float MaxDashTimePerMeter = 0.12f; // ���� Ÿ�Ӿƿ�(��ֹ� ��)

//    public ChargeSkill(SkillData data)
//    {
//        Id = data.id;
//        Name = data.name;
//        Cooldown = data.cooldown;
//        MpCost = data.mpCost;
//        Range = data.range;
//        ImpactDelay = data.impactDelay;
//        damageMul = data.damage;
//        animationName = data.animation;
//    }

//    public bool Execute(GameObject user, PlayerStatsManager stats)
//    {
//        var anim = user.GetComponent<Animation>();
//        var attack = user.GetComponent<PlayerAttacks>();
//        var mover = user.GetComponent<PlayerMove>();
//        var rb = user.GetComponent<Rigidbody>();

//        // 1) ��ȿ Ÿ�� Ȯ��(�⺻ Ÿ�� �� ���콺 ����)
//        EnemyStatsManager target = attack != null ? attack.targetEnemy : null;
//        if (target == null || target.CurrentHP <= 0)
//            if (attack != null && attack.TryPickEnemyUnderMouse(out var picked)) target = picked;

//        if (target == null || target.CurrentHP <= 0)
//        {
//            Debug.LogWarning($"{Name} ����: ��ȿ�� Ÿ�� ����");
//            return false;
//        }

//        // 2) ��Ÿ� üũ(���� ��ġ ����)
//        float dist = Vector3.Distance(user.transform.position, target.transform.position);
//        if (dist > Range)
//        {
//            Debug.LogWarning($"{Name} ����: ��Ÿ�({Range}) �� (���� {dist:F2})");
//            return false;
//        }

//        // 3) MP ����
//        if (!stats.UseMana(MpCost))
//        {
//            Debug.LogWarning($"{Name} ����: MP ����");
//            return false;
//        }

//        // 4) �Ϲ� ����/�̵� ���
//        if (attack != null)
//        {
//            attack.ForceStopAttack();
//            attack.isCastingSkill = true;
//            attack.isAttacking = true;
//        }
//        if (mover != null) mover.SetMovementLocked(true);

//        // 5) �ִϸ��̼�(����)
//        if (anim && !string.IsNullOrEmpty(animationName))
//            anim.CrossFade(animationName, 0.1f);

//        // 6) ���� �ڷ�ƾ
//        user.GetComponent<MonoBehaviour>()
//            .StartCoroutine(DashAndHit(user.transform, rb, stats, target, attack, mover));

//        return true;
//    }

//    private IEnumerator DashAndHit(Transform self, Rigidbody rb, PlayerStatsManager stats,
//                                   EnemyStatsManager target,
//                                   PlayerAttacks attack, PlayerMove mover)
//    {
//        // ���� ��ǥ ���� ���(�� �ٷ� ��)
//        Vector3 startPos = self.position;
//        Vector3 targetPos = target.transform.position;

//        // ���� ����
//        Vector3 dir = targetPos - startPos;
//        dir.y = 0f;
//        float dirMag = dir.magnitude;
//        if (dirMag < 0.0001f) dir = self.forward; else dir /= dirMag;

//        // ��/�÷��̾��� �ݰ� ���� �� �ʹ� ���� ���� �տ� ���߱�
//        float enemyR = EstimateRadius(target.GetComponent<Collider>());
//        float selfR = EstimateRadius(self.GetComponent<Collider>());
//        float stopDist = Mathf.Max(enemyR + selfR + MinStopPadding, 0.25f);

//        Vector3 desired = targetPos - dir * stopDist;

//        // ��ֹ��� ������ �� �տ��� ���ߵ��� ����ĳ��Ʈ
//        if (Physics.Raycast(startPos + Vector3.up * 0.2f, dir, out RaycastHit hit, Vector3.Distance(startPos, desired)))
//        {
//            // ��/��ֹ� �浹 �� �ణ �ڿ��� ���߱�
//            desired = hit.point - dir * 0.2f;
//        }

//        // ���� ����(�����̵� ����, ������ �̵�)
//        float totalDist = Vector3.Distance(self.position, desired);
//        float timeout = Mathf.Max(totalDist * MaxDashTimePerMeter, 0.25f);
//        float t = 0f;

//        // ���� �߿��� ��� Ÿ���� �ٶ���
//        Face(self, target.transform.position);

//        while (true)
//        {
//            t += Time.deltaTime;
//            if (t > timeout) break;

//            // ��ǥ ����: Ÿ���� �̵��ϸ� ��ǥ���� ���� ���󰡵� ��(���ϸ� ���� ���� ����)
//            if (target != null && target.CurrentHP > 0)
//            {
//                Vector3 curDir = (target.transform.position - self.position);
//                curDir.y = 0f;
//                float mag = curDir.magnitude;
//                if (mag > 0.0001f) curDir /= mag; else curDir = self.forward;

//                float curEnemyR = EstimateRadius(target.GetComponent<Collider>());
//                float curSelfR = EstimateRadius(self.GetComponent<Collider>());
//                float curStop = Mathf.Max(curEnemyR + curSelfR + MinStopPadding, 0.25f);
//                desired = target.transform.position - curDir * curStop;
//                Face(self, target.transform.position);
//            }

//            Vector3 to = desired - self.position; to.y = 0f;
//            float d = to.magnitude;
//            if (d <= 0.05f) break;

//            Vector3 step = to.normalized * DashSpeed * Time.deltaTime;

//            if (step.magnitude > d) step = to; // �������� ����

//            if (rb != null && rb.isKinematic == false)
//                rb.MovePosition(self.position + step);
//            else
//                self.position += step;

//            yield return null;
//        }

//        // ���� �� ������ ����
//        if (target != null && target.CurrentHP > 0)
//        {
//            // ġ��Ÿ ���� ���
//            bool isCrit;
//            float baseDmg = stats.CalculateDamage(out isCrit);
//            float finalDamage = baseDmg * damageMul;

//            target.TakeDamage(finalDamage);

//            // ������ �ؽ�Ʈ(ġ��Ÿ=����, ��Ÿ=���)
//            DamageTextManager.Instance.ShowDamage(
//                target.transform,
//                Mathf.RoundToInt(finalDamage),
//                isCrit ? Color.red : Color.white,
//                DamageTextTarget.Enemy
//            );

//            // ����Ʈ ������ �ʿ��ϸ� �ణ ���
//            if (ImpactDelay > 0f) yield return new WaitForSeconds(ImpactDelay);
//        }

//        // ����
//        if (attack != null)
//        {
//            attack.isCastingSkill = false;
//            attack.isAttacking = false;
//            if (attack.targetEnemy != null && attack.targetEnemy.CurrentHP > 0)
//                attack.ChangeState(new AttackingStates());
//            else
//                attack.ChangeState(new IdleStates());
//        }
//        if (mover != null) mover.SetMovementLocked(false);
//    }

//    private static void Face(Transform self, Vector3 worldTarget)
//    {
//        Vector3 d = worldTarget - self.position; d.y = 0f;
//        if (d.sqrMagnitude > 0.0001f)
//            self.rotation = Quaternion.LookRotation(d);
//    }

//    private static float EstimateRadius(Collider col)
//    {
//        if (!col) return 0.4f; // ���� �⺻��
//        var b = col.bounds;
//        // X/Z �� ū ���� ������ �ݰ����� ����
//        return Mathf.Max(b.extents.x, b.extents.z);
//    }
//}
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
    public float ImpactDelay { get; private set; }
    private float damageMul;
    private string animationName;

    // Ʃ��
    private const float DashSpeed = 50f;            // �̵� �ӵ�(����/��)
    private const float MinStopPadding = 0.25f;     // �� �� ����
    private const float MinTravelTime = 0.06f;      // �ʹ� ����� �� ����
    private const float MinAnimSpeed = 0.4f;
    private const float MaxAnimSpeed = 3.0f;
    private const float TimeoutSlack = 0.25f;       // �̵�����ð� + ����

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

        // 1) Ÿ��
        EnemyStatsManager target = attack != null ? attack.targetEnemy : null;
        if (target == null || target.CurrentHP <= 0)
            if (attack != null && attack.TryPickEnemyUnderMouse(out var picked)) target = picked;
        if (target == null || target.CurrentHP <= 0) { Debug.LogWarning($"{Name}: ��ȿ Ÿ�� ����"); return false; }

        // 2) ��Ÿ�
        if (Vector3.Distance(user.transform.position, target.transform.position) > Range)
        { Debug.LogWarning($"{Name}: ��Ÿ�({Range}) ��"); return false; }

        // 3) MP
        if (!stats.UseMana(MpCost)) { Debug.LogWarning($"{Name}: MP ����"); return false; }

        // 4) ���� ���
        if (attack != null)
        {
            attack.ForceStopAttack();
            attack.isCastingSkill = true;
            attack.isAttacking = true;
        }
        if (mover != null) mover.SetMovementLocked(true);

        // 5) ���� ��ǥ��/����ð�
        Vector3 desired = ComputeDesiredPoint(user.transform, target);
        float distTotal = Mathf.Max(Vector3.Distance(user.transform.position, desired), 0f);
        float travelSec = Mathf.Max(distTotal / Mathf.Max(DashSpeed, 0.0001f), MinTravelTime);

        // 6) �ִϸ��̼� ���� ������ ��� + �ӵ� ����ȭ
        AnimationState state = null;
        if (anim && !string.IsNullOrEmpty(animationName) && anim[animationName] != null)
        {
            state = anim[animationName];
            float clipLen = Mathf.Max(state.length, 0.0001f);
            float playSpeed = Mathf.Clamp(clipLen / travelSec, MinAnimSpeed, MaxAnimSpeed);

            anim.Stop();                 // �ٸ� ��� ����
            state.wrapMode = WrapMode.ClampForever; // �� ���� ��� �� ����(������ ������ ����)
            state.time = 0f;
            state.speed = playSpeed;
            anim.Play(animationName);    // CrossFade ��� ��� 1ȸ ���
        }

        // 7) �̵� �ڷ�ƾ ����
        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(DashAndHit(user.transform, rb, stats, target, attack, mover, desired, travelSec));

        return true;
    }

    private IEnumerator DashAndHit(Transform self, Rigidbody rb, PlayerStatsManager stats,
                                   EnemyStatsManager target, PlayerAttacks attack,
                                   PlayerMove mover, Vector3 desired, float travelSec)
    {
        Face(self, target.transform.position);

        float timeout = travelSec + TimeoutSlack;
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime;
            if (t > timeout) break;

            if (target != null && target.CurrentHP > 0)
            {
                desired = ComputeDesiredPoint(self, target);
                Face(self, target.transform.position);
            }

            Vector3 to = desired - self.position; to.y = 0f;
            float d = to.magnitude;
            if (d <= 0.05f) break;

            Vector3 step = to.normalized * DashSpeed * Time.deltaTime;
            if (step.magnitude > d) step = to;

            if (rb != null && !rb.isKinematic) rb.MovePosition(self.position + step);
            else self.position += step;

            yield return null;
        }

        // (����) ����Ʈ ������
        if (ImpactDelay > 0f) yield return new WaitForSeconds(ImpactDelay);

        // 1ȸ �� ����
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
        }

        // ���� ����
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

    private Vector3 ComputeDesiredPoint(Transform self, EnemyStatsManager target)
    {
        Vector3 startPos = self.position;
        Vector3 targetPos = target.transform.position;

        Vector3 dir = targetPos - startPos; dir.y = 0f;
        float mag = dir.magnitude;
        dir = (mag > 0.0001f) ? (dir / mag) : self.forward;

        float enemyR = EstimateRadius(target.GetComponent<Collider>());
        float selfR = EstimateRadius(self.GetComponent<Collider>());
        float stop = Mathf.Max(enemyR + selfR + MinStopPadding, 0.25f);

        Vector3 desired = targetPos - dir * stop;

        // ��ֹ� �տ��� ���߱�
        if (Physics.Raycast(startPos + Vector3.up * 0.2f, dir, out RaycastHit hit, Vector3.Distance(startPos, desired)))
            desired = hit.point - dir * 0.2f;

        return desired;
    }

    private static void Face(Transform self, Vector3 worldTarget)
    {
        Vector3 d = worldTarget - self.position; d.y = 0f;
        if (d.sqrMagnitude > 0.0001f)
            self.rotation = Quaternion.LookRotation(d);
    }

    private static float EstimateRadius(Collider col)
    {
        if (!col) return 0.4f;
        var b = col.bounds;
        return Mathf.Max(b.extents.x, b.extents.z);
    }
}
