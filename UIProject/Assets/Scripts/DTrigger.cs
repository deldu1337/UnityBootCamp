using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DTrigger : MonoBehaviour
{
    public List<Dialog> scripts;

    public void OnTriggerEnter()
    {
        if (scripts != null && scripts.Count > 0)
        {
            DialogManager.Instance.StartLine(scripts);
            // Ŭ������.Instance.�޼ҵ��()�� ���� Ŭ������ ���� �ٷ� ����� �� �ֽ��ϴ�.
            // ���� ���� GetComponent�� public ������ ����ؼ� ����� �ʿ䰡 ���� ���մϴ�.


        }
    }
}
