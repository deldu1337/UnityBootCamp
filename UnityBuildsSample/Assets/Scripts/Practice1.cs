using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseEventArgs : EventArgs
{
    // 전달할 값에 대한 설계(프로퍼티로 작업하며, get 기능만 주로 활성화합니다.)
    public int Value { get; } // Value에 대한 접근만 가능
    public int Cost { get; } // Value에 대한 접근만 가능

    // EventArgs에 대한 생성이 발생하면 값이 설정됩니다. (생성자) 
    public ChooseEventArgs(int value, int cost)
    {
        Value = value;
        Cost = cost;
    }
}
public class Practice1 : MonoBehaviour
{
    public event EventHandler<ChooseEventArgs> OnClick; // 데미지를 받았을 때에 대한 이벤트 핸들러
    public string[] items = { "Normal Knife", "Rare Knife", "Legend Knife" };

    public Text text1;
    public Text text2;
    public Text text3;

    static private int count = 10;
    public void TakeItem(int value, int cost)
    {
        // 전달받은 값 기준으로 데미지 이벤트 매개변수를 생성해,
        // 핸들러 호출 시의 정보로 전달합니다.
        OnClick?.Invoke(this, new ChooseEventArgs(value, cost));
        if (value <= 7)
        {
            Debug.Log($"{value}번 아이템 흭득: {items[0]}");
            text1.text = $"{value}번 아이템 흭득: {items[0]}";
            text3.text += " ●";
        }
        else if (value > 7 && value <= 9)
        {
            Debug.Log($"<color=red>{value}번 레어 아이템 흭득: {items[1]}</color>");
            text1.text = $"<color=red>{value}번 레어 아이템 흭득: {items[1]}</color>";
            text3.text += " <color=red>●</color>";
        }
        else
        {
            Debug.Log($"<color=orange>{value}번 레어 아이템 흭득: {items[2]}</color>");
            text1.text = $"<color=orange>{value}번 레어 아이템 흭득: {items[2]}</color>";
            text3.text += " <color=orange>●</color>";
        }
        Debug.Log($"남은 횟수: {cost}번");
        text2.text = $"남은 횟수: {count}";
        
    }

    private void Start()
    {
        //text = gameObject.GetComponent<Text>();
        text1.text = "아이템 뽑기";
        text2.text = $"남은 횟수: {count}";
        text3.text = " ";
    }

    private void Update()
    {
        if(count > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TakeItem(UnityEngine.Random.Range(1, 11), --count);
            }
        }
        else
        {
            text2.text = "횟수를 소진하셨습니다.";
        }
    }
}
