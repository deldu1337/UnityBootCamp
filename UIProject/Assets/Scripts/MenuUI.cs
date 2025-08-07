using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;

    public GameObject RuleUI;

    private void Start()
    {
        button1.onClick.AddListener(GameStart);
        button2.onClick.AddListener(RuleView);
        button3.onClick.AddListener(GameExit);
    }

    private void GameStart()
    {

    }

    private void RuleView()
    {
        RuleUI.SetActive(true);
    }

    private void GameExit()
    {
#if UNITY_EDITOR
        EditorApplication.Exit(0); // 정상적으로 종료합니다. (에디터)
#else
        Application.Quit();
#endif   
    }
}