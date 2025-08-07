using UnityEngine;

public class Sample : MonoBehaviour
{
    string items = "빨간 포션+파란 포션+주황 포션";
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
