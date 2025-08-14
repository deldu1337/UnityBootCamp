using System;
using System.IO;
using UnityEngine;

public class JsonMakers : MonoBehaviour
{
    [Serializable]
    public class Stat
    {
        public string name;
        public Job job;
        public int STR;
        public int DEX;
        public int INT;
    }

    [Serializable]
    public class Job
    {
        public bool warrior;
        public bool rogue;
        public bool wizard;
    }

    public int num = 1;

    private void Start()
    {
        num = PlayerPrefs.GetInt("Num");
        if (num == 1)
        {
            // 1) ������ ��ü�� ���� �ʱ�ȭ �۾�
            Stat stat = new Stat();

            // 2) JsonUtility.ToJson(Object, pretty_print);�� ���� C#�� ��ü�� JSON���� �������ִ� ����ȭ ����� ���� �Լ�
            string json = JsonUtility.ToJson(stat, true);
            // ToJson(��ü)�� �ش� ��ü�� JSON ���ڿ��� �������ִ� �ڵ�
            // true�� �߰��� ���, �鿩����� �ٹٲ��� ���Ե� ������ json ���Ϸ� ������ݴϴ�.
            // false�� ���ų�, �����ϴ� ����� ���� �� �ٷ� ����Ǹ� json ���Ϸ� �����˴ϴ�.

            // 3) ���� ��ο� ���� �ۼ��� �����մϴ�.
            Debug.Log($"�� ���� ���� ��ġ: {Application.persistentDataPath}");
            string path = Path.Combine(Application.persistentDataPath, "stat.json");
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

            if (File.Exists(path))
            {
                // ���� �ؽ�Ʈ�� ���� �о ������ �����ͷ� �����մϴ�.
                string json2 = File.ReadAllText(path);

                Stat loaded = JsonUtility.FromJson<Stat>(json2);

                Debug.Log("JSON ���� �ε� �Ϸ�");
            }
            else
            {
                Debug.LogWarning("�ش� ��ο� ����� JSON ������ �������� �ʽ��ϴ�.");
            }
        }
    }
}
