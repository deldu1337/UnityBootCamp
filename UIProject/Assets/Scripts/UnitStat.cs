using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

enum Jobs
{
    None,
    Warrior,
    Rogue,
    Wizard
}

public class UnitStat : MonoBehaviour
{
    public string job;

    public Text STR_text;
    public Text DEX_text;
    public Text INT_text;

    private int STR;
    private int DEX;
    private int INT;
    private string[] jobs;
    [SerializeField] private UpgradeUI upgradeUI;

    public void SetStat()
    {
        jobs = upgradeUI.icon_text.text.Split("+");
        job = jobs[0];
        if (job == Jobs.Warrior.ToString())
        {
            STR = 6;
            DEX = 3;
            INT = 0;
        }
        else if (job == Jobs.Rogue.ToString())
        {
            STR = 3;
            DEX = 6;
            INT = 0;
        }
        else if (job == Jobs.Wizard.ToString())
        {
            STR = 0;
            DEX = 0;
            INT = 9;
        }

        STR_text.text = $"STR: {STR}";
        DEX_text.text = $"DEX: {DEX}";
        INT_text.text = $"INT: {INT}";
    }

    public void UpgradeStat()
    {
        if (job == Jobs.Warrior.ToString())
        {
            STR += 2;
            DEX += 1;
        }
        else if (job == Jobs.Rogue.ToString())
        {
            STR += 1;
            DEX += 2;
        }
        else if (job == Jobs.Wizard.ToString())
        {
            INT += 3;
        }
        STR_text.text = $"STR: {STR}";
        DEX_text.text = $"DEX: {DEX}";
        INT_text.text = $"INT: {INT}";
    }
}
