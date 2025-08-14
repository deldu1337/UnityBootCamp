using System;
using System.IO;
using UnityEngine;

public class JsonMaker : MonoBehaviour
{
    [Serializable]
    public class QuestData
    {
        public string quest_name;
        public string reward;
        public string description;
    }

    [Serializable]
    public class QuestList
    {
        public QuestData[] quests;
    }
    
    private void Start()
    {
        // 1) ������ ��ü�� ���� �ʱ�ȭ �۾�
        QuestList list = new QuestList()
        {
            quests = new QuestData[]
            {
                // new �����ڸ�() { �ʵ��1 = ��1, �ʵ��2 = ��2, ... } �ش� ������ ���� ���� Ŭ���� ��ü�� �����˴ϴ�.

                new QuestData() { quest_name = "������ ���̴�.", reward = "exp + 100", description = "�����̶� �ϸ� ���̶� ����." },

                new QuestData() { quest_name = "�߰��� �ض�.", reward = "exp + 150", description = "�߰��̶� �ϴ°� ����." },

                new QuestData() { quest_name = "�ҰŸ� ������ �ض�.", reward = "exp + 500", description = "�׳� �� �϶�°��ݾ�;;" }
            }
        };

        // 2) JsonUtility.ToJson(Object, pretty_print);�� ���� C#�� ��ü�� JSON���� �������ִ� ����ȭ ����� ���� �Լ�
        string json = JsonUtility.ToJson(list, true);
        // ToJson(��ü)�� �ش� ��ü�� JSON ���ڿ��� �������ִ� �ڵ�
        // true�� �߰��� ���, �鿩����� �ٹٲ��� ���Ե� ������ json ���Ϸ� ������ݴϴ�.
        // false�� ���ų�, �����ϴ� ����� ���� �� �ٷ� ����Ǹ� json ���Ϸ� �����˴ϴ�.

        // 3) ���� ��ο� ���� �ۼ��� �����մϴ�.
        Debug.Log($"�� ���� ���� ��ġ: {Application.persistentDataPath}");
        string path = Path.Combine(Application.persistentDataPath, "quests.json");
        // Path.Combine(string path1, string path2);
        // �� ����� ���ڿ��� �ϳ��� ��η� ������ִ� ����� ������ �ֽ��ϴ�.
        // ���� ��ġ/���ϸ����� ���� ����մϴ�.

        // Application.persistentDataPath: ����Ƽ�� �� �÷������� �����ϴ� ���� ���� ������ ���� ���

        // 4) �ش� ��ο� ������ �ۼ�
        File.WriteAllText(path, json);
        // C# 723 page: System.IO ���ӽ����̽�
        //    725 page: Path Ŭ������ ���� ���� �̸�, Ȯ����, ���� ���� ��� ���
        //    733 page: Json �����Ϳ� ���� ����

        Debug.Log("JSON ���� ���� �Ϸ�");

        // ==================== ���� �ε� ====================
        // 1) �ش� ��ο� ������ �����ϴ��� �Ǵ��ϼ���.

        if(File.Exists(path))
        {
            // ���� �ؽ�Ʈ�� ���� �о ������ �����ͷ� �����մϴ�.
            string json2 = File.ReadAllText(path);

            QuestList loaded = JsonUtility.FromJson<QuestList>(json2);

            Debug.Log($"����Ʈ ����: {loaded.quests[0].quest_name}");
        }
        else
        {
            Debug.LogWarning("�ش� ��ο� ����� JSON ������ �������� �ʽ��ϴ�.");
        }
    }
}
