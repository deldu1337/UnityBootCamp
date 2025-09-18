using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] private Image tensPlace;   // 10�� �ڸ�
    [SerializeField] private Image onesPlace;   // 1�� �ڸ�
    private PlayerStatsManager playerStats;
    private Sprite[] numberSprites;

    //private void Awake()
    //{
    //    // PlayerStatsManager �ڵ� ����
    //    if (!playerStats)
    //        playerStats = GetComponentInParent<PlayerStatsManager>();

    //    // Resources �������� ��������Ʈ �ε� (Assets/Resources/Prefabs/Levels/0~9)
    //    numberSprites = Resources.LoadAll<Sprite>("Prefabs/Levels");
    //    if (numberSprites == null || numberSprites.Length < 10)
    //        Debug.LogError("[LevelUI] Prefabs/Levels ������ 0~9 ��������Ʈ�� �ʿ��մϴ�!");

    //    Transform statusUI = GameObject.Find("LevelUI").transform;
    //    tensPlace = statusUI.GetChild(0).GetComponent<Image>();
    //    onesPlace = statusUI.GetChild(1).GetComponent<Image>();
    //}
    private void Awake()
    {
        playerStats = PlayerStatsManager.Instance; // �� �̱���

        numberSprites = Resources.LoadAll<Sprite>("Prefabs/Levels");
        if (numberSprites == null || numberSprites.Length < 10)
            Debug.LogError("[LevelUI] Prefabs/Levels ������ 0~9 ��������Ʈ�� �ʿ��մϴ�!");

        Transform statusUI = GameObject.Find("LevelUI").transform;
        tensPlace = statusUI.GetChild(0).GetComponent<Image>();
        onesPlace = statusUI.GetChild(1).GetComponent<Image>();
    }

    //private void OnEnable()
    //{
    //    if (playerStats)
    //    {
    //        // �̺�Ʈ ����
    //        playerStats.OnLevelUp += UpdateLevelUI;

    //        // ���� �� ���� UI ���� �ʱ�ȭ
    //        if (playerStats.Data != null)
    //            UpdateLevelUI(playerStats.Data.Level);
    //    }
    //}
    private void OnEnable()
    {
        // �̱��� �غ�� ������ ��ٷȴٰ� ���� + �ʱ� ����
        StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        // PlayerStatsManager �ν��Ͻ��� Data�� �غ�� ������ ���
        while (PlayerStatsManager.Instance == null || PlayerStatsManager.Instance.Data == null)
            yield return null;

        playerStats = PlayerStatsManager.Instance;

        // �ߺ� ���� ���� �� ����
        playerStats.OnLevelUp -= UpdateLevelUI;
        playerStats.OnLevelUp += UpdateLevelUI;

        // ù ����
        UpdateLevelUI(playerStats.Data.Level);
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnLevelUp -= UpdateLevelUI;
    }

    /// <summary>
    /// ���� ������ UI ������Ʈ
    /// </summary>
    private void UpdateLevelUI(int level)
    {
        if (numberSprites == null || numberSprites.Length < 10)
        {
            Debug.LogWarning("[LevelUI] ���� ��������Ʈ�� �غ���� ����");
            return;
        }

        int tens = level / 10;  // 10�� �ڸ�
        int ones = level % 10;  // 1�� �ڸ�

        // 10�� �ڸ� ����
        if (tensPlace)
        {
            if (tens > 0)
            {
                tensPlace.sprite = numberSprites[tens];
                tensPlace.enabled = true;
            }
            else
            {
                tensPlace.enabled = false; // �� �ڸ� �����̸� ����
            }
        }

        // 1�� �ڸ� ����
        if (onesPlace)
        {
            onesPlace.sprite = numberSprites[ones];
            onesPlace.enabled = true;
        }

        Debug.Log($"[LevelUI] UI ������Ʈ �Ϸ� -> Level={level}");
    }
}
