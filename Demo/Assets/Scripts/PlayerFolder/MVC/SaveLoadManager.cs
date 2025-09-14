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

    // 플레이어 데이터 로드 (없으면 기본값 생성)
    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(playerDataPath))
        {
            Debug.LogWarning("저장된 플레이어 데이터가 없습니다. 기본 데이터 생성!");

            // 기본 데이터 생성
            PlayerData newData = new PlayerData
            {
                Level = 1,
                Exp = 0,
                ExpToNextLevel = 50f,
                MaxHP = 100f,
                MaxMP = 50f,
                Atk = 5f,
                Def = 5f,
                Dex = 10f,
                AttackSpeed = 2f,
                CritChance = 0.1f,
                CritDamage = 1.5f,
                CurrentHP = 100f,
                CurrentMP = 50f
            };

            // 파일로 바로 저장
            SavePlayerData(newData);
            return newData;
        }

        string json = File.ReadAllText(playerDataPath);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
