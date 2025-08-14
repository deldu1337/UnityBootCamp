using UnityEngine;
[CreateAssetMenu(menuName = "Attack Strategy Sample/RangedSample")]
public class RangedAttackSample : ScriptableObject, IAttackStrategySample
{
    //원거리 공격 형태의 공격 범위를 체크한다.
    [SerializeField] private float attackRange;

    public void Attack(GameObject attacker, GameObject target)
    {
        float distance = Vector2.Distance(attacker.transform.position, target.transform.position);

        if (distance <= attackRange)
        {
            Debug.Log("[Ranged Attack]" + target.name);
            target.GetComponent<DamagedObject>().OnDamaged();
        }
        else
        {
            Debug.Log("Attack Failed...");
        }
    }
}