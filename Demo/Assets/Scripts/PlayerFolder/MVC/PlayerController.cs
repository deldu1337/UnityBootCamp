using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStatsManager stats;
    private EquipmentPresenter equipmentPresenter;

    void Awake()
    {
        stats = GetComponent<PlayerStatsManager>();
        equipmentPresenter = FindAnyObjectByType<EquipmentPresenter>();

        var loadedData = SaveLoadManager.LoadPlayerData();

        stats.LoadData(loadedData);

        if (equipmentPresenter != null)
            stats.RecalculateStats(equipmentPresenter.GetEquipmentSlots());
    }

    void OnApplicationQuit()
    {
        if (equipmentPresenter != null)
            stats.RecalculateStats(equipmentPresenter.GetEquipmentSlots());

        SaveLoadManager.SavePlayerData(stats.Data);
    }
}
