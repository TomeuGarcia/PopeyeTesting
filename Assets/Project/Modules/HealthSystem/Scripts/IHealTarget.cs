using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthTarget
{
    public void Heal(float healAmount);
    public void HealToMax();
}
