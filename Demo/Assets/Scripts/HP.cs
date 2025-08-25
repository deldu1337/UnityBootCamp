using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; //* 현재 체력
    public float maxHealth; //* 최대 체력

    private bool isDead = false;

    public void SetHp(float amount) //*Hp설정
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

    public void CheckHp() //*HP 갱신
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
            //* Layer로 체크
            int playerLayer = LayerMask.NameToLayer("Enemy"); // Enemy 레이어 이름
            if (gameObject.layer == playerLayer)
            {
                isDead = true;
            }
        }
    }

    private void Start()
    {
        // 자동 초기화
        curHealth = maxHealth;
        CheckHp();
    }
}
