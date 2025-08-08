using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// 드롭 다운의 구성 요소
// 1. Template: 드롭 다운이 펼쳐졌을 때, 보이는 리스트 항목
// 2. Caption / Item Text: 현재 선택된 항목 / 리스트 항목 각각에 대한 텍스트
// TMP를 쓰는 경우, 한글 사용을 위해 Label과 Item Label에서 사용 중인 폰트를
// 수정해 주셔야 사용할 수 있습니다.

// 3. Options: 드롭 다운에 표시될 항목에 대한 리스트
//             인스펙터를 통해 작업 등록이 가능합니다.
//             등록하면 바로 리스트에 등록됩니다.

// 4. On Value Changed: 사용자가 항목을 선택했을 때 호출되는 이벤트
//                      인스펙터를 통해 직접 등록할 수 있습니다.
//                      드롭 다운 값에 대한 변경 발생 시 호출됩니다.

public class DropDownPractice : MonoBehaviour
{
    public List<TMP_Dropdown> list_options;
    

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;

    // options에 등록할 값은 문자열

    // 리스트에 값을 넣고 생성하는 방법
    // 리스트<T> 리스트명 = new 리스트명<T> { 요소1, 요소2, 요소3 };
    private TMP_Dropdown job;
    private TMP_Dropdown MaleFemale;
    private TMP_Dropdown team;
    private TMP_Dropdown tribe;
    private List<string> job_options = new List<string> { "전사", "마법사", "도적" };
    private List<string> MaleFemale_options = new List<string> { "남성", "여성" };
    private List<string> team_options = new List<string> { "얼라이언스", "호드" };
    private List<string> tribe_options = new List<string> { "인간", "오크", "나이트엘프", "드워프", "고블린", "트롤" };

    private void Start()
    {
        text1.text = "직업: ";
        text2.text = "성별: ";
        text3.text = "진영: ";
        text4.text = "종족: ";

        list_options.Add(job);
        list_options.Add(MaleFemale);
        list_options.Add(team);
        list_options.Add(tribe);

        for (int i = 0; i < list_options.Count; i++)
            list_options[i].ClearOptions();

        list_options[0].AddOptions(job_options);
        list_options[1].AddOptions(MaleFemale_options);
        list_options[2].AddOptions(team_options);
        list_options[3].AddOptions(tribe_options);

        //for (int i = 0; i < list_options.Count; i++)
        //    list_options[i].onValueChanged.AddListener(onDropDownValueChanged);
        //job.onValueChanged.AddListener(onDropDownValueChanged1);
        //MaleFemale.onValueChanged.AddListener(onDropDownValueChanged2);
        //team.onValueChanged.AddListener(onDropDownValueChanged3);
        //tribe.onValueChanged.AddListener(onDropDownValueChanged4);
        // 이벤트 등록 시 요구하는 함수의 형태대로 작성이 됐다면,
        // 함수의 이름을 넣어 사용할 수 있게 됩니다.

    }

    // C# System.Int32 --> int 동일
    //    System.Int64 --> long 동일
    //    System.UInt32 --> unsigned int (부호가 없는 32비트 정수) ( 0 ~ 4,294,967,295 )
    //void onDropDownValueChanged(int idx, TMP_Dropdown dropdown)
    //{
    //    text1.text = $"직업: {dropdown[].options[idx].text}";
    //}
    void onDropDownValueChanged()
    {
        text1.text = $"직업: {job.options[idx].text}";
    }
    void onDropDownValueChanged1(int idx)
    {
        text1.text = $"직업: {job.options[idx].text}";
    }
    void onDropDownValueChanged2(int idx)
    {
        text2.text = $"성별: {MaleFemale.options[idx].text}";
    }
    void onDropDownValueChanged3(int idx)
    {
        text3.text = $"진영: {team.options[idx].text}";
    }
    void onDropDownValueChanged4(int idx)
    {
        text4.text = $"종족: {tribe.options[idx].text}";
    }



}
