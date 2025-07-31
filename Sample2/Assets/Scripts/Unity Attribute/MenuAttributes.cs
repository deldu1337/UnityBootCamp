using UnityEngine;
// Attribute : [AddComponentMenu("")] ó�� Ŭ������ �Լ�, ���� �տ� �ٴ� []���� Attribute(�Ӽ�)
// �����Ϳ� ���� Ȯ���̳� ����� ���� �� ���ۿ��� �����Ǵ� ��ɵ�
// ��� ���� : ����ڰ� �����͸� �� ����������, ���������� ����ϱ� ���ؼ�

// 1. AddComponentMenu("�׷��̸�/����̸�")
// Editor�� Component�� �޴��� �߰��ϴ� ���
// �׷��� ���� ��� �׷��� �����Ǹ�, �ƴ� ��쿡�� ��ɸ� ����˴ϴ�.
// ���� ���� ���� ������ ���� ���� ������ ������ �ֽ��ϴ�.
// 1. !,#,$,* Ư�������� ��� �� ��
// 2. ����
// 3. �빮��
// 4. �ҹ���

[AddComponentMenu("Sample/AddComponentMenu")]
public class MenuAttributes : MonoBehaviour
{
    // [ContextMenuItem("������� ǥ���� �̸�", "�Լ��� �̸�")]
    // message�� ������� MessageReset ����� �����ͷμ� ����� �� �ֽ��ϴ�.
    // [ContextMenuItem("�޽��� �ʱ�ȭ","MessageReset")] public string message = "";
    // ��ó�� ContextMenuItem�� �ʵ� ���̱� ������ �ڿ� ���� �ٿ��ִ� �Ͱ� ����
    // �ٸ�, �ڵ尡 ������� ������ ���� �Ʒ�ó�� �ٹٲ��Ͽ� ���
    [ContextMenuItem("�޽��� �ʱ�ȭ","MessageReset")]
    public string message = "";
    public void MessageReset()
    {
        message = "";
    }

    [ContextMenu("��� �޽��� ȣ��")]
    public void MenuAttributesMethod()
    {
        Debug.LogWarning("��� �޽���!");
    }
}
