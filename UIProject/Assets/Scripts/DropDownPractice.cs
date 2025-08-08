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

public class DropDownPractice : MonoBehaviour
{
    public List<TMP_Dropdown> list_options;
    

    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;
    public TMP_Text text4;

    // options�� ����� ���� ���ڿ�

    // ����Ʈ�� ���� �ְ� �����ϴ� ���
    // ����Ʈ<T> ����Ʈ�� = new ����Ʈ��<T> { ���1, ���2, ���3 };
    private TMP_Dropdown job;
    private TMP_Dropdown MaleFemale;
    private TMP_Dropdown team;
    private TMP_Dropdown tribe;
    private List<string> job_options = new List<string> { "����", "������", "����" };
    private List<string> MaleFemale_options = new List<string> { "����", "����" };
    private List<string> team_options = new List<string> { "����̾�", "ȣ��" };
    private List<string> tribe_options = new List<string> { "�ΰ�", "��ũ", "����Ʈ����", "�����", "���", "Ʈ��" };

    private void Start()
    {
        text1.text = "����: ";
        text2.text = "����: ";
        text3.text = "����: ";
        text4.text = "����: ";

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
        // �̺�Ʈ ��� �� �䱸�ϴ� �Լ��� ���´�� �ۼ��� �ƴٸ�,
        // �Լ��� �̸��� �־� ����� �� �ְ� �˴ϴ�.

    }

    // C# System.Int32 --> int ����
    //    System.Int64 --> long ����
    //    System.UInt32 --> unsigned int (��ȣ�� ���� 32��Ʈ ����) ( 0 ~ 4,294,967,295 )
    //void onDropDownValueChanged(int idx, TMP_Dropdown dropdown)
    //{
    //    text1.text = $"����: {dropdown[].options[idx].text}";
    //}
    void onDropDownValueChanged()
    {
        text1.text = $"����: {job.options[idx].text}";
    }
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
