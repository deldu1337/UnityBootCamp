using UnityEngine;
using UnityEngine.UI;

public class AngelResurrectionManager : MonoBehaviour
{
    public static AngelResurrectionManager Instance { get; private set; }

    [Header("Prefab")]
    [Tooltip("플레이어 사망 위치에 소환될 천사 프리팹(월드 오브젝트). 자식에 ResurrectButton(UIButton) 포함 권장.")]
    public GameObject angelPrefab;

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

        // 플레이어의 마지막 위치/회전 값 저장
        Vector3 pos = player.transform.position;
        Quaternion rot = player.transform.rotation;

        currentAngel = Instantiate(angelPrefab, pos, rot);

        // 프리팹 내부에서 버튼을 찾아 이벤트 연결
        var resurrectBtn = currentAngel.GetComponentInChildren<Button>(true);
        if (resurrectBtn != null)
        {
            resurrectBtn.onClick.RemoveAllListeners();
            resurrectBtn.onClick.AddListener(() =>
            {
                player.ReviveAt(pos, rot);
            });
        }
        else
        {
            Debug.LogWarning("[AngelResurrectionManager] ResurrectButton(Button)이 프리팹 내에 없습니다. 직접 연결하세요.");
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
