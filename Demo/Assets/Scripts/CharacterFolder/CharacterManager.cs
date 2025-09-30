using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject CharacterBackground;
    [SerializeField] private GameObject CharacterPanel;
    [SerializeField] private GameObject CharacterObject;
    [SerializeField] private Button StartButton;
    private Button[] CharacterButtons;
    private Image[] CharacterImages;
    private Image image;
    private int currentIndex = 0;

    void Start()
    {
        CharacterButtons = new Button[8];
        CharacterImages = new Image[8];
        image = CharacterBackground.GetComponent<Image>();
        StartButton.onClick.AddListener(GameStart);

        if (CharacterPanel != null)
        {
            for (int i = 0; i < 8; i++)
            {
                int index = i;  // i�� ���� ������ ����
                CharacterButtons[i] = CharacterPanel.transform.GetChild(i).GetComponent<Button>();
                CharacterButtons[i].onClick.AddListener(() => ChangeCharacter(index));
                CharacterImages[i] = CharacterButtons[i].transform.GetChild(0).GetComponent<Image>();
            }
        }
        CharacterObject.transform.GetChild(0).gameObject.SetActive(true);
        currentIndex = 0;
        ApplySelection(currentIndex); // �⺻ ���� �ݿ�
    }

    //public void ChangeCharacter(int ButtonNum)
    //{
    //    for(int i = 0; i < 8;i++)
    //    {
    //        if (i == ButtonNum)
    //        {
    //            CharacterObject.transform.GetChild(i).gameObject.SetActive(true);
    //            image.sprite = CharacterImages[i].sprite;
    //        }
    //        else
    //        {
    //            CharacterObject.transform.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }
    //    Debug.Log(CharacterObject.transform.GetChild(ButtonNum).gameObject.name);
    //}

    //private void GameStart()
    //{
    //    SceneManager.LoadScene("DungeonScene");
    //}

    public void ChangeCharacter(int ButtonNum)
    {
        for (int i = 0; i < 8; i++)
        {
            bool active = (i == ButtonNum);
            CharacterObject.transform.GetChild(i).gameObject.SetActive(active);
            if (active) image.sprite = CharacterImages[i].sprite;
        }
        ApplySelection(ButtonNum); // ���� �ݿ�
    }

    private void ApplySelection(int index)
    {
        string raceName = CharacterObject.transform.GetChild(index).gameObject.name;
        GameContext.SelectedRace = raceName;              // ���� ����
        Debug.Log($"���õ� ����: {GameContext.SelectedRace}");
    }

    //private void GameStart()
    //{
    //    if (string.IsNullOrEmpty(GameContext.SelectedRace))
    //        GameContext.SelectedRace = CharacterObject.transform.GetChild(0).gameObject.name;

    //    GameContext.IsNewGame = true;                     // �� ���� ����
    //    SceneManager.LoadScene("DungeonScene");
    //}
    private void GameStart()
    {
        var race = string.IsNullOrEmpty(GameContext.SelectedRace)
            ? CharacterObject.transform.GetChild(0).gameObject.name
            : GameContext.SelectedRace;

        // �ش� ���� ���� ���� ���� Ȯ��
        var existing = SaveLoadService.LoadPlayerDataForRaceOrNull(race);
        GameContext.IsNewGame = (existing == null);  // ������ false(�̾��ϱ�), ������ true(�� ����)

        // (����) ������ ����(�����)�� ��ư�� ���� �� ���:
        // GameContext.ForceReset = true; // ����ڰ� ��¥ ����⸦ ���� ����!

        SceneManager.LoadScene("DungeonScene");
    }

}
