using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneUI : MonoBehaviour
{
    public Button LoginButton;
    public Button QuitButton;
    public Sprite pressedSprite;

    private void Start()
    {
        LoginButton.onClick.AddListener(GameStart);
        QuitButton.onClick.AddListener(GameExit);
    }

    private void GameStart()
    {
        SceneManager.LoadScene("DungeonScene");
    }

    // ���� ���� �Լ��� ��ó�� ���(#if, #else, #endif)
    private void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif   
    }
}