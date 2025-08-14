using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Practice : MonoBehaviour
{
    public Text score_text;
    public Text max_score_text;
    public Text time_text;
    public Button click;

    private int score;
    private int max_score;
    private int time;

    //private RaycastHit hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(transform.position, transform.forward);

        score = PlayerPrefs.GetInt("Score", score);
        max_score = PlayerPrefs.GetInt("MaxScore", 0);
        score = 0;
        time = 60;
        score_text.text = $"Score: {score}";
        max_score_text.text = $"Max Score: {max_score}";
        time_text.text = $"{time}";
        StartCoroutine(timer());
    }

    public IEnumerator timer()
    {
        while (time > 0)
        {
            time -= 1;
            if(time <= 10)
                time_text.text = $"{time}";
            time_text.text = $"{time}";
            yield return new WaitForSeconds(1);
        }
        if(score > max_score)
        {
            PlayerPrefs.SetInt("MaxScore", score);
            PlayerPrefs.Save();
        }
        PlayerPrefs.SetInt("Score", score);
        SceneManager.LoadScene("GameOver");
    }

    public void OnButtonClicked()
    {
        score++;
        score_text.text = $"Score: {score}";
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 10;
        //if (Physics.Raycast(transform.position, transform.forward))
        //    Debug.Log("Hit");
    }
}
