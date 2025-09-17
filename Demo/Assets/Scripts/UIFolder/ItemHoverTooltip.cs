using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private InventoryItem item;

    public void SetItem(InventoryItem it) => item = it;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            ItemTooltipUI.Instance?.Show(item, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.UpdatePosition(eventData.position);
    }
}