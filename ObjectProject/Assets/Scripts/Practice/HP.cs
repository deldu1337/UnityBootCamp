using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; //* ���� ü��
    public float maxHealth; //* �ִ� ü��

    private static int Score = 0;
    [SerializeField] private Text text;
    public Text gameOver;
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
            if (gameObject.CompareTag("Player"))
            {
                gameOver.text = "<color=red>Game Over</color>";
                gameOver.gameObject.SetActive(true);
            }
            else
                text.text = $"Score: {Score}";
            Destroy(gameObject);
            Debug.Log("[HP] ü�� 0 ����. ��� ó�� ����.");
            Score += 10;
        }
    }
    private void Start()
    {
        // �ڵ� �ʱ�ȭ
        curHealth = maxHealth;
        CheckHp();
        text = GameObject.FindWithTag("Score").GetComponent<Text>();
        text.text = $"Score: {Score}";
        gameOver.gameObject.SetActive(false);
    }
}
