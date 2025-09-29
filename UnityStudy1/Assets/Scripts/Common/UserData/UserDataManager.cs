using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : SingletonBehaviour<UserDataManager>
{
    // 저장된 유저 데이터 존재 여부 확인 변수
    public bool ExistsSaveData {  get; private set; }

    // 모든 유저 데이터 인스턴스를 저장하는 컨테이너
    public List<IUserData> UserDataList { get; private set; } = new List<IUserData>();

    protected override void Init()
    {
        base.Init();

        // 모든 유저 데이터를 UserDataList 에 인스턴스 추가
        UserDataList.Add(new UserSettingsData());
        UserDataList.Add(new UserGoodsData());
    }

    public void SetDefaultUserData()
    {
        foreach (var UserData in UserDataList)
        {
            UserData.SetDefaultData();
        }
    }

    public void LoadUserData()
    {
        ExistsSaveData = PlayerPrefs.GetInt("ExistsSaveData") == 1 ? true : false;

        if (ExistsSaveData)
        {
            foreach (var UserData in UserDataList)
            {
                UserData.LoadData();
            }
        }
    }

    public void SaveUserData()
    {
        bool hasSaveError = false;

        foreach (var UserData in UserDataList)
        {
            bool isSaveSuccess = UserData.SaveData();
            if (isSaveSuccess == false)
            {
                hasSaveError = true;
            }
        }

        if (hasSaveError == false)
        {
            ExistsSaveData = true;
            PlayerPrefs.SetInt("ExistsSaveData", 1);
            PlayerPrefs.Save();
        }
    }
}
