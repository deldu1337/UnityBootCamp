using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownOverlay; // 쿨타임 덮어씌울 이미지 (검은 반투명 원형)
    private float cooldownTime;
    private float cooldownRemaining;

    void Awake()
    {
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f; // 시작 시 비활성화
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
    /// 스킬 사용 시 호출 → 쿨타임 시작
    /// </summary>
    public void StartCooldown(float duration)
    {
        cooldownTime = duration;
        cooldownRemaining = duration;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f; // 꽉 찬 원에서 시작
    }
}
