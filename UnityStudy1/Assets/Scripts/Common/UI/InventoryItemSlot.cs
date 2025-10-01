using Gpm.Ui;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class InventoryItemSlotData : InfiniteScrollData  // ���Ǵ�Ƽ ��ũ�ѿ� ���� �����۵��� �������� ������ �κ�
{
    public long SerialNumber;
    public int ItemId; // 11001
}

public class InventoryItemSlot : InfiniteScrollItem // ���Ǵ�Ƽ ��ũ�ѿ� ���� ������ �� ��ü
{
    public Image _itemGradeBg;
    public Image _itemIcon;

    private InventoryItemSlotData m_inventoryItemSlotData;

    public override void UpdateData(InfiniteScrollData scrollData) // ���Ǵ�Ƽ ��ũ���� ��ũ�Ѹ� �ҋ� ���� ������Ʈ�� �����͸� �����ϴµ� ���� ����
    {
        base.UpdateData(scrollData);

        m_inventoryItemSlotData = scrollData as InventoryItemSlotData;
        if (m_inventoryItemSlotData == null)
        {
            Logger.LogError("m_inventoryItemSlotData is invalid");
            return;
        }

        // ������ ��׶��带 �������� �۾�(���)
        ItemGrade itemGrade = (ItemGrade)((m_inventoryItemSlotData.ItemId / 1000) % 10); // 21002 / 1000 => 24 % 10 => 4
        Texture2D gradeBgTexture = Resources.Load<Texture2D>($"Textures/{itemGrade}");
        if (gradeBgTexture != null)
        {
            _itemGradeBg.sprite = Sprite.Create(gradeBgTexture, new Rect(0, 0, gradeBgTexture.width, gradeBgTexture.height), new Vector2(1f, 1f));
        }

        // �������� ������ �������� �������� �۾�(����)
        StringBuilder sb = new StringBuilder(m_inventoryItemSlotData.ItemId.ToString());
        sb[1] = '1'; // ex) _itemId �� ���� 22001 �̶��, ������ �����̸��� ��ġ��Ű�� ���� 2��° �ڸ��� 1�� ���� �ٲ��� => 2'1'001 
        string itemIconName = sb.ToString();

        Texture2D itemIconTexture = Resources.Load<Texture2D>($"Textures/{itemIconName}");
        if (itemIconTexture != null)
        {
            _itemIcon.sprite = Sprite.Create(itemIconTexture, new Rect(0, 0, itemIconTexture.width, itemIconTexture.height), new Vector2(1f, 1f));
        }
    }
}
