using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    public void Init()
    {
        UIManager.Instance.EnableGoodsUI(true);
    }

    public void OnClickSettingButton()
    {
        Logger.Log($"{GetType()}::OnClickSettingButton");

        var uiData = new BaseUIData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }
}
