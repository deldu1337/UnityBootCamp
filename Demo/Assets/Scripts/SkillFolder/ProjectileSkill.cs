using UnityEngine;

public class ProjectileSkill : ISkill
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public float Cooldown { get; private set; }
    public float MpCost { get; private set; }

    private float damage;
    private string animationName;

    public ProjectileSkill(SkillData data)
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
            Debug.LogWarning($"{Name} ��� ����: MP ����");
            return;
        }

        Animation anim = user.GetComponent<Animation>();
        if (anim && !string.IsNullOrEmpty(animationName))
            anim.CrossFade(animationName, 0.1f);

        // �ֺ� �� Ž�� (�ݰ� 5f)
        Collider[] hits = Physics.OverlapSphere(user.transform.position, 5f, LayerMask.GetMask("Enemy"));
        foreach (var hit in hits)
        {
            EnemyStatsManager enemy = hit.GetComponent<EnemyStatsManager>();
            if (enemy != null)
            {
                float finalDamage = stats.CalculateDamage() + damage;
                enemy.TakeDamage(finalDamage);
                Debug.Log($"{enemy.name}���� {finalDamage} ����! (����)");
            }
        }
    }
}
