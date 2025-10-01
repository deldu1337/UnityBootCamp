using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    public void Init()
    {
        UIManager.Instance.EnableGoodsUI(true);
    }

    public void OnClickSettingButton() // 설정 버튼 연결
    {
        Logger.Log($"{GetType()}::OnClickSettingButton");

        var uiData = new BaseUIData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickProfileButton() // 프로필 버튼 연결
    {
        Logger.Log($"{GetType()}::OnClickProfileButton");

        var uiData = new BaseUIData();
        UIManager.Instance.OpenUI<InventoryUI>(uiData);
    }
}
