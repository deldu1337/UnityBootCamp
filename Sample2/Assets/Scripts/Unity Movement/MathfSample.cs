using Unity.VisualScripting;
using UnityEngine;
// �߿� Ŭ���� Mathf
// ����Ƽ���� ���� �������� �����Ǵ� ��ƿ��Ƽ Ŭ����
// ���� ���߿��� ���� �� �ִ� ���� ���꿡 ���� ���� �޼ҵ�� ����� �����մϴ�.

// ���� �޼ҵ� : static Ű����� ������ �ش� �޼ҵ�� Ŭ������.�޼ҵ��()
//               ���� ����� �����մϴ�.
// ex) Mathf.Abs(-5);

public class MathfSample : MonoBehaviour
{
    float abs = -5;
    float ceil = 4.1f;
    float floor = 4.6f;
    float round = 4.51f;
    float clamp;
    float clamp01;
    float pow = 2;
    float sqrt = 4;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"���� {abs} = " + Mathf.Abs(abs)); // ����(absolute number)
        Debug.Log($"�ø� {ceil} = " + Mathf.Ceil(ceil));           // �ø�(�Ҽ����� ������� ���� �ø� ó���մϴ�.)
        Debug.Log($"���� {floor} = " + Mathf.Floor(floor));          // ����(�Ҽ����� ������� ���� ���� ó���մϴ�.)
        Debug.Log($"�ݿø� {round} = " + Mathf.Round(round));          // �ݿø�(4 ���ϸ� ������ 5 �̻��̸� �ø��ϴ�.)
        Debug.Log($"�� ���� (7, 0, 4) = " + Mathf.Clamp(7, 0, 4));        // ���� ���� ���� �� = 7, �ּ� = 0, �ִ� = 4, ��� = 4
                                                // (��, �ּ�, �ִ�) ������ ���� �Է��մϴ�.
        Debug.Log($"�� ����(1) {5} = " + Mathf.Clamp01(5));            // ���� ���� ���� �� = 5, �ּ� = 0, �ִ� = 1. ��� = 1
                                                // ����� �ּڰ� 0 �Ǵ� �ִ밪 1�� ó��
                                                // �ۼ�Ʈ ������ ���� ó���� �� ���� ���Ǵ� �ڵ�
                                                // �ּ� �ִ� ������ ���� 0�� 1�� �����˴ϴ�.
        // Clamp vs Clamp01
        // Clamp ==> ü��, ����, �ӵ� ���� �ɷ�ġ ���信���� ���� ����
        // Clamp01 ==> ����(�ۼ�Ʈ), ���� ��(1), ���� ��(���򿡼��� ����)
        Debug.Log($"���� {pow} = " + Mathf.Pow(pow, 2));           // (��, ���� ��(����))�� ���� �޾Ƽ� �ش� ���� �ŵ� ���� ���
        Debug.Log($"������ {pow} = " + Mathf.Pow(pow, 0.5f));      // Mathf.Sqrt()�� ����ϴ� �� ���� ������ �ſ� ����
        Debug.Log("������ ������ ��� ���� ���� ���·� ����մϴ�." + Mathf.Pow(pow, -2));          // 2�� -2 ���� => 1/4
        Debug.Log(Mathf.Sqrt(sqrt));            // ���� ���� �޾� �ش� ���� ������(��Ʈ)�� ���
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
