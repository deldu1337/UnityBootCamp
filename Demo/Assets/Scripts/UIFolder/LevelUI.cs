using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private Image tensPlace;   // 10의 자리
    [SerializeField] private Image onesPlace;   // 1의 자리
    private PlayerStatsManager playerStats;
    private Sprite[] numberSprites;

    //private void Awake()
    //{
    //    // PlayerStatsManager 자동 연결
    //    if (!playerStats)
    //        playerStats = GetComponentInParent<PlayerStatsManager>();

    //    // Resources 폴더에서 스프라이트 로드 (Assets/Resources/Prefabs/Levels/0~9)
    //    numberSprites = Resources.LoadAll<Sprite>("Prefabs/Levels");
    //    if (numberSprites == null || numberSprites.Length < 10)
    //        Debug.LogError("[LevelUI] Prefabs/Levels 폴더에 0~9 스프라이트가 필요합니다!");

    //    Transform statusUI = GameObject.Find("LevelUI").transform;
    //    tensPlace = statusUI.GetChild(0).GetComponent<Image>();
    //    onesPlace = statusUI.GetChild(1).GetComponent<Image>();
    //}
    private void Awake()
    {
        playerStats = PlayerStatsManager.Instance; // ← 싱글톤

        numberSprites = Resources.LoadAll<Sprite>("Prefabs/Levels");
        if (numberSprites == null || numberSprites.Length < 10)
            Debug.LogError("[LevelUI] Prefabs/Levels 폴더에 0~9 스프라이트가 필요합니다!");

        Transform statusUI = GameObject.Find("LevelUI").transform;
        tensPlace = statusUI.GetChild(0).GetComponent<Image>();
        onesPlace = statusUI.GetChild(1).GetComponent<Image>();
    }

    //private void OnEnable()
    //{
    //    if (playerStats)
    //    {
    //        // 이벤트 구독
    //        playerStats.OnLevelUp += UpdateLevelUI;

    //        // 시작 시 레벨 UI 강제 초기화
    //        if (playerStats.Data != null)
    //            UpdateLevelUI(playerStats.Data.Level);
    //    }
    //}
    private void OnEnable()
    {
        // 싱글톤 준비될 때까지 기다렸다가 구독 + 초기 렌더
        StartCoroutine(BindWhenReady());
    }

    private IEnumerator BindWhenReady()
    {
        // PlayerStatsManager 인스턴스와 Data가 준비될 때까지 대기
        while (PlayerStatsManager.Instance == null || PlayerStatsManager.Instance.Data == null)
            yield return null;

        playerStats = PlayerStatsManager.Instance;

        // 중복 구독 방지 후 구독
        playerStats.OnLevelUp -= UpdateLevelUI;
        playerStats.OnLevelUp += UpdateLevelUI;

        // 첫 렌더
        UpdateLevelUI(playerStats.Data.Level);
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnLevelUp -= UpdateLevelUI;
    }

    /// <summary>
    /// 레벨 값으로 UI 업데이트
    /// </summary>
    private void UpdateLevelUI(int level)
    {
        if (numberSprites == null || numberSprites.Length < 10)
        {
            Debug.LogWarning("[LevelUI] 숫자 스프라이트가 준비되지 않음");
            return;
        }

        int tens = level / 10;  // 10의 자리
        int ones = level % 10;  // 1의 자리

        // 10의 자리 설정
        if (tensPlace)
        {
            if (tens > 0)
            {
                tensPlace.sprite = numberSprites[tens];
                tensPlace.enabled = true;
            }
            else
            {
                tensPlace.enabled = false; // 한 자리 레벨이면 숨김
            }
        }

        // 1의 자리 설정
        if (onesPlace)
        {
            onesPlace.sprite = numberSprites[ones];
            onesPlace.enabled = true;
        }

        Debug.Log($"[LevelUI] UI 업데이트 완료 -> Level={level}");
    }
}
