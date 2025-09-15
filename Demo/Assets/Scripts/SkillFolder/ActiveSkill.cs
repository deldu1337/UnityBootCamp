using UnityEngine;

public class ActiveSkill : ISkill
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public float Cooldown { get; private set; }
    public float MpCost { get; private set; }

    private float damage;
    private string animationName;

    public ActiveSkill(SkillData data)
    {
        Id = data.id;
        Name = data.name;
        Cooldown = data.cooldown;
        MpCost = data.mpCost;
        damage = data.damage;
        animationName = data.animation;
    }

    public void Execute(GameObject user, PlayerStatsManager stats)
    {
        if (!stats.UseMana(MpCost))
        {
            Debug.LogWarning($"{Name} 사용 실패: MP 부족");
            return;
        }

        Animation anim = user.GetComponent<Animation>();
        if (anim && !string.IsNullOrEmpty(animationName))
        {
            anim.CrossFade(animationName, 0.1f);

            // 재생 길이 후 Idle 상태 복귀
            AnimationState state = anim[animationName];
            if (state != null)
            {
                user.GetComponent<MonoBehaviour>().StartCoroutine(ReturnToIdle(anim, state.length));
            }
        }

        // 단일 타겟 공격 로직
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            EnemyStatsManager enemy = hit.collider.GetComponent<EnemyStatsManager>();
            if (enemy != null)
            {
                float finalDamage = stats.CalculateDamage() + damage;
                enemy.TakeDamage(finalDamage);
                Debug.Log($"{enemy.name}에게 {finalDamage} 피해!");
            }
        }
    }

    // Idle로 복귀하는 코루틴
    private System.Collections.IEnumerator ReturnToIdle(Animation anim, float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.CrossFade("Stand (ID 0 variation 0)", 0.1f); // Idle 애니메이션 이름
    }

}
