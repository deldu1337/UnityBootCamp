using UnityEngine;
[CreateAssetMenu(menuName = "Attack Strategy Sample/RangedSample")]
public class RangedAttackSample : ScriptableObject, IAttackStrategySample
{
    //���Ÿ� ���� ������ ���� ������ üũ�Ѵ�.
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