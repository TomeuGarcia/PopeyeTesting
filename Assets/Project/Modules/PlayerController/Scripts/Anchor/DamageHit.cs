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


    private Vector3 _position;
    public Vector3 Position {
        get { return _position; }
        set { _position = value; }
    }



    private float _knockbackMagnitude;
    public float KnockbackMagnitude
    {
        get { return _knockbackMagnitude; }
        set { 
            _knockbackMagnitude = value;
            KnockbackForce = _knockbackMagnitude * _knockbackDirection;
        }
    }
    
    private Vector3 _knockbackDirection;
    public Vector3 KnockbackDirection
    {
        get { return _knockbackDirection; }
        set { 
            _knockbackDirection = value;
            KnockbackForce = _knockbackMagnitude * _knockbackDirection;
        }
    }

    public Vector3 KnockbackForce { get; private set; }



    private float _stunDuration;
    public float StunDuration
    {
        get { return _stunDuration; }
        set { _stunDuration = value; }
    }


    private DamageHitTargetType _damageHitTargetTypeMask;
    public DamageHitTargetType DamageHitTargetTypeMask
    {
        get { return _damageHitTargetTypeMask; }
    }


    public DamageHit(DamageHitTargetType damageHitTargetTypeMask, float damage, float knockbackMagnitude, float stunDuration)
    {
        _damageHitTargetTypeMask = damageHitTargetTypeMask;
        _damage = damage;
        _position = Vector3.zero;
        _stunDuration = stunDuration;

        KnockbackMagnitude = knockbackMagnitude;
        KnockbackDirection = Vector3.zero;
    }
    
    public DamageHit(DamageHitTargetType damageHitTargetTypeMask, float damage, float knockbackMagnitude, float stunDuration, Vector3 position)
    {
        _damageHitTargetTypeMask = damageHitTargetTypeMask;
        _damage = damage;
        _position = position;
        _stunDuration = stunDuration;

        KnockbackMagnitude = knockbackMagnitude;
        KnockbackDirection = Vector3.zero;
    }


    
}
