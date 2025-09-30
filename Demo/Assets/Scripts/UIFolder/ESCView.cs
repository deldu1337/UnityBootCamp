using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCView : MonoBehaviour
{
    [SerializeField] private GameObject escUI;       // esc UI 전체
    private Button LogoutButton;
    private Button ExitGameButton;
    private Button ExitButton;      // esc창 닫기 버튼
    private bool show = false;

    void Start()
    {
        if (escUI == null)
            escUI = GameObject.Find("escUI");

        if (escUI != null)
        {
            LogoutButton = escUI.transform.GetChild(2).GetComponent<Button>();
            ExitGameButton = escUI.transform.GetChild(3).GetComponent<Button>();
            ExitButton = escUI.transform.GetChild(4).GetComponent<Button>();

            LogoutButton.onClick.AddListener(Logout);
            ExitGameButton.onClick.AddListener(ExitGame);
            ExitButton.onClick.AddListener(ToggleESC);
        }

        escUI.SetActive(show);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleESC();
        }
    }

    public void ToggleESC()
    {
        if(!show)
        {
            show = true;
            escUI.SetActive(show);
        }
        else
        {
            show = false;
            escUI.SetActive(show);
        }
    }

    public void Logout()
    {
        SceneManager.LoadScene("LoginScene");
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
