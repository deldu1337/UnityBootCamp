using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Vector3 distance; // ī�޶�� �÷��̾� �� ��� ��ġ (offset)

    void Start()
    {
        // �ʱ� ī�޶� ��ġ�� �������� offset ���
        Vector3 vector3 = new Vector3(-2f, 17f, -2f); // ī�޶� �ʱ� ��ġ
        distance = vector3 - transform.position;       // �÷��̾� ��ġ ���� offset ���
        // �ּ� ó����, ������ �ʱ� ���ͷ� offset ����
    }

    void FixedUpdate()
    {
        // ī�޶� ��ġ�� �÷��̾� ��ġ + offset���� ����
        Camera.main.transform.position = distance + transform.position;

        // ���� ī�޶� ��ġ ���� offset ����
        distance = Camera.main.transform.position - transform.position;
    }
}