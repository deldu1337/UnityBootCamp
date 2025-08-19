using UnityEngine;
using System;
// C#�� Event ����
// Ŭ���̳� ��ġ�� ���� ������ ó���ϴ� �ϳ��� �ý���

// ������(publisher)
// ������� �ൿ�� ��ٸ��ٰ� �����ڿ��� �˷��ִ� ������ �����մϴ�.

// ������(Subscribers)
// �̺�Ʈ �����ڿ� ���� ������ ���� ������� �ൿ�� ���޹޾Ƽ� �����ϴ� ������
// �����մϴ�.

// �������� ���, �������� �ൿ ��ü�� �����ڰ� �˾ƾ� �� �ʿ�� ���� ����.
// �������� ���, ���к��ϰ� ������ ��� ���� �����ڵ��� ����� ����� �� ����.

// event ����� ���ؼ� System ���ӽ����̽��� ����ؾ� �մϴ�.

public class EventSample : MonoBehaviour
{
    public event EventHandler OnSpaceEnter;
    // �̺�Ʈ ������ �̸��� ���� On + ���� / ������ ��������ϴ�.

    // C#���� �������ִ� delegate Ÿ��
    // EventHandler�� ��� ��ġ�� Ŭ�� ���� �̺�Ʈ�� �����ϴ� �뵵
    // �Ű�����
    // Object sender <- object Ÿ���� �����͸� ���޹��� �� ������,
    // �̺�Ʈ�� �߻���Ų ����� �ǹ��մϴ�.

    // EventArgs e <- �̺�Ʈ�� ���õ� �����͸� ��� �ִ� ��ü�� �ǹ��մϴ�.
    // �ش� ���� EventArgs �Ǵ� �ش� �ڽ� Ŭ������ �� �� �ֽ��ϴ�.

    private void Start()
    {
        // ���� ���
        // �̺�Ʈ�� += ���¿� �´� �޼ҵ� �̸�;
        OnSpaceEnter += Debug_OnSpaceEnter;
    }

    void Update()
    {
        // 1) �̺�Ʈ ���� ��� �̺�Ʈ��(this, EventArgs.Empty)
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽� ��ư Ŭ��
        {
            // Null �˻縦 �����ϰ� ����(�̺�Ʈ ������ �ȵǾ� ���� ��쿡�� �����ϸ� �ȵǱ� ����)
            if(OnSpaceEnter != null)
            {
                OnSpaceEnter(this, EventArgs.Empty);
            }
            // this: �̺�Ʈ�� �߻���Ų ��ü(���� Ŭ����)
            // EventArgs.Empty: �̺�Ʈ ���࿡ �־� Ư���� �߰��Ǵ� �����Ͱ� ������ �ǹ��մϴ�.
        }

        // 2) �̺�Ʈ ���� ��� Invoke �Լ��� ����ϴ� ���
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽� ��ư Ŭ��
        {
            // ?.�� ���� null�� �ƴ� �� ó���ǵ��� �Ѵ�.
            OnSpaceEnter?.Invoke(this, EventArgs.Empty);
            // �Ǵ� OnSpaceEnter?.Invoke(OnSpaceEnter, EventArgs.Empty);

            // int?�� ���� �ڷ����� ?�� ���̰� Nullable �� Ÿ������ ����ϴ� ���
            // ������������ null ���� ���� �� �ְ� ���ݴϴ�. (T?)
            // Ÿ���� ������ �� ���˴ϴ�.
            // �� ������ Ÿ�Կ� ���˴ϴ�.

            // ?. �� ���·� ���̴� ��� Null ���� �����ڷ� ���Ǵ� ���
            // ��ü�� null�� �ƴ� ���� ����� ���� ȣ���� �����ϵ��� �����մϴ�.
            // �޼ҵ�, �Ӽ�, �̺�Ʈ ���� ȣ�� �ÿ� ���˴ϴ�.
            // ���۷��� ������ Ÿ�� �Ǵ� nullable ��ü�� ������� ���˴ϴ�.

            // >> if(obj != null) ������ �ڵ� ��� ���
        }
    }
    private void Debug_OnSpaceEnter(object sender, EventArgs e)
    {
        Debug.Log("<color=yellow>���� Ű �Է� �̺�Ʈ ����</color>");
    }
}
