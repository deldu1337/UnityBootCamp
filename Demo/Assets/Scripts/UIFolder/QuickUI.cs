using UnityEngine;
using UnityEngine.UI;

public class QuickUI : MonoBehaviour
{
    private Button SkillButton;
    private Button EquipButton;
    private Button InvenButton;

    void Start()
    {
        SkillButton = GameObject.Find("QuickUI").transform.GetChild(0).GetComponent<Button>();
        EquipButton = GameObject.Find("QuickUI").transform.GetChild(1).GetComponent<Button>();
        InvenButton = GameObject.Find("QuickUI").transform.GetChild(2).GetComponent<Button>();
    }

    
}
