using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class Dialog
{
    public string character; // 캐릭터
    public string content; // 대화 텍스트

    // 오른쪽 클릭 -> 빠른 작업 및 리펙토링 -> 멤버에서 생성자 생성
    // 클래스 생성 시 호출되는 메소드(생성자)
    public Dialog(string character, string content)
    {
        // this는 클래스 자신을 의미합니다.
        // 클래스가 가진 값과 매개변수의 이름이 같아서 구분하기 위한 용도
        this.character = character;
        this.content = content;
    }
}

public class DialogManager : MonoBehaviour
{
    #region MonoSingleton
    // 1) 자기 자신에 대한 인스턴스를 가진다.(전역)
    public static DialogManager Instance { get; private set; } // 프로퍼티
    // Instance는 값을 얻어올 수 있습니다.(접근 가능)
    // 수정은 할 수 없습니다.

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; // 해당 인스턴스는 자기 자신입니다.
            DontDestroyOnLoad(gameObject);
            // DDOL은 해당 위치에 있는 오브젝트가 씬이 넘어가도 파괴되지 않게
            // 따로 관리되는 씬의 영역
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Field
    public TMP_Text name;
    public TMP_Text message;
    public GameObject panel;
    public float typing_speed;

    private Queue<Dialog> queue = new Queue<Dialog>();
    private Coroutine typing;
    private bool isTyping = false;
    private Dialog current; // 현재의 대화 내용
    #endregion


    void Start()
    {
        
    }

    void Update()
    {
        // 스페이스 키에 대한 입력이 적용되면
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 이벤트 시스템에 전달된 값이 존재하고, 그 값이 UI 위에서 눌려진 상황이라면?
            if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // 작업 x
                return;
            }

            // 
            if (isTyping) // 타이핑 중이면
            {
                CompleteLine(); // 라인 작업에 대한 마무리
            }
            else
            {
                NextLine(); // 다음 라인으로 이동
            }
        }
    }

    /// <summary>
    /// List<T>나 Queue<T>처럼 열거 형태의 데이터를 다 매개변수로 받을 수 있습니다.
    /// </summary>
    /// <param name="lines">대화 데이터에 대한 열거형 묶음 데이터</param>
    public void StartLine(IEnumerable<Dialog> lines)
    {
        queue.Clear();

        foreach (var line in lines)
        {
            queue.Enqueue(line);
        }
        panel.SetActive(true);
        NextLine();
    }

    // 다음 작업을 위한 함수
    private void NextLine()
    {
        // 작업할 내용이 더 이상 없다면
        if (queue.Count == 0)
        {
            DialogExit(); // 대화의 종료
            return;
        }
        // 큐에 저장된 값을 하나 얻어옵니다.
        current = queue.Dequeue();

        // 현재 대화창의 캐릭터 이름으로 변경
        name.text = current.character;

        // 코루틴이 남아있는 상태라면 멈춰줍니다.
        if (typing != null)
            StopCoroutine(typing);

        // 현재 대화 데이터의 콘텐츠(대화 내용)을 기준으로 대화 타이핑을 시작합니다.
        typing = StartCoroutine(TypingDialog(current.content));
    }

    private IEnumerator TypingDialog(string line)
    {
        isTyping = true; // 타이핑 진행 중임을 알림
        StringBuilder stringBuilder = new StringBuilder(line.Length);
        message.text = ""; // 내용 비우기

        // string(문자열)은 문자(char)의 배열
        foreach(char c in line)
        {
            //message.text += c;
            stringBuilder.Append(c);
            message.text = stringBuilder.ToString();
            yield return new WaitForSeconds(typing_speed);
        }
        isTyping = false;
    }

    private void DialogExit()
    {
        panel.SetActive(false);
    }

    // 기능 처리
    private void CompleteLine()
    {
        if (typing != null)
            StopCoroutine(typing);

        message.text = current.content;
        isTyping = false;
    }
}
