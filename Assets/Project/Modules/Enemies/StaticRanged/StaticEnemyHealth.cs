using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemyHealth : MonoBehaviour, IDamageHitTarget
{
    private HealthSystem _healthSystem;
    [SerializeField, Range(0.0f, 100.0f)] private float _maxHealth = 50.0f;

    void Awake()
    {
        _healthSystem = new HealthSystem(_maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public bool CanBeDamaged(DamageHit damageHit)
    {
        return !_healthSystem.IsDead() && !_healthSystem.IsInvulnerable;
    }

    public bool IsDead()
    {
        return _healthSystem.IsDead();
    }

    public DamageHitTargetType GetDamageHitTargetType()
    {
        return DamageHitTargetType.Enemy;
    }

    public DamageHitResult TakeHitDamage(DamageHit damageHit)
    {
        float receivedDamage = _healthSystem.TakeDamage(damageHit.Damage);
        if (_healthSystem.IsDead())
        {
            //TODO: this destroy is provisional
            //TOASK Tomeu: why it hits more than one time with just one shot
            Destroy(transform.parent.gameObject);
        }

        return new DamageHitResult(receivedDamage);
    }
}
