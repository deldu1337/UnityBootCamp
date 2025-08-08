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
            // 클래스명.Instance.메소드명()과 같이 클래스의 값을 바로 사용할 수 있습니다.
            // 따로 값을 GetComponent나 public 등으로 등록해서 사용할 필요가 없어 편합니다.


        }
    }
}
