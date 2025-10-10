//using UnityEngine;
//using UnityEngine.UI;

//[RequireComponent(typeof(Button))]
//public class UIPanelSwitcher : MonoBehaviour
//{
//    [Header("����(From) �г� ��Ʈ - ����θ� �ڵ����� ã��")]
//    [SerializeField] private RectTransform fromPanelRoot;

//    [Header("��ȯ ���(To) �г� ��Ʈ")]
//    [SerializeField] private RectTransform toPanelRoot;

//    [Header("�ɼ�")]
//    [SerializeField] private bool copyScaleAndRotation = true;
//    [SerializeField] private bool matchSiblingIndex = true;

//    private Button btn;

//    void Awake()
//    {
//        btn = GetComponent<Button>();
//        btn.onClick.AddListener(Switch);

//        if (!fromPanelRoot)
//            fromPanelRoot = FindRootPanelUnderCanvas(transform as RectTransform);

//        if (!fromPanelRoot || !toPanelRoot)
//            Debug.LogError("[UIPanelSwitcher] fromPanelRoot / toPanelRoot ��(��) �����ϼ���.");
//    }

//    /// <summary>
//    /// ��ư Ŭ�� �� toPanel�� ��ȯ
//    /// </summary>
//    public void Switch()
//    {
//        if (!fromPanelRoot || !toPanelRoot) return;

//        // ���� �θ�(Canvas ����)�� ����
//        if (toPanelRoot.parent != fromPanelRoot.parent)
//            toPanelRoot.SetParent(fromPanelRoot.parent, worldPositionStays: false);

//        // ���̾ƿ� ���� (��Ŀ/�ǹ�/ũ��/��ġ)
//        CopyLayout(fromPanelRoot, toPanelRoot);

//        // �ʿ� �� ������/�����̼�/���� �ε������� ����ȭ
//        if (copyScaleAndRotation)
//        {
//            toPanelRoot.localScale = fromPanelRoot.localScale;
//            toPanelRoot.localRotation = fromPanelRoot.localRotation;
//        }
//        if (matchSiblingIndex)
//            toPanelRoot.SetSiblingIndex(fromPanelRoot.GetSiblingIndex());

//        // Ȱ��/��Ȱ�� ��ȯ
//        toPanelRoot.gameObject.SetActive(true);
//        fromPanelRoot.gameObject.SetActive(false);
//    }

//    /// <summary>
//    /// ���� ��ư�� ���� Ʈ������ Canvas �ٷ� �Ʒ��� "�г� ��Ʈ" RectTransform�� ã�� ��ȯ
//    /// (��: HeadPanel �� EquipmentUI / PlayerInfoUI)
//    /// </summary>
//    private RectTransform FindRootPanelUnderCanvas(RectTransform start)
//    {
//        RectTransform cur = start;
//        while (cur && cur.parent is RectTransform parentRT)
//        {
//            // �θ� Canvas�� cur�� �г� ��Ʈ
//            if (parentRT.GetComponent<Canvas>() != null)
//                return cur;
//            cur = parentRT;
//        }
//        return null;
//    }

//    private void CopyLayout(RectTransform from, RectTransform to)
//    {
//        to.anchorMin = from.anchorMin;
//        to.anchorMax = from.anchorMax;
//        to.pivot = from.pivot;

//        to.sizeDelta = from.sizeDelta;
//        to.anchoredPosition3D = from.anchoredPosition3D;
//    }

//    // �ܺο��� �����ϰ� ȣ�� ������ ��ƿ (��ư ���̵� ���)
//    public static void CopyLayoutRT(RectTransform from, RectTransform to,
//        bool copyScaleAndRotation = true, bool matchSiblingIndex = true)
//    {
//        if (!from || !to) return;
//        to.anchorMin = from.anchorMin;
//        to.anchorMax = from.anchorMax;
//        to.pivot = from.pivot;
//        to.sizeDelta = from.sizeDelta;
//        to.anchoredPosition3D = from.anchoredPosition3D;

//        if (copyScaleAndRotation)
//        {
//            to.localScale = from.localScale;
//            to.localRotation = from.localRotation;
//        }
//        if (matchSiblingIndex)
//            to.SetSiblingIndex(from.GetSiblingIndex());
//    }

//    // ===== ������ ����� (�߰�) =====
//    private static bool hasSnapshot;
//    private static Vector2 s_anchorMin, s_anchorMax, s_pivot, s_sizeDelta;
//    private static Vector3 s_anchoredPos3D, s_scale;
//    private static Quaternion s_rotation;
//    private static int s_siblingIndex;

//    public static void SaveSnapshot(RectTransform rt)
//    {
//        if (!rt) return;
//        s_anchorMin = rt.anchorMin;
//        s_anchorMax = rt.anchorMax;
//        s_pivot = rt.pivot;
//        s_sizeDelta = rt.sizeDelta;
//        s_anchoredPos3D = rt.anchoredPosition3D;
//        s_scale = rt.localScale;
//        s_rotation = rt.localRotation;
//        s_siblingIndex = rt.GetSiblingIndex();
//        hasSnapshot = true;
//    }

//    public static void LoadSnapshot(RectTransform rt, bool applySiblingIndex = true)
//    {
//        if (!rt || !hasSnapshot) return;
//        rt.anchorMin = s_anchorMin;
//        rt.anchorMax = s_anchorMax;
//        rt.pivot = s_pivot;
//        rt.sizeDelta = s_sizeDelta;
//        rt.anchoredPosition3D = s_anchoredPos3D;
//        rt.localScale = s_scale;
//        rt.localRotation = s_rotation;
//        if (applySiblingIndex) rt.SetSiblingIndex(s_siblingIndex);
//    }

//    public static bool HasSnapshot => hasSnapshot;
//}
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIPanelSwitcher : MonoBehaviour
{
    [Header("����(From) �г� ��Ʈ - ����θ� �ڵ����� ã��")]
    [SerializeField] private RectTransform fromPanelRoot;

    [Header("��ȯ ���(To) �г� ��Ʈ")]
    [SerializeField] private RectTransform toPanelRoot;

    [Header("�ɼ�")]
    [SerializeField] private bool copyScaleAndRotation = true;
    [SerializeField] private bool matchSiblingIndex = true;

    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Switch);

        if (!fromPanelRoot)
            fromPanelRoot = FindRootPanelUnderCanvas(transform as RectTransform);

        if (!fromPanelRoot || !toPanelRoot)
            Debug.LogError("[UIPanelSwitcher] fromPanelRoot / toPanelRoot ��(��) �����ϼ���.");
    }

    /// <summary>
    /// ��ư Ŭ�� �� toPanel�� ��ȯ
    /// </summary>
    public void Switch()
    {
        if (!fromPanelRoot || !toPanelRoot) return;

        // ���� �θ�(Canvas ����)�� ����
        if (toPanelRoot.parent != fromPanelRoot.parent)
            toPanelRoot.SetParent(fromPanelRoot.parent, worldPositionStays: false);

        // ���̾ƿ� ���� (��Ŀ/�ǹ�/ũ��/��ġ)
        CopyLayout(fromPanelRoot, toPanelRoot);

        // �ʿ� �� ������/�����̼�/���� �ε������� ����ȭ
        if (copyScaleAndRotation)
        {
            toPanelRoot.localScale = fromPanelRoot.localScale;
            toPanelRoot.localRotation = fromPanelRoot.localRotation;
        }
        if (matchSiblingIndex)
            toPanelRoot.SetSiblingIndex(fromPanelRoot.GetSiblingIndex());

        // Ȱ��/��Ȱ�� ��ȯ
        toPanelRoot.gameObject.SetActive(true);
        fromPanelRoot.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���� ��ư�� ���� Ʈ������ Canvas �ٷ� �Ʒ��� "�г� ��Ʈ" RectTransform�� ã�� ��ȯ
    /// (��: HeadPanel �� EquipmentUI / PlayerInfoUI)
    /// </summary>
    private RectTransform FindRootPanelUnderCanvas(RectTransform start)
    {
        RectTransform cur = start;
        while (cur && cur.parent is RectTransform parentRT)
        {
            // �θ� Canvas�� cur�� �г� ��Ʈ
            if (parentRT.GetComponent<Canvas>() != null)
                return cur;
            cur = parentRT;
        }
        return null;
    }

    private void CopyLayout(RectTransform from, RectTransform to)
    {
        to.anchorMin = from.anchorMin;
        to.anchorMax = from.anchorMax;
        to.pivot = from.pivot;

        to.sizeDelta = from.sizeDelta;
        to.anchoredPosition3D = from.anchoredPosition3D;
    }

    // ---- �ܺ� ��ƿ (�θ���� �⺻������ ������) ----
    public static void CopyLayoutRT(
        RectTransform from, RectTransform to,
        bool ensureSameParent = true,
        bool copyScaleAndRotation = true,
        bool matchSiblingIndex = true)
    {
        if (!from || !to) return;

        if (ensureSameParent && to.parent != from.parent)
            to.SetParent(from.parent, worldPositionStays: false);

        to.anchorMin = from.anchorMin;
        to.anchorMax = from.anchorMax;
        to.pivot = from.pivot;
        to.sizeDelta = from.sizeDelta;
        to.anchoredPosition3D = from.anchoredPosition3D;

        if (copyScaleAndRotation)
        {
            to.localScale = from.localScale;
            to.localRotation = from.localRotation;
        }
        if (matchSiblingIndex)
            to.SetSiblingIndex(from.GetSiblingIndex());
    }

    // ===== ������ ����� =====

    // ----- UIPanelSwitcher.cs �� ������ �ʵ忡 �߰� -----
    private static Vector3 s_localPos;   // �� �߰�: �巡�װ� �ٲٴ� ��

    private static bool hasSnapshot;
    private static Vector2 s_anchorMin, s_anchorMax, s_pivot, s_sizeDelta;
    private static Vector3 s_anchoredPos3D, s_scale;
    private static Quaternion s_rotation;
    private static int s_siblingIndex;
    private static RectTransform s_parent; // �� �θ���� ����

    // ----- SaveSnapshot ���� -----
    public static void SaveSnapshot(RectTransform rt)
    {
        if (!rt) return;
        s_parent = rt.parent as RectTransform;
        s_anchorMin = rt.anchorMin;
        s_anchorMax = rt.anchorMax;
        s_pivot = rt.pivot;
        s_sizeDelta = rt.sizeDelta;
        s_anchoredPos3D = rt.anchoredPosition3D;
        s_localPos = rt.localPosition;   // �� �߰�
        s_scale = rt.localScale;
        s_rotation = rt.localRotation;
        s_siblingIndex = rt.GetSiblingIndex();
        hasSnapshot = true;
    }

    // ----- LoadSnapshot ���� -----
    public static void LoadSnapshot(RectTransform rt, bool applySiblingIndex = true)
    {
        if (!rt || !hasSnapshot) return;

        if (s_parent && rt.parent != s_parent)
            rt.SetParent(s_parent, worldPositionStays: false);

        // ���̾ƿ� �Ӽ� ����
        rt.anchorMin = s_anchorMin;
        rt.anchorMax = s_anchorMax;
        rt.pivot = s_pivot;
        rt.sizeDelta = s_sizeDelta;

        // ��ġ ���� (localPosition �켱 �� ��Ŀ ��ǥ�� ��� ����)
        rt.localPosition = s_localPos;       // �� �ٽ�
        rt.anchoredPosition3D = s_anchoredPos3D;  // ���� �� ȣȯ

        rt.localScale = s_scale;
        rt.localRotation = s_rotation;

        if (applySiblingIndex) rt.SetSiblingIndex(s_siblingIndex);

        // ���̾ƿ� ���� ����(�θ� ���̾ƿ� ������Ʈ ���� �� �ʱ�ȭ ���� ����)
        Canvas.ForceUpdateCanvases();
        var prt = rt.parent as RectTransform;
        if (prt) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(prt);
    }

    // UIPanelSwitcher.cs ���� (Ŭ���� �ϴ� ��)
    public static void ClearSnapshot()
    {
        hasSnapshot = false;
        s_parent = null;
    }

    public static bool HasSnapshot => hasSnapshot;
}
