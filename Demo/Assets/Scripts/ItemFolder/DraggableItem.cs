using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PlayerInventory playerInventory;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalSiblingIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // CanvasGroup ������ �ڵ� �߰�
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // ���� Canvas ã��
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("DraggableItem: Canvas�� ã�� �� �����ϴ�!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;

        // �巡�� �߿� Canvas ��Ʈ�� �̵� �� �ֻ����� ǥ��
        rectTransform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // ���콺 ��ġ���� Raycast�� ���� Ȯ��
        GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
        if (hitObj != null)
        {
            DraggableItem targetItem = hitObj.GetComponent<DraggableItem>();
            if (targetItem != null && targetItem != this)
            {
                // ���� �θ�� ��ġ ��ȯ
                Transform targetParent = targetItem.transform.parent;
                int targetSibling = targetItem.transform.GetSiblingIndex();

                targetItem.transform.SetParent(originalParent, false);
                targetItem.transform.SetSiblingIndex(originalSiblingIndex);

                rectTransform.SetParent(targetParent, false);
                rectTransform.SetSiblingIndex(targetSibling);

                // **�κ��丮 ������ ���� ����**
                playerInventory.SwapInventoryData(originalSiblingIndex, targetSibling);
                return;
            }
        }

        // �ٸ� ��ư�� ������ ���� ��ġ�� ����
        rectTransform.SetParent(originalParent, false);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
    }
}