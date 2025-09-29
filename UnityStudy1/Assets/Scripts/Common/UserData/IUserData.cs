using UnityEngine;

public interface IUserData
{
    // 기본값으로 데이터 초기화 하는 함수
    void SetDefaultData();

    // 데이터를 로드 하는 함수
    bool LoadData();

    // 데이터를 저장 하는 함수
    bool SaveData();
}
