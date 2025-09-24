using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    // 현재씬 가져오기
    public BaseScene CurrentScene { get { return GameObject.FindFirstObjectByType<BaseScene>(); } }

    // LoadScene 의 랩핑 함수 추가적으로 내용 기입할 예정
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(type.ToString());
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
