using UnityEngine;
using System.Collections.Generic;

public class WallTransparency : MonoBehaviour
{
    public Transform player;              // �÷��̾� Transform
    public Camera mainCamera;             // ���� ī�޶�
    public LayerMask wallLayer;           // �� ���̾�
    public float transparency = 0.3f;     // ����
    public float checkRadius = 0.5f;      // ���Ǿ�ĳ��Ʈ ������

    private Dictionary<Renderer, Material[]> originalMaterials = new();
    private List<Renderer> currentlyTransparent = new();

    void Update()
    {
        // ���� ���� ó�� �ʱ�ȭ
        foreach (var rend in currentlyTransparent)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.materials = originalMaterials[rend];
            }
        }
        currentlyTransparent.Clear();

        Vector3 direction = player.position - mainCamera.transform.position;
        float distance = direction.magnitude;

        // SphereCast�� ī�޶� �� �÷��̾� ���� �� ����
        if (Physics.SphereCast(mainCamera.transform.position, checkRadius, direction, out RaycastHit hit, distance, wallLayer))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                // ���� ���� ����
                if (!originalMaterials.ContainsKey(rend))
                {
                    originalMaterials[rend] = rend.materials;
                }

                // ���� ������ ����
                Material[] mats = rend.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Color c = mats[i].color;
                    c.a = transparency;
                    mats[i].color = c;
                    mats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mats[i].SetInt("_ZWrite", 0);
                    mats[i].DisableKeyword("_ALPHATEST_ON");
                    mats[i].EnableKeyword("_ALPHABLEND_ON");
                    mats[i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mats[i].renderQueue = 3000;
                }
                rend.materials = mats;

                currentlyTransparent.Add(rend);
            }
        }
    }
}
