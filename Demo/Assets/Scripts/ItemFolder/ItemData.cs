using System;

// �������� �⺻ �����͸� �����ϴ� Ŭ����
[Serializable] // Unity Inspector�� JSON ����ȭ�� ���� ����ȭ �����ϰ� ��
public class ItemData
{
    public int id;      // ������ ���� ID, ���� ������ �������� �ĺ��� �� ���
    public string name; // ������ �̸�, UI ǥ�ó� �α� ��¿� ���
    public float atk;   // ������ ���ݷ� �Ǵ� �Ӽ� ��, ���� �������� ��ġ ��꿡 ���
}