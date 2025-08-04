using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera _cam;
    void Awake()
    {
        //���� MainCamera �±� �޸� ī�޶� ã��
        if (Camera.main != null) _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (_cam == null) return;
        // 1) ����-�����̽� ������Ʈ�� �ո�(+Z��)�� ī�޶� �ִ� �������� ���ϰ�
        Vector3 dir = (_cam.transform.position - transform.position).normalized;
        // 2) �Ĵٺ��� ȸ��: forward �� ī�޶� ������ �ǵ���
        transform.rotation = Quaternion.LookRotation(dir);

        // Y�� ȸ���� ���ϸ�
        // var lookPos = _cam.transform.position;
        // lookPos.y = transform.position.y;  
        // transform.LookAt(lookPos);
    }
}