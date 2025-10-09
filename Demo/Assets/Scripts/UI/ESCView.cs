using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCView : MonoBehaviour
{
    [SerializeField] private GameObject escUI;
    private Button LogoutButton;
    private Button SelectCharacterButton;
    private Button ExitGameButton;
    private Button ReturnToGameButton;
    private Button ExitButton;
    private bool show = false;

    void Start()
    {
        UIEscapeStack.GetOrCreate(); // ���� ����

        if (escUI == null)
            escUI = GameObject.Find("escUI");

        if (escUI != null)
        {
            LogoutButton = escUI.transform.GetChild(2).GetComponent<Button>();
            SelectCharacterButton = escUI.transform.GetChild(3).GetComponent<Button>();
            ExitGameButton = escUI.transform.GetChild(4).GetComponent<Button>();
            ReturnToGameButton = escUI.transform.GetChild(5).GetComponent<Button>();
            ExitButton = escUI.transform.GetChild(6).GetComponent<Button>();

            LogoutButton.onClick.AddListener(Logout);
            SelectCharacterButton.onClick.AddListener(SelectCharacter);
            ExitGameButton.onClick.AddListener(ExitGame);
            ReturnToGameButton.onClick.AddListener(ToggleESC);
            ExitButton.onClick.AddListener(ToggleESC);
        }

        if (escUI) escUI.SetActive(show);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // 1) �ֱ� ���� UI���� �ݱ� (������ ���⼭ ��)
            if (UIEscapeStack.Instance != null && UIEscapeStack.Instance.PopTop())
                return;

            // 2) ���� UI�� ���ٸ� ESC �޴� ���
            ToggleESC();
        }
    }

    public void ToggleESC()
    {
        if (!escUI) return;
        show = !show;
        escUI.SetActive(show);
    }

    public void Logout()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void SelectCharacter()
    {
        SceneManager.LoadScene("CharacterScene");
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif   
    }
}
