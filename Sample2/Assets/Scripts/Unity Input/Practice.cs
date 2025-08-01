using UnityEngine;
using UnityEngine.UI;

public class Practice : MonoBehaviour
{
    public Text[] texts;
    public int rand;
    public int str;
    public int atk;
    public int cnt;
    public int num;

    KeyCode key;

    // Text는 Canvas 안에 있는 값이기 때문에 GetComponent의 자식(Children)으로 가져옴
    private void Start()
    {
        // 텍스트 배열 불러오기
        texts = GetComponentsInChildren<Text>();

        // 아이템 강화 초기화
        str = 0;
        texts[0].text = "쓸데없는 칼";

        // 공격력 초기화
        atk = 0;
        texts[1].text = $"공격력: 50";

        // 강화 수치 초기화
        cnt = 0;
        texts[2].text = $"강화 수치 {cnt}/10";

        // 성공 확률 초기화
        num = 100;
        texts[3].text = $"성공 확률: {num}%";

        // GetComponentInChildren<T>();
        // 현 오브젝트의 자식으로부터 컴포넌트를 얻어오는 기능
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter 키
        {
            rand = Random.Range(1, 101);
            if (rand <= num)
            {
                if (cnt < 3)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"쓸데없는 칼 ( +{str} )";
                }
                else if (cnt >=3 && cnt < 6)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"쓸모있는 칼 ( +{str} )";
                }
                else if (cnt >= 6 && cnt < 8)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"용사의 칼 ( +{str} )";
                }
                else if (cnt >= 8)
                {
                    // 아이템 강화
                    str++;
                    texts[0].text = $"개 쎈 칼 ( +{str} )";
                }
                //// 아이템 강화
                //str++;
                //texts[0].text = $"쓸데없는 칼 ( +{str} )";

                // 공격력 증가
                atk += 5;
                texts[1].text = $"공격력: 50(+{atk})";

                // 강화 수치
                cnt++;
                texts[2].text = $"강화 수치 {cnt}/10";

                // 성공 확률
                num -= 10;
                texts[3].text = $"성공 확률: {num}%";
            }
        }
    }
}
