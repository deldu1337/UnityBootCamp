using System.Collections;
using UnityEngine;

public class ActiveSkill : ISkill
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public float Cooldown { get; private set; }
    public float MpCost { get; private set; }
    public float Range {  get; private set; }
    public float ImpactDelay { get; private set; }

    private float damage;
    private string animationName;

    // 스킬 사거리(필요 시 값 조정)
    //private float castRange = 5f;

    public ActiveSkill(SkillData data)
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

    public void Execute(GameObject user, PlayerStatsManager stats)
    {
        var anim = user.GetComponent<Animation>();
        var attackComp = user.GetComponent<PlayerAttacks>();
        var moveComp = user.GetComponent<PlayerMove>();

        // 1) 유효 타겟 확보 (우선: 지정 타겟, 보조: 마우스 레이캐스트)
        EnemyStatsManager target = attackComp != null ? attackComp.targetEnemy : null;
        if (target == null || target.CurrentHP <= 0)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f))
                target = hit.collider.GetComponent<EnemyStatsManager>();
        }

        // 2) 타겟 최종 검증
        if (target == null || target.CurrentHP <= 0)
        {
            Debug.LogWarning($"{Name} 실패: 유효한 타겟이 없습니다.");
            return;
        }

        // 3) 범위 체크
        float dist = Vector3.Distance(user.transform.position, target.transform.position);
        if (dist > Range)
        {
            Debug.LogWarning($"{Name} 실패: 사거리({Range}m) 밖입니다. (현재 {dist:F2}m)");
            return;
        }

        // 4) MP 차감
        if (!stats.UseMana(MpCost))
        {
            Debug.LogWarning($"{Name} 실패: MP 부족");
            return;
        }

        // ⭐ 5) 시전 시작 시 타겟 방향으로 즉시 회전
        FaceTargetInstant(user.transform, target.transform.position);

        // 6) 애니메이션 재생 및 시전 잠금
        float animDuration = 0.5f; // 기본값(애니 없음 대비)
        if (anim && !string.IsNullOrEmpty(animationName))
        {
            anim.CrossFade(animationName, 0.1f);
            AnimationState state = anim[animationName];
            if (state != null)
                animDuration = state.length / Mathf.Max(0.0001f, state.speed);
        }

        if (attackComp != null) attackComp.isAttacking = true; // 평타 입력 잠금
        if (moveComp != null) moveComp.SetMovementLocked(true); // 이동 잠금

        // 7) 임팩트 타이밍 계산(애니 30% 또는 최대 0.25s)
        //float impactDelay = Mathf.Min(0.25f, animDuration * 0.3f);
        float impactDelay = animDuration * ImpactDelay;

        // ⭐ 임팩트 직전에 한 번 더 타겟을 향해 보정 회전
        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(DealDamageAfterDelay(user.transform, target, stats, impactDelay));

        // 8) 시전 종료 후 잠금 해제 + 상태 복귀
        user.GetComponent<MonoBehaviour>()
            .StartCoroutine(UnlockAfterDelay(attackComp, moveComp, animDuration));
    }

    // 즉시 목표를 바라보게(지면 평면 기준)
    private void FaceTargetInstant(Transform self, Vector3 targetPos)
    {
        Vector3 dir = targetPos - self.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            self.rotation = Quaternion.LookRotation(dir);
    }

    private IEnumerator DealDamageAfterDelay(Transform userTf, EnemyStatsManager target, PlayerStatsManager stats, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ⭐ 임팩트 직전 보정 회전
        if (target != null) FaceTargetInstant(userTf, target.transform.position);

        // 임팩트 시점 재검증(타겟 생존/사거리)
        if (target != null && target.CurrentHP > 0)
        {
            float dist = Vector3.Distance(userTf.position, target.transform.position);
            if (dist <= Range)
            {
                float finalDamage = stats.CalculateDamage() * damage;
                target.TakeDamage(finalDamage);
                Debug.Log($"{target.name}에게 {finalDamage} 피해! (ActiveSkill)");
            }
        }
    }

    private IEnumerator UnlockAfterDelay(PlayerAttacks attack, PlayerMove move, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (attack != null)
        {
            attack.isAttacking = false;
            if (attack.targetEnemy != null && attack.targetEnemy.CurrentHP > 0)
                attack.ChangeState(new AttackingStates()); // 자동공격 재개
            else
                attack.ChangeState(new IdleStates());
        }

        if (move != null)
            move.SetMovementLocked(false);
    }
}
