using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum Job
{
    전사,
    도적,
    궁수,
    마법사
}

[Serializable]
public class Player
{
    public string name;
    public Job job;
    public int HP;
    public int MP;
    public int ATK;
    public int INT;
    public int DEX;
    [HideInInspector]
    private int gold;
}

[Serializable]
public class Item
{
    public string name;
    public int level;
    public int HP;
    public int MP;
    public int ATK;
    public int INT;
    public int DEX;
    public int sell;
}

public class PlayerStat : MonoBehaviour
{
    public Player player = new Player();

    [Space(10)]
    [Header("Infomation")]
    public int level;
    public int exp;

    [Space(20)]
    [TextArea(5, 10)]
    public string intro;

    [Space(20)]
    public List<Item> item;

    [Space(10)]
    [Header("Setting")]
    [Range(0, 100)] public int sound;
    [Range(0, 100)] public float mouse;
    [Range(0, 100)] public int FieidOfVision;
}
