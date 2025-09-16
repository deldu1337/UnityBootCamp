using UnityEngine;

public interface ISkill
{
    string Id { get; }
    string Name { get; }
    float Cooldown { get; }
    float MpCost { get; }
    float Range { get; }
    float ImpactDelay { get; }
    void Execute(GameObject user, PlayerStatsManager stats);
}
