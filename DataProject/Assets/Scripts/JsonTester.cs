using System;
using UnityEngine;

public class JsonTester : MonoBehaviour
{
    // ����Ƽ���� ��ü(Object)�� �ʵ�(Field)�� json���� ��ȯ�ϱ� ���ؼ���
    // ���������� �޸𸮿��� �����͸� �а� ���� �۾��� �����ؾ� ��
    // ���� [Serializable] �Ӽ��� �߰��� �ش� ������ ���� ����ȭ�� ó���� �� �ʿ䰡 �ֽ��ϴ�.

    // ����ȭ�� �����͸� �����ϰų� �����ϱ� ���� �������� �������� ���·� �������ִ�
    // �۾��� �ǹ��մϴ�.

    [Serializable]
    public class Data
    {
        public int hp;
        public int atk;
        public int def;
        public string[] item;
        public Position position;
        public string Quest;
        public bool isDead;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
    }

    public Data my_data;

    // Use this for initiaization
    void Start()
    {
        var jsonText = Resources.Load<TextAsset>("data01");

        if(jsonText == null)
        {
            Debug.LogError("�ش� JSON ������ ���ҽ� �������� ã�� ���߽��ϴ�!");
            return;
        }

        // Json ���ڿ��� ��ü�� ���·� ��ȯ�մϴ�.
        my_data = JsonUtility.FromJson<Data>(jsonText.text);

        Debug.Log(my_data.hp);
        Debug.Log(my_data.atk);
        Debug.Log(my_data.def);
        Debug.Log(my_data.position.x);
        Debug.Log(my_data.position.y);
        Debug.Log(my_data.Quest);
        Debug.Log(my_data.isDead);

        foreach (var item in my_data.item)
            Debug.Log(item);
    }

    void Update()
    {
        
    }
}
