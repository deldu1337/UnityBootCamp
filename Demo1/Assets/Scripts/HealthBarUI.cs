using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private EnemyStats enemy; // HP를 표시할 대상 EnemyStats
    [SerializeField] private Image barImage;   // UI Image 컴포넌트 (Fill 방식)로 체력바 표시

    void Start()
    {
        if (gameObject != null)               // 해당 오브젝트가 존재하면
            CheckHp();                        // 초기 HP UI 갱신
    }

    void Update()
    {
        if (gameObject != null)               // 매 프레임마다 오브젝트가 존재하면
            CheckHp();                        // HP UI 갱신
    }

    /// <summary> HP UI 갱신 </summary>
    public void CheckHp()
    {
        if (barImage != null)
            // 현재 HP 비율에 따라 Fill Amount 업데이트 (0~1)
            barImage.fillAmount = enemy.currentHP / enemy.maxHP;
    }
}