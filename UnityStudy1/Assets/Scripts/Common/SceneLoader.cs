using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title, // �α���, ������ �ε��ϴ� �ܰ��� ��
    Lobby, // �κ�� 
    InGame // ���� ������ ���� �Ǵ� ��
}

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    public void LoadScene(SceneType sceneType)
    {
        Logger.Log($"{sceneType} scene loading...");

        Time.timeScale = 1f; // ���̵��� Ÿ�ӽ����� �ʱ�ȭ
        SceneManager.LoadScene(sceneType.ToString());
    }

    public void ReloadScene()
    {
        Logger.Log($"{SceneManager.GetActiveScene().name} scene loading...");

        Time.timeScale = 1f; // ���̵��� Ÿ�ӽ����� �ʱ�ȭ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public AsyncOperation LoadSceneAsync(SceneType sceneType)
    {
        Logger.Log($"{sceneType} scene loading...");

        Time.timeScale = 1;
        return SceneManager.LoadSceneAsync(sceneType.ToString());
    }
}
