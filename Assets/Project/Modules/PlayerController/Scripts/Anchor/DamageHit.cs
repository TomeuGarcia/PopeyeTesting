using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHit 
{
    private float _damage;
    public float Damage {
        get { return _damage; }
        set { _damage = value; }
    }


    Vector3 _position;
    public Vector3 Position {
        get { return _position; }
        set { _position = value; }
    }


    float _knockbackForce;
    public float KnockbackForce
    {
        get { return _knockbackForce; }
        set { _knockbackForce = value; }
    }
    
    
    float _stunDuration;
    public float StunDuration
    {
        get { return _stunDuration; }
        set { _stunDuration = value; }
    }

    public DamageHit(float damage, Vector3 position, float knockbackForce, float stunDuration)
    {
        _damage = damage;
        _position = position;
        _knockbackForce = knockbackForce;
        _stunDuration = stunDuration;
    }

}
