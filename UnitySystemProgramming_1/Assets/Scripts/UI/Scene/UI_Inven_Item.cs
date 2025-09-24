using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText
    }

    string _name;

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetGameObject((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;
        //GetGameObject((int)GameObjects.ItemIcon).GetComponent<Image>().sprite = 사용할 이미지;

        GetGameObject((int)GameObjects.ItemIcon).BindEvent((PointerEventData data) => { Debug.Log($"Item click: {_name}"); });
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
