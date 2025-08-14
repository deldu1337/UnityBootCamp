using UnityEngine;

[CreateAssetMenu(menuName = "Attack Strategy/Melee")]
public class MeleeAttack : ScriptableObject, IAttackStrategy
{
    public void Attack(GameObject target)
    {
        Debug.Log("[Melee Attack]" + target.name);
    }

    public void CheckHit(GameObject target)
    {
        target.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
