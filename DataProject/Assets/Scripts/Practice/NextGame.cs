using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static JsonMakers;

public class NextGame : MonoBehaviour
{
    public InputField name;
    public Dropdown jobs;
    public Button createButton;

    private Stat loaded;
    private string path;
    private string job;
    private List<string> job_options = new List<string> { "전사", "도적", "마법사" };
    private int num;
    void Start()
    {
        num = PlayerPrefs.GetInt("Num");

        path = Path.Combine(Application.persistentDataPath, "stat.json");
        string json = File.ReadAllText(path);
        loaded = JsonUtility.FromJson<Stat>(json);

        jobs.ClearOptions();
        jobs.AddOptions(job_options);
        jobs.onValueChanged.AddListener(onDropDownValueChanged);
    }

    void onDropDownValueChanged(int idx)
    {
        job = jobs.options[idx].text;
    }

    public void CreateGame()
    {
        //InputName();
        loaded.name = name.text;
        if (job == "전사")
        {
            loaded.job.warrior = true;
            loaded.STR = 10;
            loaded.DEX = 5;
            loaded.INT = 0;
        }
        else if(job == "도적")
        {
            loaded.job.rogue = true;
            loaded.STR = 3;
            loaded.DEX = 12;
            loaded.INT = 0;
        }
        else if(job == "마법사")
        {
            loaded.job.wizard = true;
            loaded.STR = 0;
            loaded.DEX = 0;
            loaded.INT = 15;
        }

        if (num == 1)
        {
            num++;
            PlayerPrefs.SetInt("Num", num);
        }

        PlayerPrefs.Save();

        string create_json = JsonUtility.ToJson(loaded, true);
        File.WriteAllText(path, create_json);

        SceneManager.LoadScene("StartScene");
    }
}
