using System;

[Serializable]
public class PlayerData
{
    public string Race; // �߰�: ����/�񱳿�

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