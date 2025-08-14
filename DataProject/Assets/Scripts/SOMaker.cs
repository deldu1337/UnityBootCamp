using UnityEngine;

// 에디터에서 해당 오브젝트 생성 가능
[CreateAssetMenu(fileName = "아이템", menuName = "Item/아이템")]
public class SOMaker : ScriptableObject
{
    public enum ItemType { 장비, 소비, 기타 }

    public string item_name;
    public ItemType type;
    public string description;
    public int level;
}
