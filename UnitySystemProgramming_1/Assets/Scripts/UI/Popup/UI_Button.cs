using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    // 자식 오브젝트들의 이름을 숫자형식으로 들고있음
    enum Buttons
    {
        PointButton,
    }

    // 자식 오브젝트들의 이름을 숫자형식으로 들고있음
    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum Images
    {
        ItemIcon
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 위에 만들어놓은 저장공간에 각 형식별로 컴퍼넌트들을 등록
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }

    private int _score = 0;

    public void OnButtonClicked(PointerEventData data)
    {
        GetText((int)Texts.ScoreText).text = $"점수 : {++_score}점";
    }
}
