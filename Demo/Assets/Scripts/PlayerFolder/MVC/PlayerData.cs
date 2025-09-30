using System;

[Serializable]
public class PlayerData
{
    public string Race; // �߰�: ����/�񱳿�

    // �� Base (����/���������θ� ����)
    public float BaseMaxHP;
    public float BaseMaxMP;
    public float BaseAtk;
    public float BaseDef;
    public float BaseDex;
    public float BaseAttackSpeed;
    public float BaseCritChance;
    public float BaseCritDamage;

    // ���� ����
    public float MaxHP;
    public float MaxMP;
    public float Atk;
    public float Def;
    public float Dex;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;

    // ���� ����
    public float CurrentHP;
    public float CurrentMP;

    // ���� / ����ġ
    public int Level;
    public float Exp;
    public float ExpToNextLevel;
}