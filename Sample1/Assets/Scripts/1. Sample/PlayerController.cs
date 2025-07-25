using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    public Text text;
    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        text.text = $"SCORE : {score}";
        speed = 5;
        rb = GetComponent<Rigidbody>();
        // GetComponent<T>() : ���� ������Ʈ�� �پ��ִ� ������Ʈ�� �������� ����Դϴ�.
        // T : Type
        Debug.Log("������ �Ϸ�Ǿ����ϴ�!");
    }

    // Update is called once per frame
    void Update()
    {
        //speed += 1;
        //Debug.Log($"�ӵ��� 1 �����մϴ�. ���� �ӵ��� {speed}�Դϴ�.");

        // ���� �̵�
        float horizontal = Input.GetAxis("Horizontal");

        // ���� �̵�
        float vertical = Input.GetAxis("Vertical");

        // �̵� ��ǥ(����) ����
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹ü�� ���� ������Ʈ�� �±װ� ItemBox���?
        if(other.gameObject.CompareTag("Itembox"))
        {
            Debug.Log("������ ȹ��!");
            // �浹ü�� ���� ������Ʈ ��Ȱ��ȭ
            score += 10;
            text.text = $"SCORE : {score}";
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("obstacle"))
        {
            speed -= 1;
            Debug.Log("�ӵ� 1 ����");
            // �浹ü�� ���� ������Ʈ ��Ȱ��ȭ
            other.gameObject.SetActive(false);
        }
    }
}
