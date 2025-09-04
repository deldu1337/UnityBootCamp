using System;
using System.Collections.Generic;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset; // TextAsset 타입 명시

// JSON에서 아이템 배열을 읽기 위한 래퍼 클래스
[Serializable]
public class ItemDataArray
{
    public ItemData[] items; // ItemData 객체 배열
}

// 게임 전체에서 아이템 데이터를 관리하는 싱글톤 클래스
public class DataManager : MonoBehaviour
{
    private static DataManager instance; // 싱글톤 인스턴스
    public Dictionary<int, ItemData> dicItemDatas; // ID 기반으로 ItemData를 저장
    private DataManager() { } // 외부에서 생성 방지 (싱글톤)

    // 싱글톤 인스턴스를 반환
    public static DataManager GetInstance()
    {
        if (DataManager.instance == null)
            DataManager.instance = new DataManager();
        return DataManager.instance;
    }

    // Resources 폴더 내 JSON 파일에서 아이템 데이터를 로드
    public void LoadDatas()
    {
        // Resources/Datas/itemData.json 파일 로드
        TextAsset textAsset = Resources.Load<TextAsset>("Datas/itemData");
        if (textAsset == null)
        {
            Debug.LogError("Resources/Datas/itemData.json 파일을 확인하세요!");
            return;
        }
        var json = textAsset.text; // JSON 문자열 추출

        // JSON 배열을 래퍼(ItemDataArray)로 감싸서 파싱
        ItemDataArray wrapper = JsonUtility.FromJson<ItemDataArray>(json);
        if (wrapper == null || wrapper.items == null)
        {
            Debug.LogError("JSON 파싱 실패! JSON 형식 확인 필요.");
            return;
        }

        // ID를 키로, ItemData를 값으로 하는 딕셔너리 초기화
        dicItemDatas = new Dictionary<int, ItemData>();

        // 각 아이템 데이터를 딕셔너리에 추가
        foreach (var data in wrapper.items)
        {
            dicItemDatas[data.id] = data; // 아이템 ID 기준으로 저장
            Debug.LogFormat($"{data.id}, {data.name}, {data.atk}"); // 로드 확인용 로그
        }
    }
}