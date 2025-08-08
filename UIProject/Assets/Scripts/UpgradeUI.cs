using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Button button01;
    public Text message;
    public Text icon_text;

    public Text STR_text;
    public Text DEX_text;
    public Text INT_text;

    //private bool check_items;
    //private string[] need_items;
    //private string[] need_gold;
    private string job;
    private int STR;
    private int DEX;
    private int INT;

    private string[] jobs;
    private UnitStat unitStat;
    private UnitInventory unitInventory;


    // 자료형[] 배열명 = new 자료형[]
    // {
    //     값1, 값2, 값3
    // };

    // C#에서 배열이 런타임 객체
    // const 배열 대신 사용
    // 
    private string[] materials = new string[]
    {
        "100 골드",
        "100 골드+루비",
        "200 골드+사파이어+마력석",
        "최대 강화 완료"
    };

    // max_level을 사용할 경우 배열의 길이 -1의 값을 가지게 됩니다.
    
    private int upgrade = 0;
    private int max_level => materials.Length - 1;
    // 배열에는 인덱스라는 개념이 존재합니다.
    // ex) materials가 하나의 묶음이고, 거기서 2번째 데이터는 materials[1]입니다.
    //     (카운트를 0부터 셈)
    private void Start()
    {
        unitStat = new UnitStat();
        unitInventory = new UnitInventory();
        
        STR = unitStat.STR;
        DEX = unitStat.DEX;
        INT = unitStat.INT;
        //unitStat.job
        // 버튼에 onClick 기능 안에 OnUpgradeBtnClick 메소드를 추가하는 코드
        button01.onClick.AddListener(OnUpgradeBtnClick);
        // AddListener는 유니티의 UI의 이벤트에 기능을 연결해주는 코드
        // 전달할 수 있는 값의 형태가 정해져 있어서 그 형태대로 설계해줘야 합니다.
        // 다른 형태로 쓰는 경우(매개변수가 다른 경우)라면 delegate를 활용합니다.
        // 특징) 이 기능을 통해 이벤트에 기능을 전달한다면
        // 유니티 인스펙터에서 연결된 것을 확인할 수 없습니다.

        // 장점: 직접 등록하지 않아서 잘못 넣을 확률이 낮아집니다.
        UpdateUI();
        SetStat();
        UpgradeStat();
        // 시작 시 UI에 대한 렌더링 설정
    }

    // 버튼 클릭 시 호출할 메소드 설계
    void OnUpgradeBtnClick()
    {
        if (upgrade < max_level)
        {
            upgrade++;
            UpdateUI();
            UpgradeStat();
        }
    }

    private void UpdateUI()
    {
        icon_text.text = $"Wizard+ {upgrade}";
        message.text = materials[upgrade];
    }

    public void SetStat()
    {
        jobs = icon_text.text.Split("+");
        job = jobs[0];
        if (job == Jobs.Warrior.ToString())
        {
            STR = 6;
            DEX = 3;
            INT = 0;
        }
        else if (job == Jobs.Rogue.ToString())
        {
            STR = 3;
            DEX = 6;
            INT = 0;
        }
        else if (job == Jobs.Wizard.ToString())
        {
            STR = 0;
            DEX = 0;
            INT = 9;
        }

        STR_text.text = $"STR: {STR}";
        DEX_text.text = $"DEX: {DEX}";
        INT_text.text = $"INT: {INT}";
    }

    public void UpgradeStat()
    {
        if (job == Jobs.Warrior.ToString())
        {
            STR += 2;
            DEX += 1;
        }
        else if (job == Jobs.Rogue.ToString())
        {
            STR += 1;
            DEX += 2;
        }
        else if (job == Jobs.Wizard.ToString())
        {
            INT += 3;
        }
        STR_text.text = $"STR: {STR}";
        DEX_text.text = $"DEX: {DEX}";
        INT_text.text = $"INT: {INT}";
    }
}
