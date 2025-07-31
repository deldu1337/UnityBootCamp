using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 키를 입력하면 텍스트에 특정 메세지가 나오도록 하는 코드
public class LegacyExample : MonoBehaviour
{
    public Text text;

    KeyCode key;

    // Text는 Canvas 안에 있는 값이기 때문에 GetComponent의 자식(Children)으로 가져옴
    private void Start()
    {
        text = GetComponentInChildren<Text>();
        // GetComponentInChildren<T>();
        // 현 오브젝트의 자식으로부터 컴포넌트를 얻어오는 기능
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space)) // Space 키
        //{
        //    text.text = "pata";
        //}

        //if (Input.GetKeyDown(KeyCode.Return)) // Enter 키
        //{
        //    text.text = "pong";
        //}

        //if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키
        //{
        //    text.text = "";
        //}

        // 배열과 같은 묶음으로 관리되는 데이터를 순차적으로 조사하는 코드
        // foreach(데이터 변수명 in 묶음)
        // {
        //
        // }

        // KeyCode 형태의 데이터 전체를 조사합니다.
        foreach(KeyCode Key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(Key))
            {
                switch (Key)
                {
                    case KeyCode.Escape:
                        text.text = "";
                        break;
                    case KeyCode.Space:
                        text.text = "pata";
                        break;
                    case KeyCode.Return:
                        text.text = "pong";
                        break;
                }
            }
        }
    }
}
