using System;
using System.Collections.Generic;
using System.Linq;

public class DataTableManager : SingletonBehaviour<DataTableManager>
{
    private const string DATA_PATH = "DataTable";


    

    protected override void Init()
    {
        base.Init();

        LoadChapterDataTable();
        LoaditemDataTable();
    }

    #region CHAPTER_DATA
    private const string CHAPTER_DATA_TABLE = "ChapterDataTable";
    private List<ChapterData> ChapterDataTable = new List<ChapterData>();

    private void LoadChapterDataTable()
    {
        var parsedDataTable = CSVReader.Read($"{DATA_PATH}/{CHAPTER_DATA_TABLE}");

        foreach (var data in parsedDataTable)
        {
            var chapterData = new ChapterData
            {
                ChapterNo = Convert.ToInt32(data["chapter_no"]),
                TotalStages = Convert.ToInt32(data["total_stages"]),
                ChapterRewardGem = Convert.ToInt32(data["chapter_reward_gem"]),
                ChapterRewardGold = Convert.ToInt32(data["chapter_reward_gold"]),
            };

            ChapterDataTable.Add(chapterData);
        }
    }

    public ChapterData GetChapterData(int chapterNo)
    {
        foreach (var item in ChapterDataTable)
        {
            if (item.ChapterNo == chapterNo)
                return item;
        }

        return null;
    }
    #endregion

    #region ITEM_DATA
    private const string ITEM_DATA_TABLE = "ItemDataTable";
    private List<ItemData> ItemDataTable = new List<ItemData>();

    private void LoaditemDataTable()
    {
        var parsedDataTable = CSVReader.Read($"{DATA_PATH}/{ITEM_DATA_TABLE}");

        foreach (var data in parsedDataTable)
        {
            var itemData = new ItemData
            {
                Itemid = Convert.ToInt32(data["item_id"]),
                ItemName = data["item_name"].ToString(),
                AttackPower = Convert.ToInt32(data["attack_power"]),
                Defense = Convert.ToInt32(data["defense"]),
            };

            ItemDataTable.Add(itemData);
        }
    }

    public ItemData GetItemData(int itemId)
    {
        foreach (var item in ItemDataTable)
        {
            if (item.Itemid == itemId)
                return item;
        }

        return null;
    }
    #endregion
}

public class ChapterData
{
    public int ChapterNo;
    public int TotalStages;
    public int ChapterRewardGem;
    public int ChapterRewardGold;
}

public class ItemData
{
    public int Itemid;
    public string ItemName;
    public int AttackPower;
    public int Defense;
}

public enum ItemType
{
    Weapon = 1,
    Shield,
    ChestArmor,
    Gloves,
    Boots,
    Accessory,
}

public enum ItemGrade
{
    Common = 1,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
