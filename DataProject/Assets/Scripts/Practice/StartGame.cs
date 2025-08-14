using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static JsonMakers;

public class StartGame : MonoBehaviour
{
    public Button new_button;
    public Button load_button;
    public Button exit_button;
    public Button reset_button;

    private Stat loaded;
    private string path;
    private int num;
    private void Start()
    {
        num = PlayerPrefs.GetInt("Num");
        path = Path.Combine(Application.persistentDataPath, "stat.json");
        string json = File.ReadAllText(path);
        loaded = JsonUtility.FromJson<Stat>(json);
        if (loaded.name == "")
        {
            load_button.interactable = false;
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("NextScene");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void ResetGame()
    {
        loaded.name = "";
        loaded.job.warrior = false;
        loaded.job.rogue = false;
        loaded.job.wizard = false;
        loaded.STR = 0;
        loaded.DEX = 0;
        loaded.INT = 0;

        num--;
        PlayerPrefs.SetInt("Num", num);

        PlayerPrefs.Save();

        load_button.interactable = false;

        string reset_json = JsonUtility.ToJson(loaded, true);
        File.WriteAllText(path, reset_json);
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}

//{
//    "name": "",
//    "job": {
//        "warrior": false,
//        "rogue": false,
//        "wizard": false
//    },
//    "tribe": "",
//    "STR": 0,
//    "DEX": 0,
//    "INT": 0
//}
