using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static string playerDataPath => Path.Combine(Application.persistentDataPath, "playerData.json");

    // 플레이어 데이터 저장
    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(playerDataPath, json);
        Debug.Log($"플레이어 데이터 저장 완료: {playerDataPath}");
    }

    // 플레이어 데이터 로드 (없으면 null 반환)
    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(playerDataPath))
        {
            Debug.LogWarning("저장된 플레이어 데이터가 없습니다. 새 게임으로 시작합니다.");
            return null; // null 반환
        }

        string json = File.ReadAllText(playerDataPath);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
