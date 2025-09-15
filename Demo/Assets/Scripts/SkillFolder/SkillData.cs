[System.Serializable]
public class SkillData
{
    public string id;
    public string name;
    public float cooldown;
    public float damage;
    public float mpCost;
    public string type;
    public string animation;
}

[System.Serializable]
public class AllSkillData
{
    public SkillData[] warrior;
    public SkillData[] mage;
    public SkillData[] rogue;
}
