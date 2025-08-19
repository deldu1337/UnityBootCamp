using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public Text skill;
    public static bool checkSkill;
    void Start()
    {
        skill.text = "<color=red>Nuclear Enable</color>";
        skill.enabled = false;
        checkSkill = false;
    }

    void Update()
    {
        if (!checkSkill)
        {
            skill.enabled = false;
        }
        else
        {
            skill.enabled = true;
        }
    }
}
