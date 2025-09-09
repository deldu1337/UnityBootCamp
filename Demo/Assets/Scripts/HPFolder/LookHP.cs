using UnityEngine;

public class LookHP : MonoBehaviour
{
    private Camera _cam; // ī�޶� ���� (���� ī�޶�)

    void Awake()
    {
        _cam = Camera.main; // ������ MainCamera �ڵ� ����
    }

    void LateUpdate()
    {
        if (_cam == null) return; // ī�޶� ������ �ƹ��͵� ���� ����

        // UI�� ������Ʈ�� ī�޶� �׻� �ٶ󺸵��� ȸ�� ����
        // �ַ� HP ��, �̸�ǥ, ������ ���� � ���
        transform.rotation = _cam.transform.rotation;
    }
}