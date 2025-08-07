using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; //* ���� ü��
    public float maxHealth; //* �ִ� ü��

    private static int Score = 0;
    [SerializeField] private Text text;
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
            Score += 10;
            text.text = $"Score: {Score}";
        }
    }
    private void Start()
    {
        // �ڵ� �ʱ�ȭ
        curHealth = maxHealth;
        CheckHp();
        text = GameObject.FindWithTag("Score").GetComponent<Text>();
        text.text = $"Score: {Score}";
    }
}
