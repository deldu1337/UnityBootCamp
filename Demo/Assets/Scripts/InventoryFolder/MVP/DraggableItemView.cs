using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<int, int> onItemDropped; // (fromIndex, toIndex) ����
    public Action<int> onItemRemoved;      // �κ��丮 �ܺη� �巡�� �� ȣ�� (fromIndex)

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private int originalIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Action<int, int> dropCallback, int slotIndex, Action<int> removeCallback = null)
    {
        onItemDropped = dropCallback;
        onItemRemoved = removeCallback;
        originalIndex = slotIndex;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    // ���� ����� ���� ã��
    //    int closestIndex = 0;
    //    float closestDistance = float.MaxValue;
    //    for (int i = 0; i < originalParent.childCount; i++)
    //    {
    //        float dist = Vector2.SqrMagnitude(eventData.position - (Vector2)originalParent.GetChild(i).position);
    //        if (dist < closestDistance)
    //        {
    //            closestDistance = dist;
    //            closestIndex = i;
    //        }
    //    }

    //    // ���� ��ġ�� ����
    //    transform.SetParent(originalParent);
    //    transform.SetSiblingIndex(originalIndex);
    //    canvasGroup.blocksRaycasts = true;

    //    // Presenter�� ������ ���� ��û
    //    onItemDropped?.Invoke(originalIndex, closestIndex);
    //}
    public void OnEndDrag(PointerEventData eventData)
    {
        // ���� ���� ������ �巡���ߴ��� üũ
        bool isOutside = !RectTransformUtility.RectangleContainsScreenPoint(
            originalParent as RectTransform,
            eventData.position,
            canvas.worldCamera);

        if (isOutside)
        {
            // UI ��Ȱ��ȭ
            gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = true;

            // ��Ȱ��ȭ �� ���� �θ��� �� �Ʒ��� �̵�
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);

            // �κ��丮���� �� ����
            onItemRemoved?.Invoke(originalIndex);

            Debug.Log("OnEndDrag - Removed Item (Deactivated and moved to bottom)");
        }
        else
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

            // ���� ��ġ�� ����
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            canvasGroup.blocksRaycasts = true;

            // Presenter�� ������ ���� ��û
            onItemDropped?.Invoke(originalIndex, closestIndex);
        }
    }

}
