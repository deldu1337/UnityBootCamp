//using UnityEngine;
//using UnityEngine.UI;

//public class StatusBarUI : MonoBehaviour
//{
//    [SerializeField] private MonoBehaviour hpSource;
//    private PlayerStatsManager playerStats; // IHealth�� �ƴ϶� PlayerStatsManager ����
//    private Image hpBar;
//    private Image mpBar;
//    private Image expBar;

//    //void Awake()
//    //{
//    //    // PlayerStatsManager ��������
//    //    if (hpSource != null)
//    //        playerStats = hpSource as PlayerStatsManager;

//    //    if (playerStats == null)
//    //        playerStats = GetComponentInParent<PlayerStatsManager>();

//    //    // StatusUI���� �ڽ� ������Ʈ ã�Ƽ� ����
//    //    Transform statusUI = GameObject.Find("StatusUI").transform;
//    //    hpBar = statusUI.GetChild(3).GetComponentInChildren<Image>();  // HP Bar
//    //    mpBar = statusUI.GetChild(4).GetComponentInChildren<Image>();  // MP Bar
//    //    expBar = statusUI.GetChild(5).GetComponentInChildren<Image>(); // EXP Bar
//    //}
//    void Awake()
//    {
//        playerStats = PlayerStatsManager.Instance;  // �� �̱���
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

//    /// <summary>�ܺο��� ü��/����/����ġ UI ���� ����</summary>
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
    [SerializeField] private int hpIndex = 3;   // StatusUI ���� �ڽ� �ε���
    [SerializeField] private int mpIndex = 4;
    [SerializeField] private int expIndex = 5;

    private PlayerStatsManager playerStats;
    private Coroutine initRoutine;

    private void OnEnable()
    {
        // �ʱ�ȭ �ڷ�ƾ: �÷��̾�/UI�� �غ�� ������ ��� �� ���ε�
        initRoutine = StartCoroutine(InitializeWhenReady());
    }

    private void OnDisable()
    {
        if (initRoutine != null) { StopCoroutine(initRoutine); initRoutine = null; }
        UnsubscribeEvents();
    }

    private IEnumerator InitializeWhenReady()
    {
        // 1) PlayerStatsManager.Instance�� �غ�� ������ ���
        while (PlayerStatsManager.Instance == null)
            yield return null;

        playerStats = PlayerStatsManager.Instance;

        // 2) StatusUI ��Ʈ�� �غ�� ������ ��� (Ȥ�ö� �ʰ� �����Ǵ� ���)
        Transform statusUI = null;
        while (statusUI == null)
        {
            var go = GameObject.Find(statusUIRootName);
            if (go != null) statusUI = go.transform;
            else yield return null;
        }

        // 3) �� �ν����� �������̸� �ڵ� Ž��
        if (hpBar == null && statusUI.childCount > hpIndex)
            hpBar = statusUI.GetChild(hpIndex).GetComponentInChildren<Image>();
        if (mpBar == null && statusUI.childCount > mpIndex)
            mpBar = statusUI.GetChild(mpIndex).GetComponentInChildren<Image>();
        if (expBar == null && statusUI.childCount > expIndex)
            expBar = statusUI.GetChild(expIndex).GetComponentInChildren<Image>();

        // 4) �̺�Ʈ ���� (�� ����)
        SubscribeEvents();

        // 5) ��� 1ȸ ����
        RefreshAll();
    }

    private void SubscribeEvents()
    {
        if (playerStats == null) return;

        // ��ġ�� ���� ������ �ٷιٷ� �ݿ�
        playerStats.OnHPChanged += OnHPChanged;
        playerStats.OnMPChanged += OnMPChanged;
        playerStats.OnExpChanged += OnExpChanged;
        playerStats.OnLevelUp += OnLevelUp; // ������ ���� EXP��/�ִ�ġ ��ȭ �ݿ�
    }

    private void UnsubscribeEvents()
    {
        if (playerStats == null) return;

        playerStats.OnHPChanged -= OnHPChanged;
        playerStats.OnMPChanged -= OnMPChanged;
        playerStats.OnExpChanged -= OnExpChanged;
        playerStats.OnLevelUp -= OnLevelUp;
    }

    // === �̺�Ʈ �ڵ鷯 ===
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
        // ������ �� �ִ�ġ�� �ٲ�Ƿ� ��ü ����
        RefreshAll();
    }

    // �ܺο��� ���� �����ϰ� ���� �� ȣ�� ����
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
