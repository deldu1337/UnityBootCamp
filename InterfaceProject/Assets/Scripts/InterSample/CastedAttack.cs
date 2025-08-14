using UnityEngine;

[CreateAssetMenu(menuName = "Attack Strategy/Casted")]
public class CastedAttack : ScriptableObject, IAttackStrategy
{
    public void Attack(GameObject target)
    {
        Debug.Log("[Casted Attack]" + target.name);
    }

    public void CheckHit(GameObject target)
    {
        target.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
