using System.IO;
using UnityEngine;

public static class PotionQuickBarPersistence
{
    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "potion_quickbar.json");

    public static void Save(PotionQuickBarSave data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PotionQuickBarPersistence] Save failed: {e}");
        }
    }

    public static PotionQuickBarSave Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            var json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<PotionQuickBarSave>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PotionQuickBarPersistence] Load failed: {e}");
            return null;
        }
    }
}
