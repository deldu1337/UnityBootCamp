using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public Text clearText;
    public Button quitButton;

    public static int count;
    
    void Start()
    {
        count = 0;
        clearText.text = "<color=green>Clear</color>";
        clearText.enabled = false;
        quitButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if(count >= 20)
        {
            clearText.enabled = true;
            quitButton.gameObject.SetActive(true);
        }
        else if(count == -1)
        {
            clearText.text = "<color=red>Failed</color>";
            clearText.enabled = true;
            quitButton.gameObject.SetActive(true);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
