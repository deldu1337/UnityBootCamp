using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryItem item;
    private RectTransform selfRect;

    void Awake()
    {
        selfRect = transform as RectTransform;
    }

    // �г�(������ ����/â)�� ��Ȱ��ȭ�Ǹ� ��� ���� ����
    void OnDisable()
    {
        if (ItemTooltipUI.Instance != null)
            ItemTooltipUI.Instance.Hide(this);
    }

    public void SetItem(InventoryItem it) => item = it;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
            ItemTooltipUI.Instance?.ShowNextTo(item, selfRect, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipUI.Instance?.Hide();
    }
}
