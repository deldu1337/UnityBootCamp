using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [Header("Animation")]
    public float duration = 1.0f;       // ��ü ��� �ð�(��)
    public float risePixels = 60f;      // ȭ�� �ȼ� ���� ���� �������� �� �Ÿ�
    public float horizontalDrift = 20f; // �¿�� ��¦ ��鸱 �ִ� �ȼ�

    private Text text;
    private float elapsed;
    private float driftX;
    private Color baseColor;

    // ���� ���/������/ī�޶�
    private Transform followTarget;
    private Vector3 worldOffset;
    private Camera cam;

    // �и�(Detach) ��� ����
    private bool detached = false;
    private float detachElapsed = 0f;
    private float detachDuration = 0.5f;       // �и� �� ������ �ð�(���� �ð� ������� ���)
    private Vector3 detachStartScreenPos;      // �и� ���� ȭ�� ��ǥ
    private float detachStartEase;             // �и� �������� ����� ease �� (0~1)
    private float currentAlpha = 1f;           // �и� ���� ����

    void Awake()
    {
        text = GetComponent<Text>();
        if (!text) Debug.LogWarning("[DamageText] Text ������Ʈ�� �����ϴ�.");
    }

    /// <summary>��� �����Ǵ� ������ �ؽ�Ʈ ����</summary>
    public void Setup(int damage, Color color, Transform target, Vector3 followWorldOffset, Camera cameraIfNullUseMain = null)
    {
        if (!text) return;

        text.text = damage.ToString();
        baseColor = new Color(color.r, color.g, color.b, 1f);
        text.color = baseColor;

        followTarget = target;
        worldOffset = followWorldOffset;
        cam = cameraIfNullUseMain ?? Camera.main;

        driftX = Random.Range(-horizontalDrift, horizontalDrift);

        elapsed = 0f;
        detached = false;
        detachElapsed = 0f;
        currentAlpha = 1f;

        // [�߿�] ���� ��� ���� ȭ�� ��ǥ�� ���� (ù ������ ���� �и��ŵ� ���� ��ġ�� ��Ȯ)
        if (followTarget && cam != null)
        {
            Vector3 baseScreen = cam.WorldToScreenPoint(followTarget.position + worldOffset);
            transform.position = baseScreen;
        }
    }


    void Update()
    {
        if (!text) return;

        // ����� ������ų� ��Ȱ��ȭ�Ǹ� �и� ���� ��ȯ
        if (!detached && (followTarget == null || !followTarget.gameObject.activeInHierarchy))
        {
            EnterDetachMode();
        }

        if (!detached)
        {
            // ===== ���� ��� =====
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 2f); // ease-out

            Vector3 baseScreen = transform.position; // fallback
            if (followTarget && cam != null)
                baseScreen = cam.WorldToScreenPoint(followTarget.position + worldOffset);

            float x = baseScreen.x + Mathf.Sin(t * Mathf.PI) * driftX * 0.3f;
            float y = baseScreen.y + Mathf.Lerp(0f, risePixels, ease);
            transform.position = new Vector3(x, y, 0f);

            currentAlpha = 1f - t;
            var c = baseColor; c.a = currentAlpha;
            text.color = c;

            if (elapsed >= duration)
                Destroy(gameObject);
        }
        else
        {
            // ===== �и� ��� =====
            detachElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(detachElapsed / detachDuration);
            float ease = 1f - Mathf.Pow(1f - t, 2f);

            // �и� ������ �ö� ��ŭ ������ '���� ��·�'�� �߰�
            float remainingRise = risePixels * (1f - detachStartEase);

            // X�� ����, Y�� ���� ��� ���� (Ƣ�� ���� ����)
            float x = detachStartScreenPos.x;
            float y = detachStartScreenPos.y + Mathf.Lerp(0f, remainingRise, ease);
            transform.position = new Vector3(x, y, 0f);

            // ���ĵ� �и� ������ currentAlpha���� 0���� ������
            var c = baseColor;
            c.a = Mathf.Lerp(currentAlpha, 0f, t);
            text.color = c;

            if (detachElapsed >= detachDuration)
                Destroy(gameObject);
        }
    }

    private void EnterDetachMode()
    {
        detached = true;

        // [�߿�] �и� ���� ��ǥ�� "���� transform.position" �״�� ���
        // (�̹� ����� ���/�帮��Ʈ�� �ٽ� ������ ����)
        detachStartScreenPos = transform.position;

        // �и� ���������� ���൵(ease)�� ����ؼ� ���� ��·� ��꿡�� Ȱ��
        float tSoFar = Mathf.Clamp01(elapsed / Mathf.Max(0.0001f, duration));
        detachStartEase = 1f - Mathf.Pow(1f - tSoFar, 2f);

        // ���� �ð�(�ʹ� ª���� �ּ� ����)
        float remainingTime = Mathf.Max(0f, duration - elapsed);
        detachDuration = Mathf.Max(remainingTime, 0.2f);

        // ���Ŀ� ��� ���� �ߴ�
        followTarget = null;
    }
}

