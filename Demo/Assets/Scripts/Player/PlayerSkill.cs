using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private Animation animationComponent;
    private Quaternion savedRotation;
    private bool isUsingSkill = false;

    void Awake()
    {
        animationComponent = GetComponent<Animation>();
        if (animationComponent == null)
            Debug.LogError("Animation ������Ʈ�� Player ������ �Ǵ� �ڽĿ� �����ϴ�!");
    }

    void Update()
    {
        // AŰ�� ��ų ����
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartWhirlwind();
        }

        // �̵� �Է����� ��ų ���� (����: ��Ŭ�� �̵�)
        if (isUsingSkill && Input.GetMouseButtonDown(1))
        {
            StopWhirlwind();
        }

        // ��ų �߿��� �� ������ ���� ���� ����
        if (isUsingSkill)
        {
            transform.rotation = savedRotation;
        }
    }

    private void StartWhirlwind()
    {
        if (animationComponent == null) return;

        savedRotation = transform.rotation; // ���� ����
        animationComponent.CrossFade("Whirlwind (ID 126 variation 0)", 0.1f);
        isUsingSkill = true;
        Debug.Log(savedRotation);
    }

    private void StopWhirlwind()
    {
        if (!isUsingSkill) return;

        animationComponent.Stop();
        transform.rotation = savedRotation;
        isUsingSkill = false;
    }
}
