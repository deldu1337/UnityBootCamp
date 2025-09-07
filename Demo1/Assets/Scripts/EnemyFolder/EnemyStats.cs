using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHP = 50f;             // 적의 최대 체력
    public float currentHP { get; private set; } // 현재 체력 (외부에서 읽기만 가능)

    public float attackPower = 10f;       // 적 공격력
    private ItemDropManager dropManager;   // 적이 죽을 때 아이템 드롭 관리

    void Awake()
    {
        currentHP = maxHP;                // 초기 체력을 최대 체력으로 설정
        dropManager = GetComponent<ItemDropManager>(); // 드롭 매니저 가져오기
    }

    // 적이 데미지를 받음
    public void TakeDamage(float damage)
    {
        currentHP -= damage;               // 체력 감소
        currentHP = Mathf.Max(currentHP, 0); // 체력이 0 아래로 내려가지 않도록 제한
        Debug.Log($"{gameObject.name} HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();                         // 체력이 0이면 사망 처리
            dropManager.DropItems();       // 사망 시 아이템 드롭
        }
    }

    // 적 사망 처리
    private void Die()
    {
        Debug.Log($"{gameObject.name} Died!");
        // 사망 처리 로직 추가 가능
        // 예: DropItem(), Destroy(gameObject), Animation Trigger 등
        Destroy(gameObject);               // 오브젝트 제거
    }

    // HP 회복
    public void Heal(float amount)
    {
        if (currentHP <= 0) return;       // 이미 죽은 경우 회복 불가
        currentHP += amount;               // 체력 회복
        currentHP = Mathf.Min(currentHP, maxHP); // 최대 체력 제한
    }
}