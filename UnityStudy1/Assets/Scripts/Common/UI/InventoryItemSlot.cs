using Gpm.Ui;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class InventoryItemSlotData : InfiniteScrollData  // 인피니티 스크롤에 들어가는 아이템들의 실질적인 데이터 부분
{
    public long SerialNumber;
    public int ItemId; // 11001
}

public class InventoryItemSlot : InfiniteScrollItem // 인피니티 스크롤에 들어가는 아이템 그 자체
{
    public Image _itemGradeBg;
    public Image _itemIcon;

    private InventoryItemSlotData m_inventoryItemSlotData;

    public override void UpdateData(InfiniteScrollData scrollData) // 인피니티 스크롤을 스크롤링 할떄 게임 오브젝트의 데이터를 변경하는데 끄때 사용됨
    {
        base.UpdateData(scrollData);

        m_inventoryItemSlotData = scrollData as InventoryItemSlotData;
        if (m_inventoryItemSlotData == null)
        {
            Logger.LogError("m_inventoryItemSlotData is invalid");
            return;
        }

        // 아이템 백그라운드를 가져오는 작업(등급)
        ItemGrade itemGrade = (ItemGrade)((m_inventoryItemSlotData.ItemId / 1000) % 10); // 21002 / 1000 => 24 % 10 => 4
        Texture2D gradeBgTexture = Resources.Load<Texture2D>($"Textures/{itemGrade}");
        if (gradeBgTexture != null)
        {
            _itemGradeBg.sprite = Sprite.Create(gradeBgTexture, new Rect(0, 0, gradeBgTexture.width, gradeBgTexture.height), new Vector2(1f, 1f));
        }

        // 이제부턴 아이템 아이콘을 가져오는 작업(종류)
        StringBuilder sb = new StringBuilder(m_inventoryItemSlotData.ItemId.ToString());
        sb[1] = '1'; // ex) _itemId 가 만약 22001 이라면, 아이템 파일이름과 일치시키기 위해 2번째 자리를 1로 전부 바꿔줌 => 2'1'001 
        string itemIconName = sb.ToString();

        Texture2D itemIconTexture = Resources.Load<Texture2D>($"Textures/{itemIconName}");
        if (itemIconTexture != null)
        {
            _itemIcon.sprite = Sprite.Create(itemIconTexture, new Rect(0, 0, itemIconTexture.width, itemIconTexture.height), new Vector2(1f, 1f));
        }
    }
}
