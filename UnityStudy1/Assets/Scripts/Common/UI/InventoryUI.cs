using Gpm.Ui;
using TMPro;
using UnityEngine;

public enum InventorySortType
{
    ItemGrade,
    ItemType,
}

public class InventoryUI : BaseUI
{
    public InfiniteScroll InventoryScrollList;
    public TextMeshProUGUI SortButtonText;

    private InventorySortType m_inventorySortType = InventorySortType.ItemGrade;

    public override void SetInfo(BaseUIData uiData)
    {
        base.SetInfo(uiData);

        SetInventory();
        SortInventory();
    }

    private void SetInventory()
    {
        InventoryScrollList.Clear();

        UserInventoryData userInventoryData = UserDataManager.Instance.GetUserData<UserInventoryData>();
        if (userInventoryData != null)
        {
            foreach (var item in userInventoryData.InventoryItemDataList)
            {
                var itemSlotData = new InventoryItemSlotData();
                itemSlotData.SerialNumber = item.SerialNumber;
                itemSlotData.ItemId = item.ItemId;
                InventoryScrollList.InsertData(itemSlotData);
            }
        }
    }

    private void SortInventory()
    {
        switch (m_inventorySortType)
        {
            case InventorySortType.ItemGrade:
                SortButtonText.text = "��޺�";

                InventoryScrollList.SortDataList((a, b) =>
                {
                    //1'1'001
                    var itemA = a.data as InventoryItemSlotData;
                    var itemB = b.data as InventoryItemSlotData;

                    // ��޺� ����
                    // B�� ������� A�� �ϴ°� ��������
                    // A�� ������� B�� �ϴ°� ��������
                    int compareResult = ((itemB.ItemId / 1000) % 10).CompareTo((itemA.ItemId / 1000) % 10); // 0, -1, 1

                    if (compareResult == 0) // �� ����� �����ϴٸ�
                    {
                        // �������� �������ַ��� �ϴµ� ��޿� ���� �κ��� �����ϰ� ����
                        // ������ Weapon, Shield ..... ������ ����ǰ� (��������)
                        var itemAIdString = itemA.ItemId.ToString();
                        var itemAComp = itemAIdString.Substring(0, 1) + itemAIdString.Substring(2,3); // 11001  =>  1001

                        var itemBIdString = itemB.ItemId.ToString();
                        var itemBComp = itemBIdString.Substring(0, 1) + itemBIdString.Substring(2, 3); // 21001  =>  2001

                        compareResult = itemAComp.CompareTo(itemBComp); // 0, -1, 1
                    }

                    return compareResult;
                });
                break;
            case InventorySortType.ItemType:
                SortButtonText.text = "������";

                InventoryScrollList.SortDataList((a, b) =>
                {
                    var itemA = a.data as InventoryItemSlotData;
                    var itemB = b.data as InventoryItemSlotData;

                    var itemAIdString = itemA.ItemId.ToString();
                    var itemAComp = itemAIdString.Substring(0, 1) + itemAIdString.Substring(2, 3); // 11001  =>  1001

                    var itemBIdString = itemB.ItemId.ToString();
                    var itemBComp = itemBIdString.Substring(0, 1) + itemBIdString.Substring(2, 3); // 21001  =>  2001

                    int compareResult = itemAComp.CompareTo(itemBComp);

                    if (compareResult == 0) // �� ������ �����ϴٸ�
                    {
                        compareResult = ((itemB.ItemId / 1000) % 10).CompareTo((itemA.ItemId / 1000) % 10);
                    }

                    return compareResult;
                });
                break;
            default:
                break;
        }
    }

    public void OnClickSortButton()
    {
        switch (m_inventorySortType)
        {
            case InventorySortType.ItemGrade:
                m_inventorySortType = InventorySortType.ItemType;
                break;
            case InventorySortType.ItemType:
                m_inventorySortType = InventorySortType.ItemGrade;
                break;
            default:
                break;
        }

        SortInventory();
    }
}
