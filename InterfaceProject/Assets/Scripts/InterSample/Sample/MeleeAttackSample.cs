using UnityEngine;

[CreateAssetMenu(menuName = "Attack Strategy Sample/MeleeSample")]
public class MeleeAttackSample : ScriptableObject, IAttackStrategySample
{

    public void Attack(GameObject attacker, GameObject target)
    {
        float distance = Vector2.Distance(attacker.transform.position, target.transform.position);

        if (distance <= 1 && distance >= -1)
        {
            Debug.Log("[Melee Attack]" + target.name);
            target.GetComponent<DamagedObject>().OnDamaged();
        }
        else
        {
            Debug.Log("Attack Failed...");
        }

    }
}