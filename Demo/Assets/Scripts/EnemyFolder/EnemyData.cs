using System;

[Serializable]
public class EnemyData
{
    public string id;
    public string name;
    public float hp;
    public float atk;
    public float def;
    public float dex;
    public float As;
    public float exp; // �� óġ �� �ִ� ����ġ
}

[Serializable]
public class EnemyDatabase
{
    public EnemyData[] enemies;
}