using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public Text bestText;

    private int score;
    private int best;

    public static ScoreManager Instance;
    //public BestScore bestScore;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        score = 0;
    }

    private void Start()
    {
        score = 0;
        best = GetMaxScore();
        scoreText.text = $"Score: {score}";
        bestText.text = $"Best: {best}";
    }

    public void SetScore(int value)
    {
        score += value; // 전달 받은 값 만큼 점수를 증가시킨다.
        SetScoreText(score);

        if(score >= best)
        {
            SetMaxScore(score);
            best = GetMaxScore();
            SetBestText(best);
        }
    }

    public int GetMaxScore()
    {
        int maxScore = PlayerPrefs.GetInt("MaxScore", 0);
        if(maxScore == 0)
            PlayerPrefs.SetInt("MaxScore", 0);

        PlayerPrefs.Save(); // 스크립트에 의한 저장을 강제로 호출합니다.
        return maxScore;
    }

    public void SetMaxScore(int value)
    {
        PlayerPrefs.SetInt("MaxScore", value);
        PlayerPrefs.Save();
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private void SetScoreText(int value) => scoreText.text = $"Score: {score}";

    private void SetBestText(int value) => bestText.text = $"Best: {best}";

    public int GetScore() => score;
}
