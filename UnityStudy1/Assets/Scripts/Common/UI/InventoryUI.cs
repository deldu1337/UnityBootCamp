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
                SortButtonText.text = "등급별";

                InventoryScrollList.SortDataList((a, b) =>
                {
                    //1'1'001
                    var itemA = a.data as InventoryItemSlotData;
                    var itemB = b.data as InventoryItemSlotData;

                    // 등급별 정렬
                    // B를 대상으로 A를 하는건 내림차순
                    // A를 대상으로 B를 하는건 오름차순
                    int compareResult = ((itemB.ItemId / 1000) % 10).CompareTo((itemA.ItemId / 1000) % 10); // 0, -1, 1

                    if (compareResult == 0) // 두 등급이 동일하다면
                    {
                        // 종류별로 정렬해주려고 하는데 등급에 대한 부분은 제외하고 정렬
                        // 순서는 Weapon, Shield ..... 순서로 진행되게 (오름차순)
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
                SortButtonText.text = "종류별";

                InventoryScrollList.SortDataList((a, b) =>
                {
                    var itemA = a.data as InventoryItemSlotData;
                    var itemB = b.data as InventoryItemSlotData;

                    var itemAIdString = itemA.ItemId.ToString();
                    var itemAComp = itemAIdString.Substring(0, 1) + itemAIdString.Substring(2, 3); // 11001  =>  1001

                    var itemBIdString = itemB.ItemId.ToString();
                    var itemBComp = itemBIdString.Substring(0, 1) + itemBIdString.Substring(2, 3); // 21001  =>  2001

                    int compareResult = itemAComp.CompareTo(itemBComp);

                    if (compareResult == 0) // 두 종류가 동일하다면
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
