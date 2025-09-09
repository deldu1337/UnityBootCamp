using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    protected float curHealth; // 현재 체력
    public float maxHealth;     // 최대 체력

    private bool isDead = false; // 사망 여부 체크

    // 체력 초기화: 최대 체력과 현재 체력을 동일하게 설정
    public void SetHp(float amount)
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    // 사망 상태 설정
    public void SetIsDead(bool isDead)
    {
        this.isDead = isDead;
    }

    // 사망 상태 조회
    public bool GetIsDead()
    {
        return isDead;
    }

    [SerializeField] private Image barImage; // UI 체력바 이미지

    // HP UI 갱신
    public void CheckHp()
    {
        if (barImage != null)
            barImage.fillAmount = curHealth / maxHealth; // fillAmount를 비율로 조정
    }

    // 체력 감소 처리
    public void Damage(float damage)
    {
        // 이미 체력이 0이거나 최대 체력이 0이면 무시
        if (maxHealth == 0 || curHealth <= 0)
            return;

        curHealth -= damage; // 체력 감소
        CheckHp();           // UI 갱신

        // 체력이 0 이하가 되면 사망 처리
        if (curHealth <= 0)
        {
            // 특정 레이어(Enemy)만 isDead를 true로 설정
            int playerLayer = LayerMask.NameToLayer("Enemy"); // Enemy 레이어 이름
            if (gameObject.layer == playerLayer)
            {
                isDead = true;
            }
        }
    }

    private void Start()
    {
        // 게임 시작 시 체력 초기화 및 UI 갱신
        curHealth = maxHealth;
        CheckHp();
    }
}