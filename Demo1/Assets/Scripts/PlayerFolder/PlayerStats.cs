using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHP = 100f;          // 플레이어 최대 체력
    public float currentHP { get; private set; } // 현재 체력 (외부에서는 읽기만 가능)

    public float attackPower = 20f;     // 플레이어 공격력

    void Awake()
    {
        // 게임 시작 시 현재 체력을 최대 체력으로 초기화
        currentHP = maxHP;
    }

    // 플레이어가 피해를 받을 때 호출
    public void TakeDamage(float damage)
    {
        currentHP -= damage;                         // 체력 감소
        currentHP = Mathf.Max(currentHP, 0);         // 체력이 0 미만으로 내려가지 않도록 제한
        Debug.Log($"Player HP: {currentHP}/{maxHP}"); // 현재 체력 출력

        if (currentHP <= 0)
        {
            Die(); // 체력이 0이면 사망 처리
        }
    }

    // 플레이어 사망 처리
    private void Die()
    {
        Debug.Log("Player Died!"); // 사망 로그 출력
        // TODO: 게임 오버, 리스폰 등 사망 관련 로직 추가 가능
    }
    
    // 플레이어 체력 회복
    public void Heal(float amount)
    {
        currentHP += amount;                     // 체력 증가
        currentHP = Mathf.Min(currentHP, maxHP); // 최대 체력 초과하지 않도록 제한
    }
}