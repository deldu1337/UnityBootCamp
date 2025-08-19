using UnityEngine;
using System;

// 1. EventArgs�� ����� Ŀ���� Ŭ���� �����
public class DamageEventArgs : EventArgs
{
    // ������ ���� ���� ����(������Ƽ�� �۾��ϸ�, get ��ɸ� �ַ� Ȱ��ȭ�մϴ�.)
    public int Value { get; } // Value�� ���� ���ٸ� ����
    public string EventName { get; }

    // EventArgs�� ���� ������ �߻��ϸ� ���� �����˴ϴ�. (������) 
    public DamageEventArgs(int value, string eventName)
    {
        Value = value;
        EventName = eventName;
    }
}
public class EventSample4 : MonoBehaviour
{
    public event EventHandler<DamageEventArgs> OnDamage; // �������� �޾��� ���� ���� �̺�Ʈ �ڵ鷯

    public void TakeDamage(int value, string eventName)
    {
        // ���޹��� �� �������� ������ �̺�Ʈ �Ű������� ������,
        // �ڵ鷯 ȣ�� ���� ������ �����մϴ�.
        OnDamage?.Invoke(this, new DamageEventArgs(value, eventName));

        Debug.Log($"<color=red>[{eventName}] �÷��̾ {value} �������� �޾ҽ��ϴ�.</color>");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TakeDamage(UnityEngine.Random.Range(10, 200), "���� ����");
        }
    }
}
