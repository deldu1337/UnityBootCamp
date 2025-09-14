using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static string playerDataPath => Path.Combine(Application.persistentDataPath, "playerData.json");

    // �÷��̾� ������ ����
    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(playerDataPath, json);
        Debug.Log($"�÷��̾� ������ ���� �Ϸ�: {playerDataPath}");
    }

    // �÷��̾� ������ �ε� (������ �⺻�� ����)
    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(playerDataPath))
        {
            Debug.LogWarning("����� �÷��̾� �����Ͱ� �����ϴ�. �⺻ ������ ����!");

            // �⺻ ������ ����
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

            // ���Ϸ� �ٷ� ����
            SavePlayerData(newData);
            return newData;
        }

        string json = File.ReadAllText(playerDataPath);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
