using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Header("Fire Setting")]
    [Tooltip("�Ѿ� ���� ����")] public GameObject bulletFactory;
    [Tooltip("�� ���� ����")] public GameObject nuclearFactory;
    [Tooltip("�ѱ�")] public GameObject firePosition;

    public BulletPool pool;

    private void Update()
    {
        // GetKeyXXX : KeyCode�� ��ϵǾ��ִ� Ű �Է�
        // GetButtonXXX : Axes Ű�� ���� �Է�
        // GetMouseButtonXXX : ���콺 Ŭ���� ���� ���� 0,1,2



        // Input Manager�� FIre1 Ű�� ���� �Է��� ����Ǿ��� ��� �߻縦 �����Ѵ�.
        if(Input.GetButtonDown("Fire1"))
        {
            // �Ѿ��� �Ѿ� ���� ���忡�� ����� �Ѿ��� �����Ѵ�.
            // �Ѿ��� ��ġ�� �ѱ� �������� �����Ѵ�.
            // ������ ȸ���� ���� �ʴ´�.
            //var bullet = Instantiate(bulletFactory, firePosition.transform.position, Quaternion.identity);
            var bullet = pool.GetBullet();
            bullet.transform.position = firePosition.transform.position;
        }

        if (Input.GetMouseButtonDown(1) && SkillManager.checkSkill)
        {
            var nuclear = Instantiate(nuclearFactory, firePosition.transform.position, Quaternion.identity);
            SkillManager.checkSkill = false;
        }
    }
}
