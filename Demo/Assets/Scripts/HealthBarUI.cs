using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private EnemyStats enemy;
    [SerializeField] private Image barImage;

    void Start()
    {
        if (gameObject != null)
            CheckHp();
    }

    void Update()
    {
        if (gameObject != null)
            CheckHp();
    }

    /// <summary> HP UI 갱신 </summary>
    public void CheckHp() //*HP 갱신
    {
        if (barImage != null)
            barImage.fillAmount = enemy.currentHP / enemy.maxHP;
    }
}
