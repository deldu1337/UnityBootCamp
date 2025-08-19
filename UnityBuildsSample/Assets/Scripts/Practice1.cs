using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseEventArgs : EventArgs
{
    // ������ ���� ���� ����(������Ƽ�� �۾��ϸ�, get ��ɸ� �ַ� Ȱ��ȭ�մϴ�.)
    public int Value { get; } // Value�� ���� ���ٸ� ����
    public int Cost { get; } // Value�� ���� ���ٸ� ����

    // EventArgs�� ���� ������ �߻��ϸ� ���� �����˴ϴ�. (������) 
    public ChooseEventArgs(int value, int cost)
    {
        Value = value;
        Cost = cost;
    }
}
public class Practice1 : MonoBehaviour
{
    public event EventHandler<ChooseEventArgs> OnClick; // �������� �޾��� ���� ���� �̺�Ʈ �ڵ鷯
    public string[] items = { "Normal Knife", "Rare Knife", "Legend Knife" };

    public Text text1;
    public Text text2;
    public Text text3;

    static private int count = 10;
    public void TakeItem(int value, int cost)
    {
        // ���޹��� �� �������� ������ �̺�Ʈ �Ű������� ������,
        // �ڵ鷯 ȣ�� ���� ������ �����մϴ�.
        OnClick?.Invoke(this, new ChooseEventArgs(value, cost));
        if (value <= 7)
        {
            Debug.Log($"{value}�� ������ ŉ��: {items[0]}");
            text1.text = $"{value}�� ������ ŉ��: {items[0]}";
            text3.text += " ��";
        }
        else if (value > 7 && value <= 9)
        {
            Debug.Log($"<color=red>{value}�� ���� ������ ŉ��: {items[1]}</color>");
            text1.text = $"<color=red>{value}�� ���� ������ ŉ��: {items[1]}</color>";
            text3.text += " <color=red>��</color>";
        }
        else
        {
            Debug.Log($"<color=orange>{value}�� ���� ������ ŉ��: {items[2]}</color>");
            text1.text = $"<color=orange>{value}�� ���� ������ ŉ��: {items[2]}</color>";
            text3.text += " <color=orange>��</color>";
        }
        Debug.Log($"���� Ƚ��: {cost}��");
        text2.text = $"���� Ƚ��: {count}";
        
    }

    private void Start()
    {
        //text = gameObject.GetComponent<Text>();
        text1.text = "������ �̱�";
        text2.text = $"���� Ƚ��: {count}";
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
            text2.text = "Ƚ���� �����ϼ̽��ϴ�.";
        }
    }
}
