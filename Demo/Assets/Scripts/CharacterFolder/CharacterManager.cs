using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject CharacterPanel;
    [SerializeField] private GameObject CharacterObject;
    private Button[] CharacterButtons;

    void Start()
    {
        CharacterButtons = new Button[8];

        if (CharacterPanel != null)
        {
            for (int i = 0; i < 8; i++)
            {
                int index = i;  // i 를 로컬 변수에 복사
                CharacterButtons[i] = CharacterPanel.transform.GetChild(i).GetComponent<Button>();
                CharacterButtons[i].onClick.AddListener(() => ChangeCharacter(index));
            }
        }
        CharacterObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ChangeCharacter(int ButtonNum)
    {
        for(int i = 0; i < 8;i++)
        {
            if(i == ButtonNum)
                CharacterObject.transform.GetChild(i).gameObject.SetActive(true);
            else
                CharacterObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        Debug.Log(ButtonNum);
    }
}
