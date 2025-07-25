using UnityEngine;
using UnityEngine.UI;

public class ObjectController : MonoBehaviour
{
    public GameObject player;
    public Text text;
    private float t;
    private float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        t = 0;
        speed = -0.5f;
        player = GameObject.Find("mini simple skeleton demo");
        text.text = $"{t}";
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);

        // ���࿡ ���Ϲ��� y���� -2���� �۴ٸ� ���Ϲ��� �ı��ϴ� �ڵ�
        if (transform.position.y < -2)
        {
            Destroy(gameObject); // Destroy�� �ʹ� ����� ����� �� ����
                                 // Destroy �ߵ� �� C# ���� ������ �÷���(GC)�� ġ��
        }

        // �浹 ���� ó��
        // ���� ���� �浹 ���� ���� ���
        Vector3 v1 = transform.position; // ���Ϲ� ��ǥ
        Vector3 v2 = player.transform.position; // �÷��̾� ��ǥ

        Vector3 dir = v1 - v2; // v1�� v2 ������ ��ġ

        float d = dir.magnitude; // ������ ũ�� �Ǵ� ���̸� �ǹ��մϴ�.(�� �� ������ �Ÿ��� ����� �� ����մϴ�.)

        float obj_r1 = 0.5f;
        float obj_r2 = 1.0f;

        // �� �� ������ �Ÿ��� d�� ���� ������ �������� �պ��� ũ�ٸ� �浹���� �ʴ� ��Ȳ
        if(d < obj_r1 + obj_r2)
        {
            Destroy(gameObject);
        }

        t += 1 * Time.deltaTime;
        text.text = $"{(int)t}";
    }
}
