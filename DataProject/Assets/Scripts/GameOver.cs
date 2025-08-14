using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Button replay;
    public Button exit;
    public Text current_score_text;
    public Text max_score_text;

    private int current_score;
    private int max_score;
    private void Start()
    {
        current_score = PlayerPrefs.GetInt("Score");
        max_score = PlayerPrefs.GetInt("MaxScore");
        replay.onClick.AddListener(GameStart);
        exit.onClick.AddListener(GameExit);

        current_score_text.text = $"ŉ�� ����: {current_score}";
        max_score_text.text = $"ŉ�� ����: {max_score}";
    }

    private void GameStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void GameExit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
//#if UNITY_EDITOR
//        EditorApplication.Exit(0); // ���������� �����մϴ�. (������)
//#else
//        Application.Quit();
//#endif   
    }
}
