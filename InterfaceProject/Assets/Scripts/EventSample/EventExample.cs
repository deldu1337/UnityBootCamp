using System;
using UnityEngine;
// C# event 526 page

// event: Ư�� ��Ȳ�� �߻����� �� �˸��� ������ ��Ŀ����
// 1. �÷��̾ �׾��� ��, �˸� ����, �޼ҵ� ȣ��

// Action

//public class Tester : MonoBehaviour
//{
//    private void Start()
//    {
//        EventExample eventExample = new EventExample();
//        eventExample.onDeath?.Invoke();
//        //eventExample.onStart?.Invoke(); // event Ű���尡 ���� ���, �ܺο����� ȣ�� �Ұ���

//        eventExample.onDeath = null;
//        //eventExample.onStart = null; // event Ű����� ������ �Ұ���

//        eventExample.onStart += Samples; // ���� ���� / ���� ��ҵ� ����
//    }

//    private void Samples()
//    {

//    }
//}

//                   Action          vs          EventAction
// �ܺ� ȣ��           o                             x
// �ܺ� ����           o                             x
// ���� ���           o                             o
// ���� ���           o                             o
// �� �뵵        ���η���, �ݹ�                 �̺�Ʈ �˸�

public class EventExample : MonoBehaviour
{
    // Action vs Event Action ??
    public Action onDeath;
    public event Action onStart;

    private void Start()
    {
        // �׼��� +=�� ���� �Լ�(�޼ҵ�)�� �׼ǿ� ����� �� �ֽ��ϴ�. (���)
        // �׼��� -=�� ���� �Լ�(�޼ҵ�)�� �׼ǿ��� ������ �� �ֽ��ϴ�. (����)
        // �׼��� ����� ȣ���ϸ� ��ϵǾ��ִ� �Լ��� ���������� ȣ��˴ϴ�.
        onStart += Ready;
        onStart += Fight;

        onDeath += Damaged;
        onDeath += Dead;

        // onStart�� ��ϵ� ����� �����ϴ� �ڵ� invoke();
        onStart?.Invoke();
        onDeath?.Invoke();

        // �Լ�ó�� ȣ���ϴ� �͵� �����մϴ�.
        onStart();
        onDeath();

        // ���� ����? ����. ���� ��Ÿ�� ����
        // �Լ� ȣ�⵵ ������ ���������� invoke()�� ȣ���ϰ� �Ǿ�����.
        // Invoke ����̸� null üũ ����. �ܺο����� ȣ��, ������ �䱸 �� ��õ
        // �Լ� ���¸� ���� ��������� ��. ���� �ڵ��̰ų� �ܼ� ȣ���� ��� �ش� ��� ��õ
    }

    private void Fight()
    {
        Debug.Log("<color=yellow><b>Fight!!</b></color>");
    }

    private void Ready()
    {
        Debug.Log("<color=cyan><b>Ready??</b></color>");
    }

    private void Dead()
    {
        Debug.Log("<color=blue><b>A hero is fallen.</b></color>");
    }

    private void Damaged()
    {
        Debug.Log("<color=red><b>Critical Demage!</b></color>");
    }
}
