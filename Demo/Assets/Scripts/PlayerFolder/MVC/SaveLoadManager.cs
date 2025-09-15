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

    // �÷��̾� ������ �ε� (������ null ��ȯ)
    public static PlayerData LoadPlayerData()
    {
        if (!File.Exists(playerDataPath))
        {
            Debug.LogWarning("����� �÷��̾� �����Ͱ� �����ϴ�. �� �������� �����մϴ�.");
            return null; // null ��ȯ
        }

        string json = File.ReadAllText(playerDataPath);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
