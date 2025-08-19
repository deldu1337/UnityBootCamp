using System;
using UnityEngine;
using UnityEngine.Events;
// UnityEngine.Events ���ӽ����̽� ������ �ʼ��Դϴ�.

// C#�� event���� ������
// 1. �ν����Ϳ��� Ȯ���� �����ϴ�.
// 2. �߰� ���� ����� +=, -=���� ���� �ʰ� AddListener�� RemoveListener�� �����մϴ�.

//                     UnityAction           vs           UnityEvent
// Ÿ��                 delegate                             class
// ���                 �Լ� ����                   �����Ϳ��� �ڵ鷯 ���� ��� ����
// ����                   +, -                      AddListener(), RemoveListener()
// ����� ��Ȳ        ��ũ��Ʈ �ڵ� �� ó��         �ν����Ϳ� �̺�Ʈ �ý���
// �ӵ�                   ����                      ����(���� ������ ���� �Ľ� �� ��Ÿ�� ���� ���)
// �޸�                 ����                      ����
// GC �߻� ����           ����                      ����
// ���Ǽ�             ��ü ���� �ؾ���              �ٷ� ��� ����. ���� ����

// UnityAction�� UnityEvent�� ����ϴ� �ڵ带 ������ �� ȿ�����Դϴ�.
// �Ϲ� delegate�� UnityAction<T>�� ���� Ÿ�Կ� ���� ������ �ȵǾ��־� ���� ���� ����ؾ� �մϴ�.

// ����� �� �ִ� ������
// 1. C# delegate
// 2. Unity UnityAction
// 3. C# Func<T>, Aciton<T>
public class EventSample3 : MonoBehaviour
{
    public UnityEvent OnKButtonEnter;
    public UnityAction OnAction;

    private void Start()
    {
        //OnKButtonEnter += Sample; // ����
        OnKButtonEnter.AddListener(Sample);
        OnAction += Sample2;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            OnKButtonEnter?.Invoke();
        }
    }

    private void Sample()
    {
        Debug.Log("<color=cyan>����</color>");
    }

    private void Sample2()
    {
        Debug.Log("<color=green>����</color>");
    }
}
