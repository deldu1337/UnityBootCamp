using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    [SerializeField] private MonoBehaviour hpSource;
    private PlayerStatsManager playerStats; // IHealth뿐 아니라 PlayerStatsManager 참조
    private Image hpBar;
    private Image mpBar;
    private Image expBar;

    void Awake()
    {
        // PlayerStatsManager 가져오기
        if (hpSource != null)
            playerStats = hpSource as PlayerStatsManager;

        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerStatsManager>();

        // StatusUI에서 자식 오브젝트 찾아서 참조
        Transform statusUI = GameObject.Find("StatusUI").transform;
        hpBar = statusUI.GetChild(3).GetComponentInChildren<Image>();  // HP Bar
        mpBar = statusUI.GetChild(4).GetComponentInChildren<Image>();  // MP Bar
        expBar = statusUI.GetChild(5).GetComponentInChildren<Image>(); // EXP Bar
    }

    void Update()
    {
        UpdateBars();
    }

    /// <summary>외부에서 체력/마나/경험치 UI 강제 갱신</summary>
    public void RefreshStatus()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (playerStats == null) return;

        // HP
        if (hpBar != null)
        {
            float maxHp = playerStats.MaxHP > 0 ? playerStats.MaxHP : 1f;
            hpBar.fillAmount = playerStats.CurrentHP / maxHp;
        }

        // MP
        if (mpBar != null)
        {
            float maxMp = playerStats.Data.MaxMP > 0 ? playerStats.Data.MaxMP : 1f;
            mpBar.fillAmount = playerStats.Data.CurrentMP / maxMp;
        }

        // EXP
        if (expBar != null)
        {
            float expRatio = Mathf.Clamp01(playerStats.Data.Exp / playerStats.Data.ExpToNextLevel);
            expBar.fillAmount = expRatio;
        }
    }
}
