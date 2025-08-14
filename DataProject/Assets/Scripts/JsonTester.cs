using System;
using UnityEngine;

public class JsonTester : MonoBehaviour
{
    // 유니티에서 객체(Object)의 필드(Field)를 json으로 변환하기 위해서는
    // 내부적으로 메모리에서 데이터를 읽고 쓰는 작업이 가능해야 함
    // 따라서 [Serializable] 속성을 추가해 해당 정보에 대한 직렬화를 처리해 줄 필요가 있습니다.

    // 직렬화는 데이터를 저장하거나 전송하기 위해 연속적인 데이터의 형태로 변형해주는
    // 작업을 의미합니다.

    [Serializable]
    public class Data
    {
        public int hp;
        public int atk;
        public int def;
        public string[] item;
        public Position position;
        public string Quest;
        public bool isDead;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
    }

    public Data my_data;

    // Use this for initiaization
    void Start()
    {
        var jsonText = Resources.Load<TextAsset>("data01");

        if(jsonText == null)
        {
            Debug.LogError("해당 JSON 파일을 리소스 폴더에서 찾지 못했습니다!");
            return;
        }

        // Json 문자열을 객체의 형태로 변환합니다.
        my_data = JsonUtility.FromJson<Data>(jsonText.text);

        Debug.Log(my_data.hp);
        Debug.Log(my_data.atk);
        Debug.Log(my_data.def);
        Debug.Log(my_data.position.x);
        Debug.Log(my_data.position.y);
        Debug.Log(my_data.Quest);
        Debug.Log(my_data.isDead);

        foreach (var item in my_data.item)
            Debug.Log(item);
    }

    void Update()
    {
        
    }
}
