using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TileMapGenerator mapGen;
    [SerializeField] private Text stageText;

    [Header("Stage")]
    public int currentStage = 1;
    public int bossEvery = 5;

    void Start()
    {
        UpdateStageUI();
    }

    public void NextStage()
    {
        currentStage++;
        UpdateStageUI();
        if (mapGen != null) mapGen.ReloadMap();
    }

    public bool IsBossStage() => (bossEvery > 0) && (currentStage % bossEvery == 0);

    public void UpdateStageUI()
    {
        if (stageText) stageText.text = $"Stage {currentStage}";
    }
}
