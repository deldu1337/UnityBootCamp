using UnityEngine;
using System;

// 1. EventArgs를 상속한 커스텀 클래스 만들기
public class DamageEventArgs : EventArgs
{
    // 전달할 값에 대한 설계(프로퍼티로 작업하며, get 기능만 주로 활성화합니다.)
    public int Value { get; } // Value에 대한 접근만 가능
    public string EventName { get; }

    // EventArgs에 대한 생성이 발생하면 값이 설정됩니다. (생성자) 
    public DamageEventArgs(int value, string eventName)
    {
        Value = value;
        EventName = eventName;
    }
}
public class EventSample4 : MonoBehaviour
{
    public event EventHandler<DamageEventArgs> OnDamage; // 데미지를 받았을 때에 대한 이벤트 핸들러

    public void TakeDamage(int value, string eventName)
    {
        // 전달받은 값 기준으로 데미지 이벤트 매개변수를 생성해,
        // 핸들러 호출 시의 정보로 전달합니다.
        OnDamage?.Invoke(this, new DamageEventArgs(value, eventName));

        Debug.Log($"<color=red>[{eventName}] 플레이어가 {value} 데미지를 받았습니다.</color>");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TakeDamage(UnityEngine.Random.Range(10, 200), "적의 공격");
        }
    }
}
