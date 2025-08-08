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

    public int STR;
    public int DEX;
    public int INT;
}
