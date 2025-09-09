using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private EnemyStats enemy; // HP�� ǥ���� ��� EnemyStats
    [SerializeField] private Image barImage;   // UI Image ������Ʈ (Fill ���)�� ü�¹� ǥ��

    void Start()
    {
        if (gameObject != null)               // �ش� ������Ʈ�� �����ϸ�
            CheckHp();                        // �ʱ� HP UI ����
    }

    void Update()
    {
        if (gameObject != null)               // �� �����Ӹ��� ������Ʈ�� �����ϸ�
            CheckHp();                        // HP UI ����
    }

    /// <summary> HP UI ���� </summary>
    public void CheckHp()
    {
        if (barImage != null)
            // ���� HP ������ ���� Fill Amount ������Ʈ (0~1)
            barImage.fillAmount = enemy.currentHP / enemy.maxHP;
    }
}