using System.Collections.Generic; // C#���� �������ִ� �ڷᱸ��(List<T>, dictionary<K,V> ���� ��) ��� ����
using System;
using UnityEngine;

public enum Jobs
{
    ����,
    ����,
    �ü�,
    ������
}

// ����� ���� Ÿ�� / Value Ÿ�� /
// GC �ʿ� ����(���� �������� ������ ���� �Ҵ�/�����ϴ� ���信�� Ȱ�� ex. Vector3)
[System.Serializable]
public struct Book
{
    public string name;
    public string description;
}

// ��ü�� ���� ���赵(�Ӽ��� ���) / ����Ƽ������ class ��� ����(�������� ����)
[Serializable]
public class Items
{
    public string name;
    public string description;
}

public class InspectorAttributes : MonoBehaviour
{



    //[Header("Score")] public int point; (�� �ڵ�� ����)
    [Header("Score")]
    public int point;
    public int max_point;
    [Header("Infomation")]
    public string nickname;
    
    public Jobs jobs;

    // �ν����Ϳ� �����ϱ� ���� ���� ���� ����
    [HideInInspector]
    public int value = 5;

    // ����Ƽ���� �����(Private) �ʵ带 �ν����Ϳ� �����Ű�� ����Ƽ�� ����ȭ �ý��ۿ� ���Խ�ŵ�ϴ�.
    // ���Ȼ��� ������ ������ ���Ƴ��� ���� �����ڰ� ���� �����ϵ���
    // ��� ����
    // public -> ���� o ���� o
    // private -> ���� x ���� x
    // serializeField + private -> ���� x, �ν����Ϳ����� ���� ����

    // ����ȭ(Serialization) : �����͸� ���� ������ �������� ��ȯ�ϴ� �۾�
    // �� ��ȯ�� ���� ��, ������, ���� � �����ϰ� �����ϴ� �۾��� ������ �� �ֽ��ϴ�.

    // ����ȭ ����
    // 1. public or [SerializeField]
    // 2. static�� �ƴ� ���
    // 3. ����ȭ ������ ������ Ÿ���� ���

    // ����ȭ�� ������ ������
    // 1. �⺻ ������(int, float, bool, string, ...)
    // 2. ����Ƽ���� �������ִ� ����ü(Vector3, Quaternion, Color)
    // 3. ����Ƽ ��ü ����(GameObject, Transform, Material)
    // 4. [Serializable] �Ӽ��� ���� Ŭ����
    // 5. �迭 / ����Ʈ

    // ����ȭ �Ұ����� ������
    // 1. Dictionary<K,V>
    // 2. Interface (�������̽�)
    // 3. static Ű���尡 ���� �ʵ�
    // 4. abstract(�߻� Ŭ����) Ű���尡 ���� �ʵ�

    //[SerializeField]
    //private int value2 = 7;

    // Space(����) : ���� ���̸�ŭ ������ ����ϴ�.


    // TextArea : �� ���ڿ��� ���� �ٷ� ���� �� �ִ� ����
    // [TextArea]�� ����� ��� ���� �� �Է��� ������ ���°� �˴ϴ�.
    // [TextArea(�⺻ ǥ�� ��, �ִ� ��)}�� ���� �ν����� �󿡼��� ���̸� �����մϴ�.
    // ���ڿ� ���̿� ���� �������� �κ��� �����ϴ�.
    // ���ڿ��� �幮�� ��� ������ �Ӽ�
    // �⺻�� 1��, ������ ������ �� ��ġ��ŭ ĭ�� �þ�ϴ�.
    
    [Space(30)][TextArea(5,10)]
    public string quest_info;

    public Book book;
    public Items item;

    // ����Ƽ �ν����Ϳ����� �迭�� ����Ʈ�� ������ �˴ϴ�.
    // ����Ʈ<T>�� T ������ �����͸� ��������, ���������� ������ �� �ִ� �������Դϴ�.
    // �������� �˻�, �߰�, ���� ���� ����� �����˴ϴ�.
    public List<Items> item_list;

    public Book[] books = new Book[5];

    // Range(�ּ�, �ִ�)�� ���� �ش� ���� �����Ϳ��� �ּ� ���� �ִ밡 �����Ǿ��ִ�
    // ��ũ�� ������ ������ ����˴ϴ�.
    [Range(0, 100)] public int sound;
    [Range(0, 100)] public float sfx;
}
