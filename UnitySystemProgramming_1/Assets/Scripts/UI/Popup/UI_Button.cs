using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    // �ڽ� ������Ʈ���� �̸��� ������������ �������
    enum Buttons
    {
        PointButton,
    }

    // �ڽ� ������Ʈ���� �̸��� ������������ �������
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

        // ���� �������� ��������� �� ���ĺ��� ���۳�Ʈ���� ���
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonClicked);
        //GetButton((int)Buttons.PointButton).onClick.AddListener(OnButtonClicked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }

    private int _score = 0;

    public void OnButtonClicked(PointerEventData data) 
    {
        GetText((int)Texts.ScoreText).text = $"���� : {++_score}��";
    }
}
