using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class CustomTools : Editor
{
    [MenuItem("UnityStudy/Add User Gem")]
    public static void AddUserGem()
    {
        long gem = long.Parse(PlayerPrefs.GetString("Gem"));
        gem += 100;

        PlayerPrefs.SetString("Gem", gem.ToString());
        PlayerPrefs.Save();
    }

    [MenuItem("UnityStudy/Add User Gold")]
    public static void AddUserGold()
    {
        long gold = long.Parse(PlayerPrefs.GetString("Gold"));
        gold += 1000;

        PlayerPrefs.SetString("Gold", gold.ToString());
        PlayerPrefs.Save();
    }
}
#endif
