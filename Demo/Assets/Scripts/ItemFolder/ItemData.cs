using System;

// �������� �⺻ �����͸� �����ϴ� Ŭ����
[Serializable] // Unity Inspector�� JSON ����ȭ�� ���� ����ȭ �����ϰ� ��
public class ItemData
{
    public int id;      // ������ ���� ID, ���� ������ �������� �ĺ��� �� ���
    public string name; // ������ �̸�, UI ǥ�ó� �α� ��¿� ���
    public float hp;
    public float mp;
    public float atk;   // ������ ���ݷ� �Ǵ� �Ӽ� ��, ���� �������� ��ġ ��꿡 ���
    public float def;
    public float dex;
    public float As;
    public float cc;
    public float cd;
    public string type; // ������ ���� Ÿ��
}