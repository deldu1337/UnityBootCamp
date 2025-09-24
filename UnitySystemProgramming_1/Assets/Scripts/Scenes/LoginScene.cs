using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        // 씬 타입 변경 = 
        SceneType = Define.Scene.Login;


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //SceneManager.LoadSceneAsync // 비동기
            Managers.Scene.LoadScene(Define.Scene.Game); // 동기


        }
    }

    public override void Clear()
    {
        Debug.Log("씬 초기화 했음!!!!");
    }
}
