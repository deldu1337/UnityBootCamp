using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;
    public GameObject damageTextPrefab;
    public Canvas canvas;

    void Awake()
    {
        Instance = this;
    }

    public enum DamageTextTarget
    {
        Enemy,
        Player
    }

    /// <summary>
    /// ��� Transform�� �����Ǵ� ������ �ؽ�Ʈ ����(����)
    /// </summary>
    public void ShowDamage(Transform target, int damage, Color color, DamageTextTarget type)
    {
        if (!target || damageTextPrefab == null || canvas == null) return;

        // ��� ������ ���� ������(�Ӹ� �� ��)
        Vector3 worldOffset = Vector3.up * 1.5f;

        // ĵ���� ������ ����
        GameObject go = Instantiate(damageTextPrefab, canvas.transform);

        // �ٷ� ��ġ ���� �ʿ� ����, DamageText�� �� ������ target ����
        var dt = go.GetComponent<DamageText>();
        if (dt != null)
            dt.Setup(damage, color, target, worldOffset, Camera.main);
    }

    // [����] ������ ȣȯ: worldPos�ε� ȣ�� ���������� '����'�� �� �� (���� ����)
    public void ShowDamage(Vector3 worldPos, int damage, Color color, DamageTextTarget type)
    {
        if (damageTextPrefab == null || canvas == null || Camera.main == null) return;

        // worldPos�� ��ũ������ ��ȯ�Ͽ� 1�����Ӹ� �������� ���(���� ���)
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up * 1.5f);
        GameObject go = Instantiate(damageTextPrefab, canvas.transform);
        go.transform.position = screenPos;

        var dt = go.GetComponent<DamageText>();
        if (dt != null)
        {
            // ���� ����� ������ null ����(ȭ�� �������θ� �ִϸ��̼�)
            dt.Setup(damage, color, null, Vector3.zero, Camera.main);
        }
    }
}
