using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; //* ���� ü��
    public float maxHealth; //* �ִ� ü��

    private bool isDead = false;

    public void SetHp(float amount) //*Hp����
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
    }

    public bool GetIsDead()
    {
        return isDead;
    }


    [SerializeField] private Image barImage;

    public void CheckHp() //*HP ����
    {
        if (barImage != null)
            barImage.fillAmount = curHealth / maxHealth;
    }

    public void Damage(float damage)
    {
        if (maxHealth == 0 || curHealth <= 0)
            return;

        curHealth -= damage;
        CheckHp();

        if (curHealth <= 0)
        {
            //* Layer�� üũ
            int playerLayer = LayerMask.NameToLayer("Enemy"); // Enemy ���̾� �̸�
            if (gameObject.layer == playerLayer)
            {
                isDead = true;
            }
        }
    }

    private void Start()
    {
        // �ڵ� �ʱ�ȭ
        curHealth = maxHealth;
        CheckHp();
    }
}
