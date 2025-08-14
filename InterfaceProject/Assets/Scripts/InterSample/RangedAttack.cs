using UnityEngine;

[CreateAssetMenu(menuName = "Attack Strategy/Ranged")]
public class RangedAttack : ScriptableObject, IAttackStrategy
{
    public void Attack(GameObject target)
    {
        Debug.Log("[Ranged Attack]" + target.name);
    }

    public void CheckHit(GameObject target)
    {
        target.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
