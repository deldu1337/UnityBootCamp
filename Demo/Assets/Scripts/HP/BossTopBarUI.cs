using UnityEngine;

public class BossTopBarUI : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject root;       // �� BossCanvas (ó���� ��Ȱ��)

    [Header("Components")]
    [SerializeField] private HealthBarUI healthBar; // �� BG(HealthBarUI) ����

    private IHealth bossHealth;

    void Awake()
    {
        if (root != null) root.SetActive(false);
        if (healthBar == null) healthBar = GetComponentInChildren<HealthBarUI>(true);
    }

    /// <summary>ǥ���� ���� ����</summary>
    public void SetBoss(IHealth boss)
    {
        bossHealth = boss;

        if (healthBar != null)
        {
            healthBar.SetTargetIHealth(bossHealth);
            healthBar.CheckHp(); // ��� 1ȸ ����
        }

        Show(false); // ���� ���� ����
    }

    public void Show(bool on)
    {
        if (root != null && root.activeSelf != on)
            root.SetActive(on);
    }

    void Update()
    {
        // ���� ���/�Ҹ� �� �ڵ� ����
        if (bossHealth == null || bossHealth.CurrentHP <= 0f)
            Show(false);
    }
}
