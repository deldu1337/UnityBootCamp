using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title, // 로그인, 정보를 로딩하는 단계의 씬
    Lobby, // 로비씬 
    InGame // 실제 게임이 진행 되는 씬
}

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public void LoadScene(SceneType sceneType)
    {
        Logger.Log($"{sceneType} scene loading...");

        Time.timeScale = 1f; // 씬이동후 타임스케일 초기화
        SceneManager.LoadScene(sceneType.ToString());
    }

    public void ReloadScene()
    {
        Logger.Log($"{SceneManager.GetActiveScene().name} scene loading...");

        Time.timeScale = 1f; // 씬이동후 타임스케일 초기화
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public AsyncOperation LoadSceneAsync(SceneType sceneType)
    {
        Logger.Log($"{sceneType} scene loading...");

        Time.timeScale = 1;
        return SceneManager.LoadSceneAsync(sceneType.ToString());
    }
}
