using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageHitTarget
{
    public DamageHitTargetType GetDamageHitTargetType();
    public DamageHitResult TakeHitDamage(DamageHit damageHit);
    public bool CanBeDamaged(DamageHit damageHit);
    public bool IsDead();
}
