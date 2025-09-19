using System;
using System.IO;
using UnityEngine;

public static class SaveLoadService
{
    // ���ϸ� ����
    private const string PlayerFile = "playerData.json";
    private const string InventoryFile = "playerInventory.json";
    private const string EquipmentFile = "playerEquipment.json";

    // ���� ��� ����
    private static string PathOf(string fileName) =>
        System.IO.Path.Combine(Application.persistentDataPath, fileName);

    // -----------------------------
    // ���� ���׸� ����/�ε� ��ƿ
    // -----------------------------
    public static void Save<T>(T data, string fileName, bool prettyPrint = true)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint);
            File.WriteAllText(PathOf(fileName), json);
#if UNITY_EDITOR
            Debug.Log($"[SaveLoadService] Saved {typeof(T).Name} �� {PathOf(fileName)}");
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
            string json = File.ReadAllText(path);
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

    // -----------------------------
    // PlayerData
    // -----------------------------
    public static void SavePlayerData(PlayerData data) =>
        Save(data, PlayerFile);

    public static PlayerData LoadPlayerDataOrNull()
    {
        return TryLoad(PlayerFile, out PlayerData data) ? data : null;
    }

    // -----------------------------
    // InventoryData
    // -----------------------------
    public static void SaveInventory(InventoryData data) =>
        Save(data, InventoryFile);

    public static InventoryData LoadInventoryOrNew()
    {
        if (TryLoad(InventoryFile, out InventoryData data) && data != null)
            return data;

        return new InventoryData(); // ��� ������ ����
    }

    // -----------------------------
    // EquipmentData
    // -----------------------------
    public static void SaveEquipment(EquipmentData data) =>
        Save(data, EquipmentFile);

    public static EquipmentData LoadEquipmentOrNew()
    {
        if (TryLoad(EquipmentFile, out EquipmentData data) && data != null)
            return data;

        return new EquipmentData(); // ��� ������ ����
    }
}
