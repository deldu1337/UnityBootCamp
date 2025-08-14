using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
    void Attack(GameObject target);
    void CheckHit(GameObject target);
}

public interface IDamageable
{
    void TakeDamage(int damage);
}
