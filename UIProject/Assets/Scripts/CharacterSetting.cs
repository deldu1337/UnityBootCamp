using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterSetting : MonoBehaviour
{
    public List<Dropdown> dropdown_lst;
    public Text message; //전달할 메세지

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Dropdown dropdown in dropdown_lst)
        {
            dropdown.ClearOptions();
            dropdown.onValueChanged.AddListener(onDropDownValueChanged);
            dropdown.AddOptions(dropdown.GetComponent<DropDownData>().menu);
        }
        message.text = "";
    }

    void onDropDownValueChanged(int idx)
    {
        foreach (Dropdown dropdown in dropdown_lst)
        {
            var data = dropdown.GetComponent<DropDownData>();
        }
    }
}
