//using System.Collections;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UI;

//public class StatusBarUI : MonoBehaviour
//{
//    [SerializeField] private Image hpBar;
//    [SerializeField] private Image mpBar;
//    [SerializeField] private Image expBar;
//    [SerializeField] private GameObject Circle;
//    private Image face;

//    [SerializeField] private string statusUIRootName = "StatusUI";
//    [SerializeField] private int hpIndex = 3;   // StatusUI ���� �ڽ� �ε���
//    [SerializeField] private int mpIndex = 4;
//    [SerializeField] private int expIndex = 5;

//    private PlayerStatsManager playerStats;
//    private Coroutine initRoutine;

//    private void Start()
//    {
//        face = Circle.GetComponent<Image>();
//    }

//    private void OnEnable()
//    {
//        // �ʱ�ȭ �ڷ�ƾ: �÷��̾�/UI�� �غ�� ������ ��� �� ���ε�
//        initRoutine = StartCoroutine(InitializeWhenReady());
//    }

//    private void OnDisable()
//    {
//        if (initRoutine != null) { StopCoroutine(initRoutine); initRoutine = null; }
//        UnsubscribeEvents();
//    }

//    private IEnumerator InitializeWhenReady()
//    {
//        // 1) PlayerStatsManager.Instance�� �غ�� ������ ���
//        while (PlayerStatsManager.Instance == null)
//            yield return null;

//        playerStats = PlayerStatsManager.Instance;

//        // 2) StatusUI ��Ʈ�� �غ�� ������ ��� (Ȥ�ö� �ʰ� �����Ǵ� ���)
//        Transform statusUI = null;
//        while (statusUI == null)
//        {
//            var go = GameObject.Find(statusUIRootName);
//            if (go != null) statusUI = go.transform;
//            else yield return null;
//        }

//        // 3) �� �ν����� �������̸� �ڵ� Ž��
//        if (hpBar == null && statusUI.childCount > hpIndex)
//            hpBar = statusUI.GetChild(hpIndex).GetComponentInChildren<Image>();
//        if (mpBar == null && statusUI.childCount > mpIndex)
//            mpBar = statusUI.GetChild(mpIndex).GetComponentInChildren<Image>();
//        if (expBar == null && statusUI.childCount > expIndex)
//            expBar = statusUI.GetChild(expIndex).GetComponentInChildren<Image>();

//        // 4) �̺�Ʈ ���� (�� ����)
//        SubscribeEvents();

//        // 5) ��� 1ȸ ����
//        RefreshAll();
//    }

//    private void SubscribeEvents()
//    {
//        if (playerStats == null) return;

//        // ��ġ�� ���� ������ �ٷιٷ� �ݿ�
//        playerStats.OnHPChanged += OnHPChanged;
//        playerStats.OnMPChanged += OnMPChanged;
//        playerStats.OnExpChanged += OnExpChanged;
//        playerStats.OnLevelUp += OnLevelUp; // ������ ���� EXP��/�ִ�ġ ��ȭ �ݿ�
//    }

//    private void UnsubscribeEvents()
//    {
//        if (playerStats == null) return;

//        playerStats.OnHPChanged -= OnHPChanged;
//        playerStats.OnMPChanged -= OnMPChanged;
//        playerStats.OnExpChanged -= OnExpChanged;
//        playerStats.OnLevelUp -= OnLevelUp;
//    }

//    // === �̺�Ʈ �ڵ鷯 ===
//    private void OnHPChanged(float cur, float max)
//    {
//        if (hpBar == null) return;
//        hpBar.fillAmount = (max > 0f) ? cur / max : 0f;
//    }

//    private void OnMPChanged(float cur, float max)
//    {
//        if (mpBar == null) return;
//        mpBar.fillAmount = (max > 0f) ? cur / max : 0f;
//    }

//    private void OnExpChanged(int level, float exp)
//    {
//        if (expBar == null || playerStats == null || playerStats.Data == null) return;
//        float ratio = Mathf.Clamp01(playerStats.Data.Exp / Mathf.Max(1f, playerStats.Data.ExpToNextLevel));
//        expBar.fillAmount = ratio;
//    }

//    private void OnLevelUp(int level)
//    {
//        // ������ �� �ִ�ġ�� �ٲ�Ƿ� ��ü ����
//        RefreshAll();
//    }

//    // �ܺο��� ���� �����ϰ� ���� �� ȣ�� ����
//    public void RefreshStatus() => RefreshAll();

//    private void RefreshAll()
//    {
//        if (playerStats == null || playerStats.Data == null) return;

//        // HP
//        if (hpBar != null)
//        {
//            float maxHp = Mathf.Max(1f, playerStats.MaxHP);
//            hpBar.fillAmount = playerStats.CurrentHP / maxHp;
//        }

//        // MP
//        if (mpBar != null)
//        {
//            float maxMp = Mathf.Max(1f, playerStats.Data.MaxMP);
//            mpBar.fillAmount = playerStats.Data.CurrentMP / maxMp;
//        }

//        // EXP
//        if (expBar != null)
//        {
//            float ratio = Mathf.Clamp01(playerStats.Data.Exp / Mathf.Max(1f, playerStats.Data.ExpToNextLevel));
//            expBar.fillAmount = ratio;
//        }
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    [Header("Bars")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image expBar;

    [Header("Portrait")]
    [SerializeField] private GameObject Circle; // �ʻ�ȭ�� ����ִ� ������Ʈ
    [SerializeField] private string portraitsFolder = "Portraits"; // Resources/Portraits/ �ȿ��� ã��
    [SerializeField] private string defaultPortraitName = "default"; // ��ü �̹��� ���ϸ�

    private Image face;

    [Header("StatusUI Hierarchy Auto-Wire")]
    [SerializeField] private string statusUIRootName = "StatusUI";
    [SerializeField] private int hpIndex = 3;   // StatusUI ���� �ڽ� �ε���
    [SerializeField] private int mpIndex = 4;
    [SerializeField] private int expIndex = 5;

    private PlayerStatsManager playerStats;
    private Coroutine initRoutine;

    private void Start()
    {
        if (Circle != null)
            face = Circle.GetComponent<Image>();
    }

    private void OnEnable()
    {
        initRoutine = StartCoroutine(InitializeWhenReady());
    }

    private void OnDisable()
    {
        if (initRoutine != null) { StopCoroutine(initRoutine); initRoutine = null; }
        UnsubscribeEvents();
    }

    private IEnumerator InitializeWhenReady()
    {
        // 1) PlayerStats �غ� ���
        while (PlayerStatsManager.Instance == null)
            yield return null;

        playerStats = PlayerStatsManager.Instance;

        // 2) StatusUI ��Ʈ ���
        Transform statusUI = null;
        while (statusUI == null)
        {
            var go = GameObject.Find(statusUIRootName);
            if (go != null) statusUI = go.transform;
            else yield return null;
        }

        // 3) �� �ڵ� Ž��(���������)
        if (hpBar == null && statusUI.childCount > hpIndex)
            hpBar = statusUI.GetChild(hpIndex).GetComponentInChildren<Image>();
        if (mpBar == null && statusUI.childCount > mpIndex)
            mpBar = statusUI.GetChild(mpIndex).GetComponentInChildren<Image>();
        if (expBar == null && statusUI.childCount > expIndex)
            expBar = statusUI.GetChild(expIndex).GetComponentInChildren<Image>();

        // 4) �̺�Ʈ ����
        SubscribeEvents();

        // 5) �ʻ�ȭ/��ġ ��� 1ȸ ����
        RefreshPortrait();
        RefreshAll();
    }

    private void SubscribeEvents()
    {
        if (playerStats == null) return;

        playerStats.OnHPChanged += OnHPChanged;
        playerStats.OnMPChanged += OnMPChanged;
        playerStats.OnExpChanged += OnExpChanged;
        playerStats.OnLevelUp += OnLevelUp;

        // (����) ��Ȱ �� Ư�� ������ ������ �ٲ� �� �ִٸ� ���⵵ ���� ����
        PlayerStatsManager.OnPlayerRevived -= OnPlayerRevivedRefreshPortrait;
        PlayerStatsManager.OnPlayerRevived += OnPlayerRevivedRefreshPortrait;
    }

    private void UnsubscribeEvents()
    {
        if (playerStats != null)
        {
            playerStats.OnHPChanged -= OnHPChanged;
            playerStats.OnMPChanged -= OnMPChanged;
            playerStats.OnExpChanged -= OnExpChanged;
            playerStats.OnLevelUp -= OnLevelUp;
        }
        PlayerStatsManager.OnPlayerRevived -= OnPlayerRevivedRefreshPortrait;
    }

    // ===== Portrait =====
    private void RefreshPortrait()
    {
        if (face == null) return;

        // �÷��̾� ������ ��� (������ humanmale�� ����)
        string race = "humanmale";
        if (playerStats != null && playerStats.Data != null && !string.IsNullOrEmpty(playerStats.Data.Race))
            race = playerStats.Data.Race;

        // ���ϸ� ����(���ϸ��� ������� �ٸ��� ���⼭ ��ȯ)
        string spriteName = MapRaceToPortraitName(race);

        // Resources/Portraits/<spriteName>.png �ε�
        Sprite sp = Resources.Load<Sprite>($"{portraitsFolder}/{spriteName}");
        if (sp == null)
        {
            // ��ü �̹��� �õ�
            sp = Resources.Load<Sprite>($"{portraitsFolder}/{defaultPortraitName}");
#if UNITY_EDITOR
            if (sp == null)
                Debug.LogWarning($"[StatusBarUI] Portrait not found: {portraitsFolder}/{spriteName} (also no default).");
#endif
        }

        face.sprite = sp;
        face.enabled = (sp != null);
        face.preserveAspect = true;
    }

    // ������ �� �ʻ�ȭ ���ϸ� ����(���ϸ��� ���ٸ� �״�� ��ȯ)
    private string MapRaceToPortraitName(string race)
    {
        if (string.IsNullOrEmpty(race)) return "humanmale";

        switch (race.ToLowerInvariant())
        {
            // ���ϸ��� �����ϸ� �״�� ��ȯ
            case "humanmale": return "humanmale";
            case "dwarfmale": return "dwarfmale";
            case "gnomemale": return "gnomemale";
            case "nightelfmale": return "nightelfmale";
            case "orcmale": return "orcmale";
            case "trollmale": return "trollmale";
            case "goblinmale": return "goblinmale";
            case "scourgefemale": return "scourgefemale";

            // ���ϸ��� �ٸ��� ���⼭ ���ϴ� �̸����� ��ȯ
            // case "mycustomrace":  return "portrait_mycustom";

            default: return race.ToLowerInvariant(); // �õ� �� ���� �� default�� ��ü
        }
    }

    private void OnPlayerRevivedRefreshPortrait()
    {
        RefreshPortrait();
    }

    // ===== Bars =====
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
        // ������ �� �ʻ�ȭ�� �ٲ��� ������, Ȥ�� ���� ��ȯ �ý����� �ִٸ� �Ʒ� ȣ�� ���� ����
        // RefreshPortrait();
        RefreshAll();
    }

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
