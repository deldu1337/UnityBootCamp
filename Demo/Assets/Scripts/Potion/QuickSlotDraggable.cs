using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QuickSlotDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PotionSlotUI slot;          // �� �������� ���� ���� ����
    public Canvas canvas;              // �巡�׿�
    private RectTransform rt;
    private Transform originalParent;
    private CanvasGroup cg;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = gameObject.GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null || slot.IsEmpty) return;
        originalParent = transform.parent;
        cg.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slot == null || slot.IsEmpty) return;
        rt.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;

        var qb = PotionQuickBar.Instance;

        // 1) ���� ���� ���� ��� �� Move
        if (qb && qb.TryGetSlotIndexAtScreenPosition(eventData.position, out int targetIndex))
        {
            if (slot != null && targetIndex != slot.index)
                qb.Move(slot.index, targetIndex);

            SnapBack();
            return;
        }

        // 2) �� ��(����) �� �κ��丮�� ��ȯ
        if (slot != null)
            qb?.ReturnToInventory(slot.index);   // �� �ٽ� �� ��

        SnapBack();
    }

    private void SnapBack()
    {
        transform.SetParent(originalParent, false);
        rt.anchoredPosition = Vector2.zero;

        // �θ�(Potion1) �ȿ��� Text�� �׻� �� ���� ���� ����
        if (originalParent != null)
        {
            var qtyTr = originalParent.Find("Qty");
            if (qtyTr) qtyTr.SetAsLastSibling();

            var textTr = originalParent.Find("Text (Legacy)");
            if (textTr) textTr.SetAsLastSibling();
        }
    }
}
