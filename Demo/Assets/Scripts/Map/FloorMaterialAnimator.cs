using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FloorMaterialAnimator : MonoBehaviour
{
    [Header("Frames (��1)")]
    [Tooltip("���� �ؽ�ó �迭�� ���")]
    public Texture2D[] textures;

    [Tooltip("��������Ʈ �迭�� ���(���ο��� texture ���)")]
    public Sprite[] sprites;

    [Header("Playback")]
    public float fps = 8f;
    public bool loop = true;
    public bool randomStartFrame = true;

    [Header("Auto Load (����)")]
    public bool autoLoadFromResources = false;
    [Tooltip("Resources ���� ��� (��: Textures/Floor)")]
    public string resourcesFolder = "Textures/Floor";
    [Tooltip("�� ���λ�� �����ϴ� ���¸� �ε� (��: Floor_)")]
    public string namePrefix = "Floor_";

    [Header("Shader Property")]
    [Tooltip("URP/Lit�� _BaseMap, ���� Standard�� _MainTex")]
    public string texturePropertyName = "_BaseMap";

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private int propId;
    private float timeAcc;
    private int index;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        propId = Shader.PropertyToID(texturePropertyName);

        if (autoLoadFromResources)
        {
            // 1) �ؽ�ó �켱 �ε�
            var texAll = Resources.LoadAll<Texture2D>(resourcesFolder);
            textures = System.Array.FindAll(texAll, t => t.name.StartsWith(namePrefix));
            System.Array.Sort(textures, (a, b) => string.CompareOrdinal(a.name, b.name));

            // 2) �ؽ�ó�� ������ ��������Ʈ �ε� �õ�
            if (textures == null || textures.Length == 0)
            {
                var sprAll = Resources.LoadAll<Sprite>(resourcesFolder);
                sprites = System.Array.FindAll(sprAll, s => s.name.StartsWith(namePrefix));
                System.Array.Sort(sprites, (a, b) => string.CompareOrdinal(a.name, b.name));
            }
        }

        int count = GetFrameCount();
        if (count == 0)
        {
            enabled = false;
            Debug.LogWarning($"{name}: FloorMaterialAnimator frames ���� �� ��Ȱ��ȭ");
            return;
        }

        if (randomStartFrame)
        {
            index = Random.Range(0, count);
            timeAcc = Random.value / Mathf.Max(fps, 0.0001f);
        }

        ApplyFrame(index);
    }

    void Update()
    {
        int count = GetFrameCount();
        if (count == 0 || fps <= 0f) return;

        timeAcc += Time.deltaTime;
        float frameDur = 1f / fps;

        while (timeAcc >= frameDur)
        {
            timeAcc -= frameDur;
            index++;
            if (index >= count)
            {
                if (loop) index = 0;
                else { index = count - 1; enabled = false; break; }
            }
            ApplyFrame(index);
        }
    }

    int GetFrameCount()
    {
        if (textures != null && textures.Length > 0) return textures.Length;
        if (sprites != null && sprites.Length > 0) return sprites.Length;
        return 0;
    }

    Texture GetFrameTexture(int i)
    {
        if (textures != null && textures.Length > 0) return textures[i];
        if (sprites != null && sprites.Length > 0)
        {
            // ��������Ʈ ��Ʈ�� ��쿡�� �� �ؽ�ó�� ���
            return sprites[i] ? sprites[i].texture : null;
        }
        return null;
    }

    void ApplyFrame(int i)
    {
        var tex = GetFrameTexture(i);
        if (tex == null) return;

        // _BaseMap ����
        rend.GetPropertyBlock(mpb);
        mpb.SetTexture(propId, tex);
        rend.SetPropertyBlock(mpb);

        // ǥ�� ���̴� ȣȯ (_MainTex)�� ���� ����(����)
        int mainId = Shader.PropertyToID("_MainTex");
        rend.GetPropertyBlock(mpb);
        mpb.SetTexture(mainId, tex);
        rend.SetPropertyBlock(mpb);
    }
}
