using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// ��� �ٿ��� ���� ���
// 1. Template: ��� �ٿ��� �������� ��, ���̴� ����Ʈ �׸�
// 2. Caption / Item Text: ���� ���õ� �׸� / ����Ʈ �׸� ������ ���� �ؽ�Ʈ
// TMP�� ���� ���, �ѱ� ����� ���� Label�� Item Label���� ��� ���� ��Ʈ��
// ������ �ּž� ����� �� �ֽ��ϴ�.

// 3. Options: ��� �ٿ ǥ�õ� �׸� ���� ����Ʈ
//             �ν����͸� ���� �۾� ����� �����մϴ�.
//             ����ϸ� �ٷ� ����Ʈ�� ��ϵ˴ϴ�.

// 4. On Value Changed: ����ڰ� �׸��� �������� �� ȣ��Ǵ� �̺�Ʈ
//                      �ν����͸� ���� ���� ����� �� �ֽ��ϴ�.
//                      ��� �ٿ� ���� ���� ���� �߻� �� ȣ��˴ϴ�.

public class DropDownSample : MonoBehaviour
{
    public TMP_Dropdown job;
    public TMP_Dropdown MaleFemale;
    public TMP_Dropdown team;
    public TMP_Dropdown tribe;

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;

    // options�� ����� ���� ���ڿ�

    // ����Ʈ�� ���� �ְ� �����ϴ� ���
    // ����Ʈ<T> ����Ʈ�� = new ����Ʈ��<T> { ���1, ���2, ���3 };

    private List<string> job_options = new List<string> { "����", "������" ,"����" };
    private List<string> MaleFemale_options = new List<string> { "����", "����" };
    private List<string> team_options = new List<string> { "����̾�", "ȣ��" };
    private List<string> tribe_options = new List<string> { "�ΰ�", "��ũ", "����Ʈ����", "�����", "���", "Ʈ��" };

    private void Start()
    {
        text1.text = $"����: {job_options[0]}";
        text2.text = $"����: {MaleFemale_options[0]}";
        text3.text = $"����: {team_options[0]}";
        text4.text = $"����: {tribe_options[0]}";

        job.ClearOptions(); // ��Ӵٿ��� Option ����� �����ϴ� �ڵ�
        MaleFemale.ClearOptions();
        team.ClearOptions();
        tribe.ClearOptions();

        job.AddOptions(job_options); // �غ�� ��ܿ� ���� �߰�
        MaleFemale.AddOptions(MaleFemale_options);
        team.AddOptions(team_options);
        tribe.AddOptions(tribe_options);

        job.onValueChanged.AddListener(onDropDownValueChanged1);
        MaleFemale.onValueChanged.AddListener(onDropDownValueChanged2);
        team.onValueChanged.AddListener(onDropDownValueChanged3);
        tribe.onValueChanged.AddListener(onDropDownValueChanged4);
        // �̺�Ʈ ��� �� �䱸�ϴ� �Լ��� ���´�� �ۼ��� �ƴٸ�,
        // �Լ��� �̸��� �־� ����� �� �ְ� �˴ϴ�.

    }

    // C# System.Int32 --> int ����
    //    System.Int64 --> long ����
    //    System.UInt32 --> unsigned int (��ȣ�� ���� 32��Ʈ ����) ( 0 ~ 4,294,967,295 )

    void onDropDownValueChanged1(int idx)
    {
        text1.text = $"����: {job.options[idx].text}";
    }
    void onDropDownValueChanged2(int idx)
    {
        text2.text = $"����: {MaleFemale.options[idx].text}";
    }
    void onDropDownValueChanged3(int idx)
    {
        text3.text = $"����: {team.options[idx].text}";
    }
    void onDropDownValueChanged4(int idx)
    {
        text4.text = $"����: {tribe.options[idx].text}";
    }
}
