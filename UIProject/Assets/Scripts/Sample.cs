using UnityEngine;

public class Sample : MonoBehaviour
{
    string items = "���� ����+�Ķ� ����+��Ȳ ����";
    string[] item_table;
    void Start()
    {
        item_table = items.Split('+');

        foreach (string item in item_table)
        {
            Debug.Log(item);
        }
    }

    void Update()
    {

    }
}
