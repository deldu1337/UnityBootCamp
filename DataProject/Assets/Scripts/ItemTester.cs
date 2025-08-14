using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public SOMaker somaker;

    void Start()
    {
        Debug.Log(somaker.description);
    }

    public void LevelUp()
    {
        somaker.level++;
        Debug.Log("레벨이 증가했습니다!");
    }
}
