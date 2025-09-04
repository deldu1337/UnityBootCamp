using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; // ���� ü��
    public float maxHealth;     // �ִ� ü��

    private bool isDead = false; // ��� ���� üũ

    // ü�� �ʱ�ȭ: �ִ� ü�°� ���� ü���� �����ϰ� ����
    public void SetHp(float amount)
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    // ��� ���� ����
    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
    }

    // ��� ���� ��ȸ
    public bool GetIsDead()
    {
        return isDead;
    }

    [SerializeField] private Image barImage; // UI ü�¹� �̹���

    // HP UI ����
    public void CheckHp()
    {
        if (barImage != null)
            barImage.fillAmount = curHealth / maxHealth; // fillAmount�� ������ ����
    }

    // ü�� ���� ó��
    public void Damage(float damage)
    {
        // �̹� ü���� 0�̰ų� �ִ� ü���� 0�̸� ����
        if (maxHealth == 0 || curHealth <= 0)
            return;

        curHealth -= damage; // ü�� ����
        CheckHp();           // UI ����

        // ü���� 0 ���ϰ� �Ǹ� ��� ó��
        if (curHealth <= 0)
        {
            // Ư�� ���̾�(Enemy)�� isDead�� true�� ����
            int playerLayer = LayerMask.NameToLayer("Enemy"); // Enemy ���̾� �̸�
            if (gameObject.layer == playerLayer)
            {
                isDead = true;
            }
        }
    }

    private void Start()
    {
        // ���� ���� �� ü�� �ʱ�ȭ �� UI ����
        curHealth = maxHealth;
        CheckHp();
    }
}