using System;
using System.IO;
using UnityEngine;

public class JsonMaker : MonoBehaviour
{
    [Serializable]
    public class QuestData
    {
        public string quest_name;
        public string reward;
        public string description;
    }

    [Serializable]
    public class QuestList
    {
        public QuestData[] quests;
    }
    
    private void Start()
    {
        // 1) 설계한 객체에 대한 초기화 작업
        QuestList list = new QuestList()
        {
            quests = new QuestData[]
            {
                // new 생성자명() { 필드명1 = 값1, 필드명2 = 값2, ... } 해당 형태의 값을 가진 클래스 객체가 생성됩니다.

                new QuestData() { quest_name = "시작이 반이다.", reward = "exp + 100", description = "시작이라도 하면 반이라도 가지." },

                new QuestData() { quest_name = "중간만 해라.", reward = "exp + 150", description = "중간이라도 하는게 어디야." },

                new QuestData() { quest_name = "할거면 끝까지 해라.", reward = "exp + 500", description = "그냥 다 하라는거잖아;;" }
            }
        };

        // 2) JsonUtility.ToJson(Object, pretty_print);를 통해 C#의 객체를 JSON으로 변경해주는 직렬화 기능을 가진 함수
        string json = JsonUtility.ToJson(list, true);
        // ToJson(객체)는 해당 객체를 JSON 문자열로 변경해주는 코드
        // true를 추가할 경우, 들여쓰기와 줄바꿈이 포함된 형식의 json 파일로 만들어줍니다.
        // false를 쓰거나, 생략하는 경우라면 전부 한 줄로 압축되면 json 파일로 설정됩니다.

        // 3) 저장 경로에 대한 작성을 진행합니다.
        Debug.Log($"현 저장 폴더 위치: {Application.persistentDataPath}");
        string path = Path.Combine(Application.persistentDataPath, "quests.json");
        // Path.Combine(string path1, string path2);
        // 두 경로의 문자열을 하나의 경로로 만들어주는 기능을 가지고 있습니다.
        // 저장 위치/파일명으로 자주 사용합니다.

        // Application.persistentDataPath: 유니티가 각 플랫폼마다 제공하는 영구 저장 가능한 폴더 경로

        // 4) 해당 경로에 파일을 작성
        File.WriteAllText(path, json);
        // C# 723 page: System.IO 네임스페이스
        //    725 page: Path 클래스를 통해 파일 이름, 확장자, 폴더 정보 얻는 방법
        //    733 page: Json 데이터에 대한 설명

        Debug.Log("JSON 파일 저장 완료");

        // ==================== 파일 로드 ====================
        // 1) 해당 경로에 파일이 존재하는지 판단하세요.

        if(File.Exists(path))
        {
            // 파일 텍스트를 전부 읽어서 문자형 데이터로 변경합니다.
            string json2 = File.ReadAllText(path);

            QuestList loaded = JsonUtility.FromJson<QuestList>(json2);

            Debug.Log($"퀘스트 수락: {loaded.quests[0].quest_name}");
        }
        else
        {
            Debug.LogWarning("해당 경로에 저장된 JSON 파일이 존재하지 않습니다.");
        }
    }
}
