using UnityEngine;

public class UnityRandom : MonoBehaviour
{
    [ContextMenuItem("���� �� ȣ��", "MenuAttributesMethod")]
    public int rand;

    public void MenuAttributesMethod()
    {
        // ����Ƽ�� ���� Random.Range(�ּ�, �ִ�)
        // �ּ� �� ���� ����
        // �ִ� �� ���� x (����)

        // �ּ� �� ���� ����
        // �ִ� �� ���� o (�Ǽ�)
        rand = Random.Range(1, 101); // 1 ~ 100
        // 1 ~ 100 �߿��� 90���� ���� ������ ��� = 90%
    }
}
