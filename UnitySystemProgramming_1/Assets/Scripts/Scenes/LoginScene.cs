using System.Collections.Generic;
using UnityEngine;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        // 씬 타입 변경 = 
        SceneType = Define.Scene.Login;

        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            list.Add(Managers.Resource.Instantiate("Player"));
        }

        foreach (GameObject go in list)
        {
            Managers.Resource.Destroy(go);
        }
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
