
using System;
using System.Collections.Generic;
using System.Linq;

public class DataTableManager : SingletonBehaviour<DataTableManager>
{
    // ������ �������� ���
    private const string DATA_PATH = "DataTable";

    // ������ ���̺� ���� ���� �̸�
    // é�͵�����
    private const string CHAPTER_DATA_TABLE = "ChapterDataTable";
    private List<ChapterData> ChapterDataTable = new List<ChapterData>();

    protected override void Init()
    {
        base.Init();

        LoadChapterDataTable();
        // otherDtatTable();
    }

    private void LoadChapterDataTable()
    {
        // é�� ������ ���� �о����
        // Read �� �̹� ����Ƽ�� �ִ� Resources.Load �Լ��� �־ Resources ������ �ȳ־ ��
        var parseDataTable = CSVReader.Read($"{DATA_PATH}/{CHAPTER_DATA_TABLE}"); 

        foreach (var data in parseDataTable)
        {
            ChapterData chapterData = new ChapterData() // �����ڸ��� �ʱ�ȭ
            {
                ChapterNo = Convert.ToInt32(data["chapter_no"]),
                TotalStage = Convert.ToInt32(data["total_stages"]),
                ChapterRewordGem = Convert.ToInt32(data["chapter_reward_gem"]),
                ChapterRewordGold = Convert.ToInt32(data["chpater_reward_gold"]),
            };

            ChapterDataTable.Add(chapterData);
        }
    }

    public ChapterData GetChapterData(int chapterNo)
    {
        foreach (ChapterData item in ChapterDataTable)
        {
            if (item.ChapterNo == chapterNo)
            {
                return item;
            }
        }

        return null;

        // return ChapterDataTable.Where(item => item.ChapterNo == chapterNo).FirstOrDefault(); // Linq
    }
}

public class ChapterData
{
    public int ChapterNo;
    public int TotalStage;
    public int ChapterRewordGem;
    public int ChapterRewordGold;
}