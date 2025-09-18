using UnityEngine;

public class EnemyStatsManager : MonoBehaviour, IHealth
{
    [Header("�� ID (enemyData.json�� id�� ��ġ)")]
    public string enemyId;

    public EnemyData Data { get; private set; }
    public float CurrentHP { get; private set; }
    public float MaxHP => Data.hp;

    private ItemDropManager dropManager;

    void Awake()
    {
        dropManager = GetComponent<ItemDropManager>();
        LoadEnemyData();
    }

    private void LoadEnemyData()
    {
        TextAsset json = Resources.Load<TextAsset>("Datas/enemyData");
        if (json == null)
        {
            Debug.LogError("Resources/Datas/enemyData.json ������ �ʿ��մϴ�!");
            return;
        }

        EnemyDatabase db = JsonUtility.FromJson<EnemyDatabase>(json.text);
        Data = System.Array.Find(db.enemies, e => e.id == enemyId);

        if (Data == null)
        {
            Debug.LogError($"enemyId '{enemyId}' �����͸� ã�� �� �����ϴ�!");
            return;
        }

        CurrentHP = Data.hp;
        Debug.Log($"{Data.name} ������ �ε� �Ϸ�. HP: {CurrentHP}, ATK: {Data.atk}");
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Max(damage - Data.def, 1f);
        CurrentHP = Mathf.Max(CurrentHP - damage, 0);
        Debug.Log($"{Data.name} HP: {CurrentHP}/{Data.hp}");

        if (CurrentHP <= 0)
            Die();
    }

    //private void Die()
    //{
    //    Debug.Log($"{Data.name} ���!");

    //    // EXP ����
    //    var player = FindAnyObjectByType<PlayerStatsManager>();
    //    if (player != null)
    //    {
    //        player.GainExp(Data.exp);
    //        Debug.Log($"�÷��̾ {Data.exp} EXP�� ȹ��!");
    //    }

    //    dropManager?.DropItems();
    //    Destroy(gameObject);
    //}
    private void Die()
    {
        Debug.Log($"{Data.name} ���!");

        var player = PlayerStatsManager.Instance;   // �� ����
        if (player != null)
        {
            player.GainExp(Data.exp);
            Debug.Log($"�÷��̾ {Data.exp} EXP�� ȹ��!");
        }

        dropManager?.DropItems();
        Destroy(gameObject);
    }


    public void Heal(float amount)
    {
        if (CurrentHP <= 0) return;
        CurrentHP = Mathf.Min(CurrentHP + amount, Data.hp);
    }
}
