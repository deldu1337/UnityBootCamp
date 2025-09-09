using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<int, int> onItemDropped; // (fromIndex, toIndex) 전달
    public Action<int> onItemRemoved;      // 인벤토리 외부로 드래그 시 호출 (fromIndex)

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
    //    // 가장 가까운 슬롯 찾기
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

    //    // 원래 위치로 복귀
    //    transform.SetParent(originalParent);
    //    transform.SetSiblingIndex(originalIndex);
    //    canvasGroup.blocksRaycasts = true;

    //    // Presenter에 데이터 스왑 요청
    //    onItemDropped?.Invoke(originalIndex, closestIndex);
    //}
    public void OnEndDrag(PointerEventData eventData)
    {
        // 슬롯 영역 밖으로 드래그했는지 체크
        bool isOutside = !RectTransformUtility.RectangleContainsScreenPoint(
            originalParent as RectTransform,
            eventData.position,
            canvas.worldCamera);

        if (isOutside)
        {
            // UI 비활성화
            gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = true;

            // 비활성화 후 원래 부모의 맨 아래로 이동
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalIndex);

            // 인벤토리에서 모델 삭제
            onItemRemoved?.Invoke(originalIndex);

            Debug.Log("OnEndDrag - Removed Item (Deactivated and moved to bottom)");
        }
        else
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

            // 원래 위치로 복귀
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
            canvasGroup.blocksRaycasts = true;

            // Presenter에 데이터 스왑 요청
            onItemDropped?.Invoke(originalIndex, closestIndex);
        }
    }

}
