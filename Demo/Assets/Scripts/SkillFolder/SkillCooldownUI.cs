using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownOverlay; // ��Ÿ�� ����� �̹��� (���� ������ ����)
    private float cooldownTime;
    private float cooldownRemaining;

    void Awake()
    {
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f; // ���� �� ��Ȱ��ȭ
    }

    void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownOverlay != null && cooldownTime > 0f)
                cooldownOverlay.fillAmount = cooldownRemaining / cooldownTime;
        }
        else if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
        }
    }

    /// <summary>
    /// ��ų ��� �� ȣ�� �� ��Ÿ�� ����
    /// </summary>
    public void StartCooldown(float duration)
    {
        cooldownTime = duration;
        cooldownRemaining = duration;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f; // �� �� ������ ����
    }
}
