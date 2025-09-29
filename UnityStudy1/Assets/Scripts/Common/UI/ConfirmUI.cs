using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ConfirmType
{
    OK,
    OK_CANCLE,
}

public class ConfirmUIData : BaseUIData
{
    public ConfirmType ConfirmType;
    public string TitleText;
    public string DescText;
    public string OKBtnText;
    public Action OnClickOKBtn;
    public string CancleBtnText;
    public Action OnClickCancelBtn;
}

public class ConfirmUI : BaseUI
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescText;
    public Button OKBtn;
    public Button CancelBtn;
    public TextMeshProUGUI OKBtnText;
    public TextMeshProUGUI CancelBtnText;

    private ConfirmUIData m_ConfirmUIData;
    private Action m_OnClickOKBtn;
    private Action m_OnClickCancelBtn;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        m_ConfirmUIData = uiData as ConfirmUIData;

        TitleText.text = m_ConfirmUIData.TitleText;
        DescText.text = m_ConfirmUIData.DescText;
        OKBtnText.text = m_ConfirmUIData.OKBtnText;
        m_OnClickOKBtn = m_ConfirmUIData.OnClickOKBtn;
        CancelBtnText.text = m_ConfirmUIData.CancleBtnText;
        m_OnClickCancelBtn = m_ConfirmUIData.OnClickCancelBtn;

        OKBtn.gameObject.SetActive(true);
        CancelBtn.gameObject.SetActive(m_ConfirmUIData.ConfirmType == ConfirmType.OK_CANCLE);
    }

    public void OnClickOKBtn()
    {
        m_OnClickOKBtn?.Invoke();
        m_OnClickOKBtn = null;
        CloseUI();
    }

    public void OnClickCancelBtn()
    {
        m_OnClickCancelBtn?.Invoke();
        m_OnClickCancelBtn = null;
        CloseUI();
    }
}
