using UnityEngine;

public interface IUserData
{
    // 기본값을 데이터를 초기화 하는 함수
    void SetDefaultData();

    // 데이터를 로드 하는 함수
    bool LoadData();

    // 데이터를 저장 하는 함수
    bool SaveData();

}
