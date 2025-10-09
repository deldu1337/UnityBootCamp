using System.IO;
using UnityEngine;

public static class PotionQuickBarPersistence
{
    private const string LegacyFile = "potion_quickbar.json";

    private static string PathOf(string fileName)
        => Path.Combine(Application.persistentDataPath, fileName);

    private static string FileNameForRace(string race)
    {
        var rk = string.IsNullOrWhiteSpace(race) ? "humanmale" : race.ToLower();
        return $"potion_quickbar_{rk}.json";
    }

    private static string FilePathForRace(string race)
        => PathOf(FileNameForRace(race));

    /// <summary>������ ����</summary>
    public static void SaveForRace(string race, PotionQuickBarSave data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePathForRace(race), json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PotionQuickBarPersistence] Save failed: {e}");
        }
    }

    /// <summary>
    /// ������ �ε�. ������ ���Ž� ����(����) �ֳ� ����, ������ �ش� ���� ���Ϸ� 1ȸ ���̱׷��̼��ؼ� ��ȯ.
    /// �� �� ������ �� ������ ��ȯ.
    /// </summary>
    public static PotionQuickBarSave LoadForRaceOrNew(string race)
    {
        // 1) ������ ���� �켱
        var perRacePath = FilePathForRace(race);
        if (File.Exists(perRacePath))
        {
            try
            {
                var json = File.ReadAllText(perRacePath);
                return JsonUtility.FromJson<PotionQuickBarSave>(json) ?? new PotionQuickBarSave();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PotionQuickBarPersistence] Load failed (per race): {e}");
                return new PotionQuickBarSave();
            }
        }

        // 2) ���Ž� ���� ������ �� ���� ���� ���Ϸ� ���̱׷��̼�
        var legacyPath = PathOf(LegacyFile);
        if (File.Exists(legacyPath))
        {
            try
            {
                var json = File.ReadAllText(legacyPath);
                var legacy = JsonUtility.FromJson<PotionQuickBarSave>(json) ?? new PotionQuickBarSave();

                // ��� �� ���� ���Ϸ� ����
                SaveForRace(race, legacy);
#if UNITY_EDITOR
                Debug.Log($"[PotionQuickBarPersistence] Migrated legacy {LegacyFile} �� {FileNameForRace(race)}");
#endif
                return legacy;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PotionQuickBarPersistence] Migrate failed: {e}");
                return new PotionQuickBarSave();
            }
        }

        // 3) �ƹ��͵� ������ �� ������
        return new PotionQuickBarSave();
    }
}
