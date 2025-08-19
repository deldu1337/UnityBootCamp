using UnityEngine;
// �÷��̾�� ���� ����
// �÷��̾�: ��Ʈ���� ����ڰ� �����մϴ�.
// ��: ��Ƽ ������ �ƴ϶��, ���� ������ ��ɿ� ���� �ڵ����� �����̰� �˴ϴ�.

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Down, Chase // �Ʒ��� �������� ����, �÷��̾ �����ϴ� ����
    }
    // �̵� �ӵ�
    public float speed = 5.0f;
    public EnemyType type = EnemyType.Down; // �⺻�����δ� �Ʒ��� �������� ��͸� ����
    Vector3 dir; // ���� ����

    // ���� ���� ����
    private void Start()
    {
        // �Լ� �и�
        // ����: ������ ������
        //       ���뼺�� ������ �� ����(���� ���� ����, ����� ���� ���� ���� ��)
        PatternSetting();
    }

    void PatternSetting()
    {
        int rand = Random.Range(0, 10); // 0 ~ 9������ �� �� �ϳ��� ���� �������� �������ڽ��ϴ�.

        if (rand < 3) // 0, 1, 2 (��ü ���� 10�� �߿� 3���ϱ� 30%)
        {
            type = EnemyType.Chase;
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            dir = target.transform.position - transform.position; // Ÿ�� ��ġ - ���� ��ġ = ����
            dir.Normalize(); // ������ ũ��� 1�� �����մϴ�.
        }
        else
        {
            type = EnemyType.Down;
            // �Ʒ��� �������� ���
            dir = Vector3.down;
        }
    }

    
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    // �浹 �̺�Ʈ ����
    // ������Ʈ�� ������Ʈ ���� �������� �浹 �߻� �� ȣ��˴ϴ�.
    // �� �� �ϳ��� Rigidbody(��ü)�� ������ �־�� ó���˴ϴ�.

    // OnCollisionEnter : �浹 �߻� �� 1���� ȣ��
    // OnCollisionStay : �浹 �����ϴ� ���� ȣ��
    // OnCollisionExit : �浹 �߻� �� �浹 �۾����� ��� ��� 1�� ȣ��

    // Ʈ���ŵ� OnTriggerXXX�� ���� ���� ������ ������ ������ �ֽ��ϴ�.
    // 2D�� ��� OnCollisionEnter2Dó�� �������� 2D�� ����մϴ�.

    // �Ϲ����� ������ �浹 Collision (�������� ���� ���� ��ü�� ȸ���ϰų� �̵���)
    // Is Trigger üũ�� ����� ������Ʈ���� Ʈ���� �浹 Trigger (�浹 ���θ� üũ��)
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject); // ���� �ı�
        Destroy(gameObject); // �ڽ� �ı�
    }
}
