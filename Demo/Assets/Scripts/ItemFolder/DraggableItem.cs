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

        // CanvasGroup 없으면 자동 추가
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 상위 Canvas 찾기
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("DraggableItem: Canvas를 찾을 수 없습니다!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;

        // 드래그 중엔 Canvas 루트로 이동 → 최상위로 표시
        rectTransform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 마우스 위치에서 Raycast로 슬롯 확인
        GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
        if (hitObj != null)
        {
            DraggableItem targetItem = hitObj.GetComponent<DraggableItem>();
            if (targetItem != null && targetItem != this)
            {
                // 서로 부모와 위치 교환
                Transform targetParent = targetItem.transform.parent;
                int targetSibling = targetItem.transform.GetSiblingIndex();

                targetItem.transform.SetParent(originalParent, false);
                targetItem.transform.SetSiblingIndex(originalSiblingIndex);

                rectTransform.SetParent(targetParent, false);
                rectTransform.SetSiblingIndex(targetSibling);

                // **인벤토리 데이터 순서 갱신**
                playerInventory.SwapInventoryData(originalSiblingIndex, targetSibling);
                return;
            }
        }

        // 다른 버튼이 없으면 원래 위치로 복귀
        rectTransform.SetParent(originalParent, false);
        rectTransform.SetSiblingIndex(originalSiblingIndex);
    }
}