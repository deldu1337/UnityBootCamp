using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; //* ���� ü��
    public float maxHealth; //* �ִ� ü��
    public void SetHp(float amount) //*Hp����
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    [SerializeField] private Image barImage;
    
    public void CheckHp() //*HP ����
    {
        if (barImage != null)
            barImage.fillAmount = curHealth / maxHealth;
    }

    public void Damage(float damage) //* ������ �޴� �Լ�
    {
        if (maxHealth == 0 || curHealth <= 0) //* �̹� ü�� 0���ϸ� �н�
            return;
        curHealth -= damage;
        CheckHp(); //* ü�� ����
        if (curHealth <= 0)
        {
            //* ü���� 0 ���϶� ����
            Destroy(gameObject);
            Debug.Log("[HP] ü�� 0 ����. ��� ó�� ����.");
        }
    }
    private void Start()
    {
        // �ڵ� �ʱ�ȭ
        curHealth = maxHealth;
        CheckHp();
    }
}
