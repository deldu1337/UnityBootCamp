//using UnityEngine;
//using UnityEngine.UI;

//public class PlayerInfoPresenter : MonoBehaviour
//{
//    [SerializeField] private GameObject playerInfoUI;
//    [SerializeField] private Button exitButton;
//    private EquipmentPresenter equipmentPresenter;

//    [SerializeField] private GameObject equipmentUI;

//    [SerializeField] private bool forceCloseOnStart = true;

//    private RectTransform playerInfoRect;
//    private RectTransform equipmentRect;

//    private bool isOpen = false;
//    public bool IsOpen => isOpen;

//    // ��Ȱ�� ���� Ž�� ����
//    private static GameObject FindIncludingInactive(string name)
//    {
//        var all = Resources.FindObjectsOfTypeAll<GameObject>();
//        for (int i = 0; i < all.Length; i++)
//        {
//            var go = all[i];
//            if (go && go.name == name && (go.hideFlags == 0))
//                return go;
//        }
//        return null;
//    }

//    void Start()
//    {
//        if (!playerInfoUI) playerInfoUI = GameObject.Find("PlayerInfoUI") ?? FindIncludingInactive("PlayerInfoUI");
//        if (!equipmentUI) equipmentUI = GameObject.Find("EquipmentUI") ?? FindIncludingInactive("EquipmentUI");

//        UIEscapeStack.GetOrCreate();

//        if (!playerInfoUI)
//        {
//            Debug.LogError("[PlayerInfoPresenter] playerInfoUI�� ã�� ���߽��ϴ�.");
//            enabled = false;
//            return;
//        }

//        // EquipmentPresenter/EquipmentView ������ Rect�� ��� ��Ʈ
//        if (!equipmentPresenter)
//            equipmentPresenter = FindAnyObjectByType<EquipmentPresenter>(FindObjectsInactive.Include);

//        playerInfoRect = playerInfoUI.GetComponent<RectTransform>();

//        if (!equipmentUI && equipmentPresenter != null)
//        {
//            // View.RootRect ����
//            var view = FindAnyObjectByType<EquipmentView>(FindObjectsInactive.Include);
//            if (view && view.RootRect)
//                equipmentUI = view.RootRect.gameObject;
//        }

//        equipmentRect = equipmentUI ? equipmentUI.GetComponent<RectTransform>() : null;

//        if (exitButton) exitButton.onClick.AddListener(Close);

//        if (forceCloseOnStart)
//        {
//            if (playerInfoUI.activeSelf) playerInfoUI.SetActive(false);
//            isOpen = false;
//            UIEscapeStack.Instance.Remove("playerinfo");
//        }
//        else
//        {
//            isOpen = playerInfoUI.activeSelf;
//            if (isOpen) UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);
//        }
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.R))
//        {
//            // �� ��ȯ ����: ���â�� ���� ������ ���� ��ġ ������ ����
//            bool equipWasOpen = equipmentPresenter && equipmentPresenter.IsOpen;
//            if (equipWasOpen && equipmentRect)
//                UIPanelSwitcher.SaveSnapshot(equipmentRect);

//            // ���� ���� ����
//            if (equipWasOpen && equipmentRect && playerInfoRect)
//                UIPanelSwitcher.CopyLayoutRT(equipmentRect, playerInfoRect);

//            Toggle();

//            if (equipWasOpen && equipmentPresenter)
//            {
//                UIPanelSwitcher.CopyLayoutRT(playerInfoRect, equipmentRect);
//                equipmentPresenter.CloseEquipmentPublic();
//            }
//        }
//    }

//    public void Open()
//    {
//        if (isOpen || !playerInfoUI) return;

//        // �� ������ ���� ������ ������ ����
//        if (playerInfoRect && UIPanelSwitcher.HasSnapshot)
//            UIPanelSwitcher.LoadSnapshot(playerInfoRect);

//        if (equipmentUI && equipmentUI.activeSelf) equipmentUI.SetActive(false);

//        playerInfoUI.SetActive(true);
//        isOpen = true;
//        UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);
//    }

//    public void Close()
//    {
//        if (!isOpen || !playerInfoUI) return;

//        // �� ������ ���� ���� ��ġ ������ ����
//        if (playerInfoRect)
//            UIPanelSwitcher.SaveSnapshot(playerInfoRect);

//        playerInfoUI.SetActive(false);
//        isOpen = false;
//        UIEscapeStack.Instance.Remove("playerinfo");
//    }


//    public void Toggle() { if (isOpen) Close(); else Open(); }
//}
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInfoPresenter : MonoBehaviour
{
    [SerializeField] private GameObject playerInfoUI;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject equipmentUI;
    [SerializeField] private bool forceCloseOnStart = true;

    // �� �߰�: ���� �ؽ�Ʈ ���۷���
    [SerializeField] private Text statsText;

    private Button InfoButton;
    private Image image;
    private Sprite[] sprites;
    private RectTransform playerInfoRect;   // �� ������ �����̴� RT
    private RectTransform equipmentRect;    // �� ������ �����̴� RT
    private bool isOpen = false;
    public bool IsOpen => isOpen;

    private Coroutine initRoutine;                // �� �߰�: �ʱ�ȭ �ڷ�ƾ
    private PlayerStatsManager ps;                // �� �߰�: ĳ��

    // ��Ȱ�� ���� Ž�� (�̸�����)
    private static GameObject FindIncludingInactive(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < all.Length; i++)
        {
            var go = all[i];
            if (go && go.name == name && (go.hideFlags == 0))
                return go;
        }
        return null;
    }

    // ��Ʈ���� ���� �巡�׷� �̵��ϴ� "â �г�" RT ã��
    // ��Ģ: ��Ʈ �������� UIDragHandler�� ã��, �� �ڵ鷯�� parent RT�� ��ȯ. ������ ��Ʈ�� RT.
    // �� ����: ������ ������ "â ��Ʈ" RT�� ��´�.
    // ��Ģ:
    //  1) root �������� �̸��� "HeadPanel"�� Ʈ�������� ã�� -> �� parent RT�� â ��Ʈ�� ���
    //  2) ������ root�� RectTransform ���
    //  3) ���������� Canvas�� '���� �ڽ�' �������� Ÿ�� �ö� ����
    private static RectTransform GetMovableWindowRT(GameObject root)
    {
        if (!root) return null;

        RectTransform cand = null;

        // 1) HeadPanel �켱
        var head = root.transform.Find("HeadPanel");
        if (head == null)
        {
            // Ȥ�� �� ���� ���� �� ������ ��ü Ž��
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "HeadPanel") { head = t; break; }
            }
        }

        if (head && head.parent is RectTransform headParentRT)
        {
            cand = headParentRT; // HeadPanel�� �θ� 'â ��Ʈ'
        }
        else
        {
            // 2) fallback: root�� RT
            cand = root.GetComponent<RectTransform>();
        }

        if (!cand) return null;

        // 3) Canvas ���� �ڽ� �������� ����ø���(���� �г��� ���� ������ ����)
        RectTransform cur = cand;
        while (cur && cur.parent is RectTransform prt)
        {
            if (prt.GetComponent<Canvas>() != null) break; // prt�� Canvas �� cur�� Canvas ����
            cur = prt;
        }
        return cur;
    }

    void OnEnable()
    {
        // �� �÷��̾�/�Ŵ��� �غ�� ������ ��� �� �̺�Ʈ ����
        initRoutine = StartCoroutine(InitializeWhenReady());
    }

    void OnDisable()
    {
        if (initRoutine != null) { StopCoroutine(initRoutine); initRoutine = null; }
        UnsubscribeStatEvents();
    }

    private IEnumerator InitializeWhenReady()     // �� �߰�
    {
        // PlayerStatsManager.Instance ���
        while (PlayerStatsManager.Instance == null) yield return null;
        ps = PlayerStatsManager.Instance;

        // UI ������ ���� null�̸� �� ������ ���� �� ���
        if (playerInfoUI == null)
        {
            yield return null;
            playerInfoUI = GameObject.Find("PlayerInfoUI") ?? playerInfoUI;
        }

        // �̺�Ʈ ���� & ù ����
        SubscribeStatEvents();
        RefreshStatsText();
    }

    private void SubscribeStatEvents()            // �� �߰�
    {
        if (ps == null) return;

        ps.OnHPChanged -= OnHPChanged;
        ps.OnMPChanged -= OnMPChanged;
        ps.OnExpChanged -= OnExpChanged;
        ps.OnLevelUp -= OnLevelUp;

        ps.OnHPChanged += OnHPChanged;
        ps.OnMPChanged += OnMPChanged;
        ps.OnExpChanged += OnExpChanged;
        ps.OnLevelUp += OnLevelUp;
    }

    private void UnsubscribeStatEvents()          // �� �߰�
    {
        if (ps == null) return;

        ps.OnHPChanged -= OnHPChanged;
        ps.OnMPChanged -= OnMPChanged;
        ps.OnExpChanged -= OnExpChanged;
        ps.OnLevelUp -= OnLevelUp;
    }

    // ===== �̺�Ʈ �ڵ鷯: ���� �ؽ�Ʈ �������÷� ���� =====
    private void OnHPChanged(float cur, float max) => RefreshStatsText();
    private void OnMPChanged(float cur, float max) => RefreshStatsText();
    private void OnExpChanged(int level, float exp) => RefreshStatsText();
    private void OnLevelUp(int level) => RefreshStatsText();

    // ===== Start/Update/Toggle/Open/Close �� ���� ������ �״�� �ΰ�,
    //      Open()���� �� �� �� RefreshStatsText() ȣ���ϴ� ������ ���� =====

    void Start()
    {
        if (!playerInfoUI) playerInfoUI = GameObject.Find("PlayerInfoUI") ?? FindIncludingInactive("PlayerInfoUI");
        if (!equipmentUI) equipmentUI = GameObject.Find("EquipmentUI") ?? FindIncludingInactive("EquipmentUI");
        exitButton = playerInfoUI.transform.GetChild(4).GetComponent<Button>();
        sprites = new Sprite[8];
        sprites = Resources.LoadAll<Sprite>("CharacterIcons");
        statsText = playerInfoUI.transform.GetChild(7).GetComponent<Text>();

        var ps = PlayerStatsManager.Instance;
        string race = (ps != null && ps.Data != null && !string.IsNullOrEmpty(ps.Data.Race))
                        ? ps.Data.Race
                        : "humanmale";

        var quickUI = GameObject.Find("QuickUI");
        if (quickUI != null && quickUI.transform.childCount > 1)
        {
            InfoButton = quickUI.transform.GetChild(2).GetComponent<Button>();
            if (InfoButton) InfoButton.onClick.AddListener(Toggle);
        }

        image = InfoButton.GetComponent<Image>();

        for (int i = 0; i < sprites.Length; i++)
        {
            Debug.Log(sprites[i].name);
            if(sprites[i].name == race)
                image.sprite = sprites[i];
        }

        UIEscapeStack.GetOrCreate();

        if (!playerInfoUI)
        {
            Debug.LogError("[PlayerInfoPresenter] playerInfoUI�� ã�� ���߽��ϴ�.");
            enabled = false;
            return;
        }

        // ���� �����̴� RT�� ����
        playerInfoRect = GetMovableWindowRT(playerInfoUI);
        equipmentRect = GetMovableWindowRT(equipmentUI);

        if (exitButton) exitButton.onClick.AddListener(Close);

        if (forceCloseOnStart)
        {
            if (playerInfoUI.activeSelf) playerInfoUI.SetActive(false);
            isOpen = false;
            UIEscapeStack.Instance.Remove("playerinfo");
        }
        else
        {
            isOpen = playerInfoUI.activeSelf;
            if (isOpen) UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // ��ȯ ����: ���â�� ���� ������, ���â�� "�����̴� RT" �������� ������ ���� + ���̾ƿ� ����
            var eqPresenter = FindAnyObjectByType<EquipmentPresenter>();
            bool equipWasOpen = eqPresenter && eqPresenter.IsOpen;

            if (equipWasOpen && equipmentRect)
            {
                Debug.Log($"[SNAP] Save from: {PathOf(playerInfoRect)} localPos={playerInfoRect.localPosition}");
                UIPanelSwitcher.SaveSnapshot(equipmentRect);
            }

            if (equipWasOpen && equipmentRect && playerInfoRect)
                UIPanelSwitcher.CopyLayoutRT(equipmentRect, playerInfoRect);

            Toggle();

            if (equipWasOpen && eqPresenter)
            {
                eqPresenter.CloseEquipmentPublic();

                // (����) ���� PI ��ġ�� ����ʿ��� �ݿ��صΰ� �ݱ�
                if (playerInfoRect && equipmentRect)
                    UIPanelSwitcher.CopyLayoutRT(playerInfoRect, equipmentRect);
            }
        }
    }

    private static string PathOf(Transform t)
    {
        if (!t) return "<null>";
        System.Text.StringBuilder sb = new System.Text.StringBuilder(t.name);
        while (t.parent)
        {
            t = t.parent;
            sb.Insert(0, t.name + "/");
        }
        return sb.ToString();
    }

    public void Toggle() { if (isOpen) Close(); else Open(); }

    // PlayerInfoPresenter
    public void Open()
    {
        if (isOpen || !playerInfoUI) return;

        // 1�� ����(���� ���� ����)
        if (playerInfoRect && UIPanelSwitcher.HasSnapshot)
        {
            Debug.Log($"[SNAP] Load  to: {PathOf(playerInfoRect)}");
            UIPanelSwitcher.LoadSnapshot(playerInfoRect);
        }

        playerInfoUI.SetActive(true);
        isOpen = true;
        UIEscapeStack.Instance.Push("playerinfo", Close, () => isOpen);

        // �� �߰�: ���� �� �ֽ� ���� ����
        RefreshStatsText();

        // �� �ٽ�: Ȱ��ȭ�� ���� ���̾ƿ� �����尡 ���� "���� ������"�� �ٽ� ����
        if (playerInfoRect && UIPanelSwitcher.HasSnapshot)
            StartCoroutine(ReapplySnapshotNextFrame(playerInfoRect));
    }

    private System.Collections.IEnumerator ReapplySnapshotNextFrame(RectTransform rt)
    {
        yield return null; // �� ������ ��� (SetActive �� �θ� ���̾ƿ� ������ ���� ��)
        Debug.Log($"[SNAP] Load  to: {PathOf(playerInfoRect)}");
        UIPanelSwitcher.LoadSnapshot(rt);           // ������ ������
        Canvas.ForceUpdateCanvases();
        var prt = rt.parent as RectTransform;
        if (prt) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(prt);
    }


    public void Close()
    {
        if (!isOpen || !playerInfoUI) return;

        // ������ ���� ���� ��ġ ������ ���� (�����̴� RT)
        if (playerInfoRect)
            UIPanelSwitcher.SaveSnapshot(playerInfoRect);

        playerInfoUI.SetActive(false);
        isOpen = false;
        UIEscapeStack.Instance.Remove("playerinfo");
    }

    // �� �߰�: ���� �ؽ�Ʈ ���� �Լ� (�ܺο����� ȣ�� ����)
    public void RefreshStatsText()
    {
        if (!statsText) return;

        var ps = PlayerStatsManager.Instance;
        var d = ps != null ? ps.Data : null;
        if (d == null)
        {
            statsText.text = "���� �����͸� �ҷ��� �� �����ϴ�.";
            return;
        }

        // ���� ���� �������� ����
        var sb = new StringBuilder();
        sb.AppendLine($"���� : {d.Race}");
        sb.AppendLine($"���� : {d.Level}");
        sb.AppendLine($"����ġ(EXP) : {d.Exp:#,0} / {d.ExpToNextLevel:#,0}");
        sb.AppendLine();
        sb.AppendLine($"HP : {d.CurrentHP:#,0.##} / {d.MaxHP:#,0.##}");
        sb.AppendLine($"MP : {d.CurrentMP:#,0.##} / {d.MaxMP:#,0.##}");
        sb.AppendLine($"������(ATK) : {d.Atk:#,0.##}");
        sb.AppendLine($"����(DEF) : {d.Def:#,0.##}");
        sb.AppendLine($"��ø��(DEX) : {d.Dex:#,0.##}");
        sb.AppendLine($"���� �ӵ�(AS) : {d.AttackSpeed:#,0.##}");
        sb.AppendLine($"ġ��Ÿ Ȯ��(CC) : {d.CritChance * 100f:0.##}%");
        sb.AppendLine($"ġ��Ÿ ������(CD) : {d.CritDamage:0.##}x");

        statsText.text = sb.ToString();
    }
}
