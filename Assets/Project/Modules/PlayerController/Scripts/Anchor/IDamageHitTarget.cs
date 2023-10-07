using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageHitTarget
{
    public void TakeHit(DamageHit anchorHit);
    public bool CanBeDamaged(DamageHit damageHit);
    public bool IsDead();
}
