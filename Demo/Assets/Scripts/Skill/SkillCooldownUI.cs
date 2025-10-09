using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownOverlay; // Inspector ���� or BindOverlay�� ����
    private float cooldownTime;
    private float cooldownRemaining;
    private bool isRunning; // �� �߰�: ������ ī��Ʈ ������

    // �ܺο��� Overlay ����
    public void BindOverlay(Image overlay)
    {
        cooldownOverlay = overlay;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.raycastTarget = false;
            cooldownOverlay.type = Image.Type.Filled;
            cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
            cooldownOverlay.fillOrigin = 2; // Top
            cooldownOverlay.fillClockwise = false;

            // ���� ������ �ƴ϶�� 0���� �ʱ�ȭ (���� ���̸� ����)
            if (!isRunning) cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.enabled = true;
        }
    }

    void Awake()
    {
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        isRunning = false;
        cooldownTime = 0f;
        cooldownRemaining = 0f;
    }

    void Update()
    {
        // ���� ���� �ƴϰų� �������̰� ������ ��ġ���� ����
        if (!isRunning || cooldownOverlay == null) return;

        // �ð� ����
        cooldownRemaining -= Time.deltaTime;

        if (cooldownTime > 0f)
        {
            float t = Mathf.Clamp01(cooldownRemaining / cooldownTime);
            cooldownOverlay.fillAmount = t;
        }

        // �������� �� ���� 0���� ����� ����
        if (cooldownRemaining <= 0f)
        {
            cooldownOverlay.fillAmount = 0f;
            isRunning = false;
        }
    }

    public void StartCooldown(float duration)
    {
        // ���: 0���� ��ٿ��� �ǹ� ������ �׳� ���� ó��
        if (duration <= 0f)
        {
            isRunning = false;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            return;
        }

        cooldownTime = duration;
        cooldownRemaining = duration;
        isRunning = true;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;
    }
}
