using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack Strategy Sample/CastedSample")]
public class CastedAttackSample : ScriptableObject, IAttackStrategySample
{

    public bool casting = true;
    public int count;
    public int caston;

    public void Attack(GameObject attacker, GameObject target)
    {
        Debug.Log("[Casted Attack]");
        while (count < caston)
        {
            count += 1;
            Debug.Log(count);
        }
        target.GetComponent<DamagedObject>().OnDamaged();
    }
}