// QuickBarPersistence.cs
using System.IO;
using UnityEngine;

public static class QuickBarPersistence
{
    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "quickbar.json");

    public static void Save(QuickBarSave data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
            // Debug.Log($"[QuickBarPersistence] Saved: {FilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuickBarPersistence] Save failed: {e}");
        }
    }

    public static QuickBarSave Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            var json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<QuickBarSave>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuickBarPersistence] Load failed: {e}");
            return null;
        }
    }
}
