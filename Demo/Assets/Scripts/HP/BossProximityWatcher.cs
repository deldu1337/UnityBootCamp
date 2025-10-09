using System.Linq;
using UnityEngine;

public class BossProximityWatcher : MonoBehaviour
{
    [Header("Player (Layer�� Ž��)")]
    [SerializeField] private Transform player;         // ���� playerLayer�� �ڵ� Ž��
    [SerializeField] private LayerMask playerLayer;    // ��: Player ���̾� üũ

    [Header("UI")]
    [SerializeField] private BossTopBarUI bossTopUI;   // BossCanvas�� ���� ��ũ��Ʈ

    [Header("Settings")]
    [SerializeField] private float showRadius = 100f;   // �� �Ÿ� �̳��� HP�� ǥ��

    private EnemyStatsManager bossESM;                 // IHealth ����ü

    void Awake()
    {
        // �÷��̾ ���̾�� �ڵ� Ž��
        if (player == null)
        {
            // �� �� ��� Transform �� ���̾� ��ġ�ϴ� ù��°�� ã��
            var all = FindObjectsByType<Transform>(FindObjectsSortMode.None);
            var t = all.FirstOrDefault(tf => (playerLayer.value & (1 << tf.gameObject.layer)) != 0);
            if (t != null) player = t;
        }

        if (bossTopUI == null)
            bossTopUI = FindAnyObjectByType<BossTopBarUI>();
    }

    /// <summary>EnemySpawn���� ���� ���� �� ȣ��</summary>
    public void SetBoss(EnemyStatsManager esm)
    {
        bossESM = esm;
        if (bossTopUI != null && bossESM != null)
        {
            bossTopUI.SetBoss(bossESM); // HealthBarUI ���α��� Ÿ�� ���õ�
        }
    }

    void Update()
    {
        if (bossTopUI == null) return;

        if (bossESM == null || player == null || bossESM.CurrentHP <= 0f)
        {
            bossTopUI.Show(false);
            return;
        }

        float d = Vector3.Distance(player.position, bossESM.transform.position);
        bossTopUI.Show(d <= showRadius);
    }
}
