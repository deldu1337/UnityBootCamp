using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private MonoBehaviour hpSource;
    private IHealth hasHP;
    [SerializeField] private Image barImage;

    void Awake()
    {
        if (hpSource != null) hasHP = hpSource as IHealth;
        if (hasHP == null) hasHP = GetComponentInParent<IHealth>();
        if (barImage == null) barImage = GetComponentInChildren<Image>();
    }

    void Update()
    {
        UpdateBar();
    }

    /// <summary> �ܺο��� ü�¹ٸ� ������ �����ϰ� ���� �� ȣ�� </summary>
    public void CheckHp()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (hasHP == null || barImage == null) return;
        float maxHp = hasHP.MaxHP > 0 ? hasHP.MaxHP : 1f;
        barImage.fillAmount = hasHP.CurrentHP / maxHp;
    }

    public void SetTarget(MonoBehaviour newSource)
    {
        hpSource = newSource;
        hasHP = hpSource as IHealth;
    }
    public void SetTargetIHealth(IHealth newTarget)
    {
        hasHP = newTarget;
    }
}
