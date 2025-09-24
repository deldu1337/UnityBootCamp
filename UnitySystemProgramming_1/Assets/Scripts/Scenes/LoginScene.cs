using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        // �� Ÿ�� ���� = 
        SceneType = Define.Scene.Login;


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //SceneManager.LoadSceneAsync // �񵿱�
            Managers.Scene.LoadScene(Define.Scene.Game); // ����


        }
    }

    public override void Clear()
    {
        Debug.Log("�� �ʱ�ȭ ����!!!!");
    }
}
