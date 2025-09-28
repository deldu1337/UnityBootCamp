using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Offsets")]
    [Tooltip("�⺻ ī�޶� ������(�÷��̾� ����). ���� �� ���� ī�޶� ������ ��� �ɼ��� ���������� ���õ˴ϴ�.")]
    public Vector3 baseOffset = new Vector3(-2f, 16.5f, -2f);

    [Tooltip("���� ���� �� (���� ī�޶� ��ġ - �÷��̾� ��ġ)�� ������ �ڵ� ����")]
    public bool useCurrentCameraOffsetOnStart = true;

    [Tooltip("���� ������ Ȯ���(�־���) ������ ���")]
    public float zoomOutMultiplier = 1.7f;

    [Header("Boss Zoom")]
    [Tooltip("������ �� �Ÿ� �̳��� �����ϸ� �ܾƿ� ����")]
    public float bossTriggerRadius = 40f;

    [Tooltip("������ ��ȯ �ӵ�(�ʴ�). ���� Ŭ���� ������ ��ȯ")]
    public float zoomLerpSpeed = 7f;

    [Tooltip("���� Ž���� ����� �±�")]
    public string bossTag = "Boss";

    [Header("Rotation (optional)")]
    public bool lockRotation = false;
    public Vector3 lockedEuler = new Vector3(55f, 45f, 0f);

    private Transform camT;
    private Transform nearestBoss;

    private Vector3 currentOffset; // ī�޶� �� ������ ��� ����� ������(���� ����)

    void Awake()
    {
        camT = Camera.main ? Camera.main.transform : null;
        if (!camT) Debug.LogWarning("[PlayerCamera] Main Camera�� ã�� ���߽��ϴ�.");
    }

    void Start()
    {
        if (!camT)
        {
            var main = Camera.main;
            if (main) camT = main.transform;
        }

        // ���� �� ���� ��ũ��Ʈó�� ���� ī�޶� ���� ������ ���
        if (camT && useCurrentCameraOffsetOnStart)
            baseOffset = camT.position - transform.position;

        currentOffset = baseOffset;

        // ù �����Ӻ��� ��Ȯ�� ���̱�
        if (camT) camT.position = transform.position + currentOffset;
        if (camT && lockRotation) camT.rotation = Quaternion.Euler(lockedEuler);
    }

    void FixedUpdate() // �÷��̾� �̵� �� ī�޶� �����
    {
        if (!camT) return;

        // 1) ���� ����� ���� ã��
        UpdateNearestBoss();

        // 2) ��ǥ ������ ���(���� �Ÿ� ���)
        Vector3 targetOffset = baseOffset;
        if (nearestBoss)
        {
            float d = Vector3.Distance(transform.position, nearestBoss.position);
            float t = 1f - Mathf.Clamp01(d / bossTriggerRadius); // 0(��)~1(����)
            targetOffset = Vector3.Lerp(baseOffset, baseOffset * zoomOutMultiplier, t);
        }

        // 3) �����¸� �ε巴�� ���� (ī�޶� ��ġ ��ü�� ��� ����)
        //    ������ ����: �����ӷ���Ʈ�� �����ϰ� ������ ��ȯ��
        float k = 1f - Mathf.Exp(-zoomLerpSpeed * Time.deltaTime);
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, k);

        // 4) ī�޶� ��ġ�� '���' ���� �� �÷��̾�� �����ϰ� ������(���� ����)
        camT.position = transform.position + currentOffset;

        if (lockRotation)
            camT.rotation = Quaternion.Euler(lockedEuler);
    }

    private void UpdateNearestBoss()
    {
        GameObject[] bosses = GameObject.FindGameObjectsWithTag(bossTag);
        float best = float.MaxValue;
        Transform bestT = null;

        foreach (var b in bosses)
        {
            float d = Vector3.Distance(transform.position, b.transform.position);
            if (d < best) { best = d; bestT = b.transform; }
        }
        nearestBoss = bestT;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, bossTriggerRadius);
    }
#endif
}

