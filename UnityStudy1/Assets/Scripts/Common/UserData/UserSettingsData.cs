using UnityEngine;

public class UserSettingsData : IUserData
{
    // 사용자가 사운드를 사용할지 말지 여부
    public bool Sound {  get; set; }

    // 초기 설정
    public void SetDefaultData()
    {
        Logger.Log($"{GetType()}::SetDefaultData");

        Sound = true;
    }

    public bool LoadData()
    {
        Logger.Log($"{GetType()}::LoadData");

        bool result = false;

        try
        {
            Sound = PlayerPrefs.GetInt("Sound") == 1 ? true : false;
            result = Sound;

            Logger.Log($"Sound : {Sound}");
        }
        catch (System.Exception e)
        {
            Logger.LogError($"Load failed ({e.Message})");
        }

        return result;
    }

    public bool SaveData()
    {
        Logger.Log($"{GetType()}::SaveData");

        bool result = false;

        try
        {
            PlayerPrefs.SetInt("Sound", Sound ? 1 : 0);
            PlayerPrefs.Save(); // PlayerPrefs 에서  Set을 한 이후 반드시 Save 해야 적용됨
            result = true;

            Logger.Log($"Sound : {Sound}");
        }
        catch (System.Exception e)
        {
            Logger.LogError($"Save failed {e.Message}");
        }

        return result;
    }
}
