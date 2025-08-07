using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; //* 현재 체력
    public float maxHealth; //* 최대 체력

    private static int Score = 0;
    [SerializeField] private Text text;
    public void SetHp(float amount) //*Hp설정
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    [SerializeField] private Image barImage;
    
    public void CheckHp() //*HP 갱신
    {
        if (barImage != null)
            barImage.fillAmount = curHealth / maxHealth;
    }

    public void Damage(float damage) //* 데미지 받는 함수
    {
        if (maxHealth == 0 || curHealth <= 0) //* 이미 체력 0이하면 패스
            return;
        curHealth -= damage;
        CheckHp(); //* 체력 갱신
        if (curHealth <= 0)
        {
            //* 체력이 0 이하라 죽음
            Destroy(gameObject);
            Debug.Log("[HP] 체력 0 이하. 사망 처리 예정.");
            Score += 10;
            text.text = $"Score: {Score}";
        }
    }
    private void Start()
    {
        // 자동 초기화
        curHealth = maxHealth;
        CheckHp();
        text = GameObject.FindWithTag("Score").GetComponent<Text>();
        text.text = $"Score: {Score}";
    }
}
