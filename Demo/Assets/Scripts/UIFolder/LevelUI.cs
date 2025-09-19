using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField] private Image tensPlace;   // 10�� �ڸ�
    [SerializeField] private Image onesPlace;   // 1�� �ڸ�

    private PlayerStatsManager playerStats;     // �̱��� ����
    private Sprite[] numberSprites;

    private void Awake()
    {
        // ���� ��������Ʈ �ε� (Assets/Resources/Prefabs/Levels/0~9)
        numberSprites = Resources.LoadAll<Sprite>("Prefabs/Levels");
        if (numberSprites == null || numberSprites.Length < 10)
            Debug.LogError("[LevelUI] Prefabs/Levels ������ 0~9 ��������Ʈ�� �ʿ��մϴ�!");

        // �ʿ� ��, ���̾��Ű���� ���� ã�� ���� (���� �巡�� ����Ǿ� ������ ���� ����)
        if (!tensPlace || !onesPlace)
        {
            Transform statusUI = GameObject.Find("LevelUI")?.transform;
            if (statusUI != null)
            {
                tensPlace = tensPlace ? tensPlace : statusUI.GetChild(0).GetComponent<Image>();
                onesPlace = onesPlace ? onesPlace : statusUI.GetChild(1).GetComponent<Image>();
            }
        }
    }

    private void OnEnable()
    {
        // �̱���/������ �غ�� ������ ��� �� ���� + �ʱ� 1ȸ ����
        StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        while (PlayerStatsManager.Instance == null || PlayerStatsManager.Instance.Data == null)
            yield return null;

        playerStats = PlayerStatsManager.Instance;

        // �ߺ� ���� �� ����
        playerStats.OnLevelUp -= UpdateLevelUI;
        playerStats.OnLevelUp += UpdateLevelUI;

        // �ʱ� 1ȸ ���� ����
        UpdateLevelUI(playerStats.Data.Level);
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnLevelUp -= UpdateLevelUI;
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnLevelUp -= UpdateLevelUI;
    }

    /// <summary>
    /// ���� ������ UI ������Ʈ (�̺�Ʈ ���)
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

        // 10�� �ڸ�
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

        // 1�� �ڸ�
        if (onesPlace)
        {
            onesPlace.sprite = numberSprites[ones];
            onesPlace.enabled = true;
        }

        // �����
        // Debug.Log($"[LevelUI] ������Ʈ -> Level={level}");
    }
}
