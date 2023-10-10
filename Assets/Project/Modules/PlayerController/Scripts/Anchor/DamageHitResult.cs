using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitResult
{

    private float _receivedDamage;
    public float ReceivedDamage
    {
        get { return _receivedDamage; }
        set { _receivedDamage = value; }
    }

    public DamageHitResult(float receivedDamage)
    {
        _receivedDamage = receivedDamage;
    }
}
