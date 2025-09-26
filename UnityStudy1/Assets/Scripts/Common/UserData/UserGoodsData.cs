using UnityEngine;

public class UserGoodsData : IUserData
{
    // 유저의 재화
    // Gem : 보석
    public long Gem { get; set; }
    // Gold : 돈
    public long Gold { get; set; }

    public void SetDefaultData()
    {
        Logger.Log($"{GetType()}::SetDefaultData");

        Gem = 0;
        Gold = 0;
    }

    public bool LoadData()
    {
        Logger.Log($"{GetType()}::LoadData");

        bool result = false;

        try
        {
            Gem = long.Parse(PlayerPrefs.GetString("Gem"));
            Gold = long.Parse(PlayerPrefs.GetString("Gold"));
            result = true;

            Logger.Log($"Gem : {Gem}, Gold : {Gold}");
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
            PlayerPrefs.SetString("Gem", Gem.ToString());
            PlayerPrefs.SetString("Gold", Gold.ToString());
            PlayerPrefs.Save();
            result = true;

            Logger.Log($"Gem : {Gem}, Gold : {Gold}");
        }
        catch (System.Exception e)
        {
            Logger.LogError($"Save failed ({e.Message})");
        }

        return result;
    }
}
