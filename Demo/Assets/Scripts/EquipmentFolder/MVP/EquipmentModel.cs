using UnityEngine;

public class EquipmentModel
{
    public bool IsOpen { get; private set; }

    public EquipmentModel()
    {
        IsOpen = false;
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
    }

    public void Close()
    {
        IsOpen = false;
    }
}
