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

        // placeholder는 필요 없음 (버튼은 고정)
        canvasGroup.blocksRaycasts = false;

        // 아이콘만 드래그 (버튼이 아니라)
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 가장 가까운 슬롯 찾기
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

        // 아이콘 원래 자리로 복귀
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);

        canvasGroup.blocksRaycasts = true;

        // 데이터만 교체
        playerInventory.SwapInventoryData(originalIndex, closestIndex);
    }
}