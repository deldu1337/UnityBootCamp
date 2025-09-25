using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.UI.ShowSceneUI<UI_Inven>();

        for (int i = 0; i < 10; i++)
        {
            Managers.Resource.Instantiate("Player");
        }

    }


    public override void Clear()
    {

    }
}
