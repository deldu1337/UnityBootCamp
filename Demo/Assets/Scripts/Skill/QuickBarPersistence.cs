//// QuickBarPersistence.cs
//using System.IO;
//using UnityEngine;

//public static class QuickBarPersistence
//{
//    private static string FilePath =>
//        Path.Combine(Application.persistentDataPath, "quickbar.json");

//    public static void Save(QuickBarSave data)
//    {
//        try
//        {
//            var json = JsonUtility.ToJson(data, true);
//            File.WriteAllText(FilePath, json);
//            // Debug.Log($"[QuickBarPersistence] Saved: {FilePath}");
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError($"[QuickBarPersistence] Save failed: {e}");
//        }
//    }

//    public static QuickBarSave Load()
//    {
//        try
//        {
//            if (!File.Exists(FilePath)) return null;
//            var json = File.ReadAllText(FilePath);
//            return JsonUtility.FromJson<QuickBarSave>(json);
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError($"[QuickBarPersistence] Load failed: {e}");
//            return null;
//        }
//    }
//}
// QuickBarPersistence.cs
using System.IO;
using UnityEngine;

public static class QuickBarPersistence
{
    private const string LegacyFileName = "quickbar.json";

    private static string PathOf(string fileName)
        => Path.Combine(Application.persistentDataPath, fileName);

    private static string FileNameFor(string race)
    {
        if (string.IsNullOrEmpty(race)) race = "humanmale";
        // ��: quickbar_humanmale.json
        return $"quickbar_{race}.json";
    }

    // ===== ������ ���� =====
    public static void SaveForRace(string race, QuickBarSave data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(PathOf(FileNameFor(race)), json);
#if UNITY_EDITOR
            Debug.Log($"[QuickBarPersistence] Saved ({race}): {PathOf(FileNameFor(race))}");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuickBarPersistence] SaveForRace failed: {e}");
        }
    }

    // ===== ������ �ε� (������ ���Žÿ��� 1ȸ ���̱׷��̼�) =====
    public static QuickBarSave LoadForRaceOrNull(string race)
    {
        string perRacePath = PathOf(FileNameFor(race));
        if (File.Exists(perRacePath))
        {
            try
            {
                var json = File.ReadAllText(perRacePath);
                return JsonUtility.FromJson<QuickBarSave>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[QuickBarPersistence] LoadForRace failed: {e}");
                return null;
            }
        }

        // ���Ž� quickbar.json �� ���� ���Ϸ� ���̱׷��̼�
        string legacyPath = PathOf(LegacyFileName);
        if (File.Exists(legacyPath))
        {
            try
            {
                var json = File.ReadAllText(legacyPath);
                var data = JsonUtility.FromJson<QuickBarSave>(json);
                if (data != null)
                {
                    SaveForRace(race, data);
#if UNITY_EDITOR
                    Debug.Log($"[QuickBarPersistence] Migrated {LegacyFileName} �� {FileNameFor(race)}");
#endif
                    return data;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[QuickBarPersistence] Migrate legacy failed: {e}");
            }
        }

        return null;
    }

    public static QuickBarSave LoadForRaceOrNew(string race)
        => LoadForRaceOrNull(race) ?? new QuickBarSave();

    // ====== (���Ž� API ����: �ʿ�� ���� �ڵ� ȣȯ) ======
    private static string LegacyFilePath => PathOf(LegacyFileName);

    public static void Save(QuickBarSave data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(LegacyFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuickBarPersistence] Save (legacy) failed: {e}");
        }
    }

    public static QuickBarSave Load()
    {
        try
        {
            if (!File.Exists(LegacyFilePath)) return null;
            var json = File.ReadAllText(LegacyFilePath);
            return JsonUtility.FromJson<QuickBarSave>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuickBarPersistence] Load (legacy) failed: {e}");
            return null;
        }
    }
}
