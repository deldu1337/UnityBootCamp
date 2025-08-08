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


    // �ڷ���[] �迭�� = new �ڷ���[]
    // {
    //     ��1, ��2, ��3
    // };

    // C#���� �迭�� ��Ÿ�� ��ü
    // const �迭 ��� ���
    // 
    private string[] materials = new string[]
    {
        "100 ���",
        "100 ���+���",
        "200 ���+�����̾�+���¼�",
        "�ִ� ��ȭ �Ϸ�"
    };

    // max_level�� ����� ��� �迭�� ���� -1�� ���� ������ �˴ϴ�.
    
    private int upgrade = 0;
    private int max_level => materials.Length - 1;
    // �迭���� �ε������ ������ �����մϴ�.
    // ex) materials�� �ϳ��� �����̰�, �ű⼭ 2��° �����ʹ� materials[1]�Դϴ�.
    //     (ī��Ʈ�� 0���� ��)
    private void Start()
    {
        unitStat = new UnitStat();
        unitInventory = new UnitInventory();
        
        STR = unitStat.STR;
        DEX = unitStat.DEX;
        INT = unitStat.INT;
        //unitStat.job
        // ��ư�� onClick ��� �ȿ� OnUpgradeBtnClick �޼ҵ带 �߰��ϴ� �ڵ�
        button01.onClick.AddListener(OnUpgradeBtnClick);
        // AddListener�� ����Ƽ�� UI�� �̺�Ʈ�� ����� �������ִ� �ڵ�
        // ������ �� �ִ� ���� ���°� ������ �־ �� ���´�� ��������� �մϴ�.
        // �ٸ� ���·� ���� ���(�Ű������� �ٸ� ���)��� delegate�� Ȱ���մϴ�.
        // Ư¡) �� ����� ���� �̺�Ʈ�� ����� �����Ѵٸ�
        // ����Ƽ �ν����Ϳ��� ����� ���� Ȯ���� �� �����ϴ�.

        // ����: ���� ������� �ʾƼ� �߸� ���� Ȯ���� �������ϴ�.
        UpdateUI();
        SetStat();
        UpgradeStat();
        // ���� �� UI�� ���� ������ ����
    }

    // ��ư Ŭ�� �� ȣ���� �޼ҵ� ����
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
