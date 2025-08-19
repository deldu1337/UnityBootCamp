using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSample : MonoBehaviour
{
    public Text quizText;
    public Text answerText;
    public InputField answer;
    public AnswerSample quizs;
    public Button nextButton;

    private int count;

    private void Start()
    {
        count = 0;
        answerText.text = "";
        nextButton.interactable = false;
        quizText.text = $"{quizs.quiz[count]}";
    }
    public void BackTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void NextStage()
    {
        nextButton.interactable = false;
        count++;
        quizText.text = $"{quizs.quiz[count]}";
        answerText.text = "";
        answer.text = "";
    }

    public void OnEnterAnswer()
    {
        if(answer.text == quizs.answer[count])
        {
            answerText.text = $"<color=blue>{quizs.answer[count]} </color><color=green>정답!</color>";
            nextButton.interactable = true;
        }
        else
        {
            answerText.text = "<color=red>오답!</color>";
        }
    }

    private void Update()
    {
        //quizText.text = $"{quizs.quiz[count]}";
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnEnterAnswer();
        }
    }
}
