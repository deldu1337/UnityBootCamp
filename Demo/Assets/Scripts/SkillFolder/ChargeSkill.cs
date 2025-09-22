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
//    public float ImpactDelay { get; private set; } // 도착 후 임팩트 연출용(원하면 사용)
//    private float damageMul;
//    private string animationName;

//    // 튜닝 파라미터
//    private const float DashSpeed = 50f;          // 돌진 속도(유닛/초)
//    private const float MinStopPadding = 0.25f;   // 적 앞에 멈출 여유 거리
//    private const float MaxDashTimePerMeter = 0.12f; // 안전 타임아웃(장애물 등)

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

//        // 1) 유효 타겟 확보(기본 타겟 → 마우스 보정)
//        EnemyStatsManager target = attack != null ? attack.targetEnemy : null;
//        if (target == null || target.CurrentHP <= 0)
//            if (attack != null && attack.TryPickEnemyUnderMouse(out var picked)) target = picked;

//        if (target == null || target.CurrentHP <= 0)
//        {
//            Debug.LogWarning($"{Name} 실패: 유효한 타겟 없음");
//            return false;
//        }

//        // 2) 사거리 체크(현재 위치 기준)
//        float dist = Vector3.Distance(user.transform.position, target.transform.position);
//        if (dist > Range)
//        {
//            Debug.LogWarning($"{Name} 실패: 사거리({Range}) 밖 (현재 {dist:F2})");
//            return false;
//        }

//        // 3) MP 차감
//        if (!stats.UseMana(MpCost))
//        {
//            Debug.LogWarning($"{Name} 실패: MP 부족");
//            return false;
//        }

//        // 4) 일반 공격/이동 잠금
//        if (attack != null)
//        {
//            attack.ForceStopAttack();
//            attack.isCastingSkill = true;
//            attack.isAttacking = true;
//        }
//        if (mover != null) mover.SetMovementLocked(true);

//        // 5) 애니메이션(선택)
//        if (anim && !string.IsNullOrEmpty(animationName))
//            anim.CrossFade(animationName, 0.1f);

//        // 6) 돌진 코루틴
//        user.GetComponent<MonoBehaviour>()
//            .StartCoroutine(DashAndHit(user.transform, rb, stats, target, attack, mover));

//        return true;
//    }

//    private IEnumerator DashAndHit(Transform self, Rigidbody rb, PlayerStatsManager stats,
//                                   EnemyStatsManager target,
//                                   PlayerAttacks attack, PlayerMove mover)
//    {
//        // 돌진 목표 지점 계산(적 바로 앞)
//        Vector3 startPos = self.position;
//        Vector3 targetPos = target.transform.position;

//        // 수평 방향
//        Vector3 dir = targetPos - startPos;
//        dir.y = 0f;
//        float dirMag = dir.magnitude;
//        if (dirMag < 0.0001f) dir = self.forward; else dir /= dirMag;

//        // 적/플레이어의 반경 추정 → 너무 붙지 말고 앞에 멈추기
//        float enemyR = EstimateRadius(target.GetComponent<Collider>());
//        float selfR = EstimateRadius(self.GetComponent<Collider>());
//        float stopDist = Mathf.Max(enemyR + selfR + MinStopPadding, 0.25f);

//        Vector3 desired = targetPos - dir * stopDist;

//        // 장애물에 막히면 그 앞에서 멈추도록 레이캐스트
//        if (Physics.Raycast(startPos + Vector3.up * 0.2f, dir, out RaycastHit hit, Vector3.Distance(startPos, desired)))
//        {
//            // 벽/장애물 충돌 시 약간 뒤에서 멈추기
//            desired = hit.point - dir * 0.2f;
//        }

//        // 실제 돌진(순간이동 금지, 빠르게 이동)
//        float totalDist = Vector3.Distance(self.position, desired);
//        float timeout = Mathf.Max(totalDist * MaxDashTimePerMeter, 0.25f);
//        float t = 0f;

//        // 돌진 중에는 계속 타겟을 바라보자
//        Face(self, target.transform.position);

//        while (true)
//        {
//            t += Time.deltaTime;
//            if (t > timeout) break;

//            // 목표 갱신: 타겟이 이동하면 목표점도 조금 따라가도 됨(원하면 고정 유지 가능)
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

//            if (step.magnitude > d) step = to; // 오버슈팅 방지

//            if (rb != null && rb.isKinematic == false)
//                rb.MovePosition(self.position + step);
//            else
//                self.position += step;

//            yield return null;
//        }

//        // 도착 → 데미지 적용
//        if (target != null && target.CurrentHP > 0)
//        {
//            // 치명타 포함 계산
//            bool isCrit;
//            float baseDmg = stats.CalculateDamage(out isCrit);
//            float finalDamage = baseDmg * damageMul;

//            target.TakeDamage(finalDamage);

//            // 데미지 텍스트(치명타=빨강, 평타=흰색)
//            DamageTextManager.Instance.ShowDamage(
//                target.transform,
//                Mathf.RoundToInt(finalDamage),
//                isCrit ? Color.red : Color.white,
//                DamageTextTarget.Enemy
//            );

//            // 임팩트 연출이 필요하면 약간 대기
//            if (ImpactDelay > 0f) yield return new WaitForSeconds(ImpactDelay);
//        }

//        // 해제
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
//        if (!col) return 0.4f; // 대충 기본값
//        var b = col.bounds;
//        // X/Z 중 큰 값의 절반을 반경으로 가정
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

    // 튜닝
    private const float DashSpeed = 50f;            // 이동 속도(유닛/초)
    private const float MinStopPadding = 0.25f;     // 적 앞 여유
    private const float MinTravelTime = 0.06f;      // 너무 가까울 때 방지
    private const float MinAnimSpeed = 0.4f;
    private const float MaxAnimSpeed = 3.0f;
    private const float TimeoutSlack = 0.25f;       // 이동예상시간 + 여유

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

        // 1) 타겟
        EnemyStatsManager target = attack != null ? attack.targetEnemy : null;
        if (target == null || target.CurrentHP <= 0)
            if (attack != null && attack.TryPickEnemyUnderMouse(out var picked)) target = picked;
        if (target == null || target.CurrentHP <= 0) { Debug.LogWarning($"{Name}: 유효 타겟 없음"); return false; }

        // 2) 사거리
        if (Vector3.Distance(user.transform.position, target.transform.position) > Range)
        { Debug.LogWarning($"{Name}: 사거리({Range}) 밖"); return false; }

        // 3) MP
        if (!stats.UseMana(MpCost)) { Debug.LogWarning($"{Name}: MP 부족"); return false; }

        // 4) 상태 잠금
        if (attack != null)
        {
            attack.ForceStopAttack();
            attack.isCastingSkill = true;
            attack.isAttacking = true;
        }
        if (mover != null) mover.SetMovementLocked(true);

        // 5) 도착 목표점/예상시간
        Vector3 desired = ComputeDesiredPoint(user.transform, target);
        float distTotal = Mathf.Max(Vector3.Distance(user.transform.position, desired), 0f);
        float travelSec = Mathf.Max(distTotal / Mathf.Max(DashSpeed, 0.0001f), MinTravelTime);

        // 6) 애니메이션 “한 번만” 재생 + 속도 동기화
        AnimationState state = null;
        if (anim && !string.IsNullOrEmpty(animationName) && anim[animationName] != null)
        {
            state = anim[animationName];
            float clipLen = Mathf.Max(state.length, 0.0001f);
            float playSpeed = Mathf.Clamp(clipLen / travelSec, MinAnimSpeed, MaxAnimSpeed);

            anim.Stop();                 // 다른 재생 중지
            state.wrapMode = WrapMode.ClampForever; // 한 번만 재생 후 정지(마지막 프레임 유지)
            state.time = 0f;
            state.speed = playSpeed;
            anim.Play(animationName);    // CrossFade 대신 즉시 1회 재생
        }

        // 7) 이동 코루틴 시작
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

        // (선택) 임팩트 딜레이
        if (ImpactDelay > 0f) yield return new WaitForSeconds(ImpactDelay);

        // 1회 딜 적용
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

        // 상태 복귀
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

        // 장애물 앞에서 멈추기
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
