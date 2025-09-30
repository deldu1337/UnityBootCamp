//using System;
//using System.IO;
//using UnityEngine;

//public static class SaveLoadService
//{
//    // 파일명 정의
//    private const string PlayerFile = "playerData.json";
//    private const string InventoryFile = "playerInventory.json";
//    private const string EquipmentFile = "playerEquipment.json";

//    // 공통 경로 조립
//    private static string PathOf(string fileName) =>
//        System.IO.Path.Combine(Application.persistentDataPath, fileName);

//    // -----------------------------
//    // 공통 제네릭 저장/로드 유틸
//    // -----------------------------
//    public static void Save<T>(T data, string fileName, bool prettyPrint = true)
//    {
//        try
//        {
//            string json = JsonUtility.ToJson(data, prettyPrint);
//            File.WriteAllText(PathOf(fileName), json);
//#if UNITY_EDITOR
//            Debug.Log($"[SaveLoadService] Saved {typeof(T).Name} → {PathOf(fileName)}");
//#endif
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"[SaveLoadService] Save failed ({typeof(T).Name}): {e}");
//        }
//    }

//    public static bool TryLoad<T>(string fileName, out T data)
//    {
//        string path = PathOf(fileName);
//        if (!File.Exists(path))
//        {
//            data = default;
//#if UNITY_EDITOR
//            Debug.LogWarning($"[SaveLoadService] Not found: {path}");
//#endif
//            return false;
//        }

//        try
//        {
//            string json = File.ReadAllText(path);
//            data = JsonUtility.FromJson<T>(json);
//            return true;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"[SaveLoadService] Load failed ({typeof(T).Name}): {e}");
//            data = default;
//            return false;
//        }
//    }

//    // -----------------------------
//    // PlayerData
//    // -----------------------------
//    public static void SavePlayerData(PlayerData data) =>
//        Save(data, PlayerFile);

//    public static PlayerData LoadPlayerDataOrNull()
//    {
//        return TryLoad(PlayerFile, out PlayerData data) ? data : null;
//    }

//    // -----------------------------
//    // InventoryData
//    // -----------------------------
//    public static void SaveInventory(InventoryData data) =>
//        Save(data, InventoryFile);

//    public static InventoryData LoadInventoryOrNew()
//    {
//        if (TryLoad(InventoryFile, out InventoryData data) && data != null)
//            return data;

//        return new InventoryData(); // 비어 있으면 새로
//    }

//    // -----------------------------
//    // EquipmentData
//    // -----------------------------
//    public static void SaveEquipment(EquipmentData data) =>
//        Save(data, EquipmentFile);

//    public static EquipmentData LoadEquipmentOrNew()
//    {
//        if (TryLoad(EquipmentFile, out EquipmentData data) && data != null)
//            return data;

//        return new EquipmentData(); // 비어 있으면 새로
//    }
//}
using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveLoadService
{
    // (기존) 단일 파일 — 레거시 호환용
    private const string LegacyPlayerFile = "playerData.json";

    private const string InventoryFile = "playerInventory.json";
    private const string EquipmentFile = "playerEquipment.json";

    private static string PathOf(string fileName) =>
        System.IO.Path.Combine(Application.persistentDataPath, fileName);

    private static void EnsureDir(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    // ---------- 공통 저장/로드 ----------
    //    public static void Save<T>(T data, string fileName, bool prettyPrint = true, bool atomic = true)
    //    {
    //        try
    //        {
    //            string path = PathOf(fileName);
    //            EnsureDir(path);
    //            string json = JsonUtility.ToJson(data, prettyPrint);

    //            if (atomic)
    //            {
    //                var tmp = path + ".tmp";
    //                File.WriteAllText(tmp, json, new UTF8Encoding(false));
    //#if UNITY_2021_2_OR_NEWER
    //                File.Replace(tmp, path, null);
    //#else
    //                if (File.Exists(path)) File.Delete(path);
    //                File.Move(tmp, path);
    //#endif
    //            }
    //            else
    //            {
    //                File.WriteAllText(path, json, new UTF8Encoding(false));
    //            }
    //#if UNITY_EDITOR
    //            Debug.Log($"[SaveLoadService] Saved {typeof(T).Name} → {path}");
    //#endif
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError($"[SaveLoadService] Save failed ({typeof(T).Name}): {e}");
    //        }
    //    }
    public static void Save<T>(T data, string fileName, bool prettyPrint = true)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint);
            File.WriteAllText(PathOf(fileName), json);
#if UNITY_EDITOR
            Debug.Log($"[SaveLoadService] Saved {typeof(T).Name} → {PathOf(fileName)}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadService] Save failed ({typeof(T).Name}): {e}");
        }
    }

    public static bool TryLoad<T>(string fileName, out T data)
    {
        string path = PathOf(fileName);
        if (!File.Exists(path))
        {
            data = default;
#if UNITY_EDITOR
            Debug.LogWarning($"[SaveLoadService] Not found: {path}");
#endif
            return false;
        }

        try
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            data = JsonUtility.FromJson<T>(json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadService] Load failed ({typeof(T).Name}): {e}");
            data = default;
            return false;
        }
    }

    // ---------- 종족별 파일명 ----------
    private static string PlayerFileFor(string race)
    {
        if (string.IsNullOrEmpty(race)) race = "humanmale"; // 기본값 방어
        return $"playerData_{race}.json";
    }

    // ---------- PlayerData: 종족별 ----------
    public static void SavePlayerDataForRace(string race, PlayerData data)
        => Save(data, PlayerFileFor(race));

    public static PlayerData LoadPlayerDataForRaceOrNull(string race)
        => TryLoad(PlayerFileFor(race), out PlayerData data) ? data : null;

    // ---------- 레거시(단일 파일) 호환 ----------
    public static PlayerData LoadLegacyPlayerDataOrNull()
        => TryLoad(LegacyPlayerFile, out PlayerData data) ? data : null;

    public static void MigrateLegacyIfMatchRace(PlayerData legacy, string race)
    {
        if (legacy == null) return;
        // Race가 비어 있거나(아주 옛날 버전) 같을 때만 해당 종족 파일로 마이그레이션
        if (string.IsNullOrEmpty(legacy.Race) || string.Equals(legacy.Race, race, StringComparison.OrdinalIgnoreCase))
        {
            legacy.Race = race;
            SavePlayerDataForRace(race, legacy);
            // 원하면 레거시 파일 삭제 가능:
            // var legacyPath = PathOf(LegacyPlayerFile);
            // if (File.Exists(legacyPath)) File.Delete(legacyPath);
        }
    }

    // ---------- Inventory/Equipment는 그대로(공유 파일) ----------
    public static void SaveInventory(InventoryData data) => Save(data, InventoryFile);
    public static InventoryData LoadInventoryOrNew()
        => TryLoad(InventoryFile, out InventoryData data) && data != null ? data : new InventoryData();

    public static void SaveEquipment(EquipmentData data) => Save(data, EquipmentFile);
    public static EquipmentData LoadEquipmentOrNew()
        => TryLoad(EquipmentFile, out EquipmentData data) && data != null ? data : new EquipmentData();
}
