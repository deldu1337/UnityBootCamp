using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject greidPanel = GetGameObject((int)GameObjects.GridPanel);
        foreach (Transform child in greidPanel.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }

        // 실제하는 아이템의 데이터를 받아서 하나씩 증가처리
        for (int i = 0; i < 8; i++)
        {

            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(greidPanel.transform).gameObject;
            UI_Inven_Item invenItem = item.GetorAddComponent<UI_Inven_Item>();
            invenItem.SetInfo($"철가면{i}번");
        }
    }

    void Update()
    {
        
    }
}
