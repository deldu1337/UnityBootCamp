//using UnityEngine;
//using UnityEngine.UI;

//public class StatusBarUI : MonoBehaviour
//{
//    [SerializeField] private MonoBehaviour hpSource;
//    private PlayerStatsManager playerStats; // IHealth뿐 아니라 PlayerStatsManager 참조
//    private Image hpBar;
//    private Image mpBar;
//    private Image expBar;

//    //void Awake()
//    //{
//    //    // PlayerStatsManager 가져오기
//    //    if (hpSource != null)
//    //        playerStats = hpSource as PlayerStatsManager;

//    //    if (playerStats == null)
//    //        playerStats = GetComponentInParent<PlayerStatsManager>();

//    //    // StatusUI에서 자식 오브젝트 찾아서 참조
//    //    Transform statusUI = GameObject.Find("StatusUI").transform;
//    //    hpBar = statusUI.GetChild(3).GetComponentInChildren<Image>();  // HP Bar
//    //    mpBar = statusUI.GetChild(4).GetComponentInChildren<Image>();  // MP Bar
//    //    expBar = statusUI.GetChild(5).GetComponentInChildren<Image>(); // EXP Bar
//    //}
//    void Awake()
//    {
//        playerStats = PlayerStatsManager.Instance;  // ← 싱글톤
//        if (playerStats == null && hpSource != null)
//            playerStats = hpSource as PlayerStatsManager;

//        Transform statusUI = GameObject.Find("StatusUI").transform;
//        hpBar = statusUI.GetChild(3).GetComponentInChildren<Image>();
//        mpBar = statusUI.GetChild(4).GetComponentInChildren<Image>();
//        expBar = statusUI.GetChild(5).GetComponentInChildren<Image>();
//    }

//    void Update()
//    {
//        UpdateBars();
//    }

//    /// <summary>외부에서 체력/마나/경험치 UI 강제 갱신</summary>
//    public void RefreshStatus()
//    {
//        UpdateBars();
//    }

//    private void UpdateBars()
//    {
//        if (playerStats == null) return;

//        // HP
//        if (hpBar != null)
//        {
//            float maxHp = playerStats.MaxHP > 0 ? playerStats.MaxHP : 1f;
//            hpBar.fillAmount = playerStats.CurrentHP / maxHp;
//        }

//        // MP
//        if (mpBar != null)
//        {
//            float maxMp = playerStats.Data.MaxMP > 0 ? playerStats.Data.MaxMP : 1f;
//            mpBar.fillAmount = playerStats.Data.CurrentMP / maxMp;
//        }

//        // EXP
//        if (expBar != null)
//        {
//            float expRatio = Mathf.Clamp01(playerStats.Data.Exp / playerStats.Data.ExpToNextLevel);
//            expBar.fillAmount = expRatio;
//        }
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image expBar;

    [SerializeField] private string statusUIRootName = "StatusUI";
    [SerializeField] private int hpIndex = 3;   // StatusUI 하위 자식 인덱스
    [SerializeField] private int mpIndex = 4;
    [SerializeField] private int expIndex = 5;

    private PlayerStatsManager playerStats;
    private Coroutine initRoutine;

    private void OnEnable()
    {
        // 초기화 코루틴: 플레이어/UI가 준비될 때까지 대기 후 바인딩
        initRoutine = StartCoroutine(InitializeWhenReady());
    }

    private void OnDisable()
    {
        if (initRoutine != null) { StopCoroutine(initRoutine); initRoutine = null; }
        UnsubscribeEvents();
    }

    private IEnumerator InitializeWhenReady()
    {
        // 1) PlayerStatsManager.Instance가 준비될 때까지 대기
        while (PlayerStatsManager.Instance == null)
            yield return null;

        playerStats = PlayerStatsManager.Instance;

        // 2) StatusUI 루트가 준비될 때까지 대기 (혹시라도 늦게 생성되는 경우)
        Transform statusUI = null;
        while (statusUI == null)
        {
            var go = GameObject.Find(statusUIRootName);
            if (go != null) statusUI = go.transform;
            else yield return null;
        }

        // 3) 바 인스펙터 미지정이면 자동 탐색
        if (hpBar == null && statusUI.childCount > hpIndex)
            hpBar = statusUI.GetChild(hpIndex).GetComponentInChildren<Image>();
        if (mpBar == null && statusUI.childCount > mpIndex)
            mpBar = statusUI.GetChild(mpIndex).GetComponentInChildren<Image>();
        if (expBar == null && statusUI.childCount > expIndex)
            expBar = statusUI.GetChild(expIndex).GetComponentInChildren<Image>();

        // 4) 이벤트 구독 (한 번만)
        SubscribeEvents();

        // 5) 즉시 1회 갱신
        RefreshAll();
    }

    private void SubscribeEvents()
    {
        if (playerStats == null) return;

        // 수치가 변할 때마다 바로바로 반영
        playerStats.OnHPChanged += OnHPChanged;
        playerStats.OnMPChanged += OnMPChanged;
        playerStats.OnExpChanged += OnExpChanged;
        playerStats.OnLevelUp += OnLevelUp; // 레벨업 때도 EXP바/최대치 변화 반영
    }

    private void UnsubscribeEvents()
    {
        if (playerStats == null) return;

        playerStats.OnHPChanged -= OnHPChanged;
        playerStats.OnMPChanged -= OnMPChanged;
        playerStats.OnExpChanged -= OnExpChanged;
        playerStats.OnLevelUp -= OnLevelUp;
    }

    // === 이벤트 핸들러 ===
    private void OnHPChanged(float cur, float max)
    {
        if (hpBar == null) return;
        hpBar.fillAmount = (max > 0f) ? cur / max : 0f;
    }

    private void OnMPChanged(float cur, float max)
    {
        if (mpBar == null) return;
        mpBar.fillAmount = (max > 0f) ? cur / max : 0f;
    }

    private void OnExpChanged(int level, float exp)
    {
        if (expBar == null || playerStats == null || playerStats.Data == null) return;
        float ratio = Mathf.Clamp01(playerStats.Data.Exp / Mathf.Max(1f, playerStats.Data.ExpToNextLevel));
        expBar.fillAmount = ratio;
    }

    private void OnLevelUp(int level)
    {
        // 레벨업 시 최대치가 바뀌므로 전체 재계산
        RefreshAll();
    }

    // 외부에서 강제 갱신하고 싶을 때 호출 가능
    public void RefreshStatus() => RefreshAll();

    private void RefreshAll()
    {
        if (playerStats == null || playerStats.Data == null) return;

        // HP
        if (hpBar != null)
        {
            float maxHp = Mathf.Max(1f, playerStats.MaxHP);
            hpBar.fillAmount = playerStats.CurrentHP / maxHp;
        }

        // MP
        if (mpBar != null)
        {
            float maxMp = Mathf.Max(1f, playerStats.Data.MaxMP);
            mpBar.fillAmount = playerStats.Data.CurrentMP / maxMp;
        }

        // EXP
        if (expBar != null)
        {
            float ratio = Mathf.Clamp01(playerStats.Data.Exp / Mathf.Max(1f, playerStats.Data.ExpToNextLevel));
            expBar.fillAmount = ratio;
        }
    }
}
