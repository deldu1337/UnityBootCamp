using System;

[Serializable]
public class PlayerData
{
    public string Race; // 추가: 저장/비교용

    // 최종 스탯
    public float MaxHP;
    public float MaxMP;
    public float Atk;
    public float Def;
    public float Dex;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;

    // 현재 상태
    public float CurrentHP;
    public float CurrentMP;

    // 레벨 / 경험치
    public int Level;
    public float Exp;
    public float ExpToNextLevel;
}