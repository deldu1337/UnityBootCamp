//using UnityEngine;
//using UnityEngine.UI;

//public class AngelResurrectionManager : MonoBehaviour
//{
//    public static AngelResurrectionManager Instance { get; private set; }

//    [Header("Prefab")]
//    [Tooltip("�÷��̾� ��� ��ġ�� ��ȯ�� õ�� ������(���� ������Ʈ). �ڽĿ� ResurrectButton(UIButton) ���� ����.")]
//    public GameObject angelPrefab;

//    private GameObject currentAngel;

//    void Awake()
//    {
//        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
//        Instance = this;
//    }

//    void OnEnable()
//    {
//        PlayerStatsManager.OnPlayerDeathAnimFinished += SpawnAngelAtPlayer;
//        PlayerStatsManager.OnPlayerRevived += CleanupAngel;
//    }

//    void OnDisable()
//    {
//        PlayerStatsManager.OnPlayerDeathAnimFinished -= SpawnAngelAtPlayer;
//        PlayerStatsManager.OnPlayerRevived -= CleanupAngel;
//    }

//    private void SpawnAngelAtPlayer()
//    {
//        if (currentAngel || angelPrefab == null) return;

//        var player = PlayerStatsManager.Instance;
//        if (!player) return;

//        // �÷��̾��� ������ ��ġ/ȸ�� �� ����
//        Vector3 pos = player.transform.position;
//        Quaternion rot = player.transform.rotation;

//        currentAngel = Instantiate(angelPrefab, pos, rot);

//        // ������ ���ο��� ��ư�� ã�� �̺�Ʈ ����
//        var resurrectBtn = currentAngel.GetComponentInChildren<Button>(true);
//        if (resurrectBtn != null)
//        {
//            resurrectBtn.onClick.RemoveAllListeners();
//            resurrectBtn.onClick.AddListener(() =>
//            {
//                player.ReviveAt(pos, rot);
//            });
//        }
//        else
//        {
//            Debug.LogWarning("[AngelResurrectionManager] ResurrectButton(Button)�� ������ ���� �����ϴ�. ���� �����ϼ���.");
//        }
//    }

//    private void CleanupAngel()
//    {
//        if (currentAngel)
//        {
//            Destroy(currentAngel);
//            currentAngel = null;
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;

public class AngelResurrectionManager : MonoBehaviour
{
    public static AngelResurrectionManager Instance { get; private set; }

    [Header("Prefab")]
    [Tooltip("�÷��̾� ��� ��ġ�� ��ȯ�� õ�� ������(���� ������Ʈ). �ڽĿ� ResurrectButton(UIButton) ���� ����.")]
    public GameObject angelPrefab;

    [Header("Resurrect Button Sprite Swap")]
    [Tooltip("��ư�� ������ �ִ� ����(Pressed)�� ������ ��������Ʈ")]
    public Sprite pressedSprite;
    [Tooltip("���콺 ����(Highlight) �� ������ ��������Ʈ (����)")]
    public Sprite highlightedSprite;
    [Tooltip("����(Selected) �� ������ ��������Ʈ (����)")]
    public Sprite selectedSprite;
    [Tooltip("��Ȱ��(Disabled) �� ������ ��������Ʈ (����)")]
    public Sprite disabledSprite;

    private GameObject currentAngel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        PlayerStatsManager.OnPlayerDeathAnimFinished += SpawnAngelAtPlayer;
        PlayerStatsManager.OnPlayerRevived += CleanupAngel;
    }

    void OnDisable()
    {
        PlayerStatsManager.OnPlayerDeathAnimFinished -= SpawnAngelAtPlayer;
        PlayerStatsManager.OnPlayerRevived -= CleanupAngel;
    }

    private void SpawnAngelAtPlayer()
    {
        if (currentAngel || angelPrefab == null) return;

        var player = PlayerStatsManager.Instance;
        if (!player) return;

        Vector3 pos = player.transform.position;
        Quaternion rot = player.transform.rotation;

        currentAngel = Instantiate(angelPrefab, pos, rot);

        // ������ ���ο��� ��ư�� ã�� �̺�Ʈ ����
        var resurrectBtn = currentAngel.GetComponentInChildren<Button>(true);
        if (resurrectBtn != null)
        {
            // 1) ��ư Transition�� SpriteSwap���� ����
            resurrectBtn.transition = Selectable.Transition.SpriteSwap;

            // 2) ���� SpriteState�� ������ �ʿ��� �׸� �����
            var st = resurrectBtn.spriteState;
            if (pressedSprite) st.pressedSprite = pressedSprite;
            if (highlightedSprite) st.highlightedSprite = highlightedSprite;
            if (selectedSprite) st.selectedSprite = selectedSprite;
            if (disabledSprite) st.disabledSprite = disabledSprite;
            resurrectBtn.spriteState = st;

            // 3) Ŭ�� ����
            resurrectBtn.onClick.RemoveAllListeners();
            resurrectBtn.onClick.AddListener(() =>
            {
                player.ReviveAt(pos, rot);
            });
        }
        else
        {
            Debug.LogWarning("[AngelResurrectionManager] ResurrectButton(Button)�� ������ ���� �����ϴ�. ���� �����ϼ���.");
        }
    }

    private void CleanupAngel()
    {
        if (currentAngel)
        {
            Destroy(currentAngel);
            currentAngel = null;
        }
    }
}
