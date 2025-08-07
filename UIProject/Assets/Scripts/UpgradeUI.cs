using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public Button button01;
    public Text message;
    public Text icon_text;

    //private bool check_items;
    //private string[] need_items;
    //private string[] need_gold;
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
        // ��ư�� onClick ��� �ȿ� OnUpgradeBtnClick �޼ҵ带 �߰��ϴ� �ڵ�
        button01.onClick.AddListener(OnUpgradeBtnClick);
        // AddListener�� ����Ƽ�� UI�� �̺�Ʈ�� ����� �������ִ� �ڵ�
        // ������ �� �ִ� ���� ���°� ������ �־ �� ���´�� ��������� �մϴ�.
        // �ٸ� ���·� ���� ���(�Ű������� �ٸ� ���)��� delegate�� Ȱ���մϴ�.
        // Ư¡) �� ����� ���� �̺�Ʈ�� ����� �����Ѵٸ�
        // ����Ƽ �ν����Ϳ��� ����� ���� Ȯ���� �� �����ϴ�.

        // ����: ���� ������� �ʾƼ� �߸� ���� Ȯ���� �������ϴ�.
        UpdateUI();
        unitStat.SetStat();
        unitStat.UpgradeStat();
        // ���� �� UI�� ���� ������ ����
    }

    // ��ư Ŭ�� �� ȣ���� �޼ҵ� ����
    void OnUpgradeBtnClick()
    {
        if (upgrade < max_level)
        {
            upgrade++;
            UpdateUI();
            unitStat.UpgradeStat();
        }
    }

    private void UpdateUI()
    {
        icon_text.text = $"Wizard+ {upgrade}";
        message.text = materials[upgrade];
    }
}
