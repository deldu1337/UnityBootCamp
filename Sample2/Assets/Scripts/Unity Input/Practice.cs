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

    // Text�� Canvas �ȿ� �ִ� ���̱� ������ GetComponent�� �ڽ�(Children)���� ������
    private void Start()
    {
        // �ؽ�Ʈ �迭 �ҷ�����
        texts = GetComponentsInChildren<Text>();

        // ������ ��ȭ �ʱ�ȭ
        str = 0;
        texts[0].text = "�������� Į";

        // ���ݷ� �ʱ�ȭ
        atk = 0;
        texts[1].text = $"���ݷ�: 50";

        // ��ȭ ��ġ �ʱ�ȭ
        cnt = 0;
        texts[2].text = $"��ȭ ��ġ {cnt}/10";

        // ���� Ȯ�� �ʱ�ȭ
        num = 100;
        texts[3].text = $"���� Ȯ��: {num}%";

        // GetComponentInChildren<T>();
        // �� ������Ʈ�� �ڽ����κ��� ������Ʈ�� ������ ���
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter Ű
        {
            rand = Random.Range(1, 101);
            if (rand <= num)
            {
                if (cnt < 3)
                {
                    // ������ ��ȭ
                    str++;
                    texts[0].text = $"�������� Į ( +{str} )";
                }
                else if (cnt >=3 && cnt < 6)
                {
                    // ������ ��ȭ
                    str++;
                    texts[0].text = $"�����ִ� Į ( +{str} )";
                }
                else if (cnt >= 6 && cnt < 8)
                {
                    // ������ ��ȭ
                    str++;
                    texts[0].text = $"����� Į ( +{str} )";
                }
                else if (cnt >= 8)
                {
                    // ������ ��ȭ
                    str++;
                    texts[0].text = $"�� �� Į ( +{str} )";
                }
                //// ������ ��ȭ
                //str++;
                //texts[0].text = $"�������� Į ( +{str} )";

                // ���ݷ� ����
                atk += 5;
                texts[1].text = $"���ݷ�: 50(+{atk})";

                // ��ȭ ��ġ
                cnt++;
                texts[2].text = $"��ȭ ��ġ {cnt}/10";

                // ���� Ȯ��
                num -= 10;
                texts[3].text = $"���� Ȯ��: {num}%";
            }
        }
    }
}
