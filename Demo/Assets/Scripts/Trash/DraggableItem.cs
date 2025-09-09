using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PlayerInventory playerInventory;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalIndex;
    private GameObject placeholder;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();

        // placeholder�� �ʿ� ���� (��ư�� ����)
        canvasGroup.blocksRaycasts = false;

        // �����ܸ� �巡�� (��ư�� �ƴ϶�)
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ���� ����� ���� ã��
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < originalParent.childCount; i++)
        {
            float dist = Vector2.SqrMagnitude(eventData.position - (Vector2)originalParent.GetChild(i).position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        // ������ ���� �ڸ��� ����
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);

        canvasGroup.blocksRaycasts = true;

        // �����͸� ��ü
        playerInventory.SwapInventoryData(originalIndex, closestIndex);
    }
}