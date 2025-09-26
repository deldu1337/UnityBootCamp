
using System;
using System.Collections.Generic;
using System.Linq;

public class DataTableManager : SingletonBehaviour<DataTableManager>
{
    // 가져올 데이터의 경로
    private const string DATA_PATH = "DataTable";

    // 데이터 테이블 실제 파일 이름
    // 챕터데이터
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
        // 챕터 데이터 파일 읽어오기
        // Read 에 이미 유니티에 있는 Resources.Load 함수가 있어서 Resources 폴더는 안넣어도 됨
        var parseDataTable = CSVReader.Read($"{DATA_PATH}/{CHAPTER_DATA_TABLE}"); 

        foreach (var data in parseDataTable)
        {
            ChapterData chapterData = new ChapterData() // 만들자마자 초기화
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