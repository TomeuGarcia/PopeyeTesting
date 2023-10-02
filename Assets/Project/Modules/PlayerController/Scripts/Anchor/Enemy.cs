using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IDamageHitTarget, IMovementInputHandler
{
    [Header("COMPONENTS")]
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private PlayerController _enemyController;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("KNOCKBACK")]
    [SerializeField, Range(0.0f, 1.0f)] private float _knockbackEffectiveness = 1.0f;

    [Header("CHASING")]
    [SerializeField, Range(0.0f, 10.0f)] private float _chaseStartDistance = 8.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _chaseStopDistance = 7.0f;

    [Header("HEALTH")]
    [SerializeField, Range(0.0f, 100.0f)] private float _maxHealth = 50.0f;
    private HealthSystem _healthSystem;


    private Vector3 Position => transform.position;
    private Vector3 TargetPosition => _lookTarget.position;

    private bool _canMove;

    private Vector3 initPos;
    
    private void Awake()
    {
        _enemyController.MovementInputHandler = this;
        _canMove = true;

        _healthSystem = new HealthSystem(_maxHealth);

        initPos = transform.position;
    }

    private void Update()
    {
        float distanceFromTarget = Vector3.Distance(Position, TargetPosition);
        if (_canMove && distanceFromTarget < _chaseStopDistance)
        {
            _canMove = false;
        }
        else if (!_canMove && distanceFromTarget > _chaseStartDistance)
        {
            _canMove = true;
        }

        if (transform.position.y < -1)
        {
            Respawn();
        }
    }


    public void TakeHit(DamageHit damageHit)
    {
        TakeKnockback(damageHit.Position, damageHit.KnockbackForce);

        _healthSystem.TakeDamage(damageHit.Damage);
        if (_healthSystem.IsDead())
        {
            float deathDuration = 1.0f;
            _enemyController.enabled = false;
            transform.DOBlendableRotateBy(Vector3.right * 180f, deathDuration).OnComplete(() =>
            {
                Respawn();
            });

            GetStunned(deathDuration);
        }
        else
        {
            GetStunned(damageHit.StunDuration);
        }
    }

    public bool CanBeDamaged()
    {
        return !_healthSystem.IsDead() && !_healthSystem.IsInvulnerable;
    }

    public bool IsDead()
    {
        return _healthSystem.IsDead();
    }

    private void Respawn()
    {
        _enemyController.enabled = true;
        transform.position = initPos;
        _healthSystem.HealToMax();
        transform.rotation = Quaternion.identity;
    }
    
    private void TakeKnockback(Vector3 originPosition, float knockbackForce)
    {
        Vector3 pushForce = GetHeightIgnoredDirection(originPosition, Position) * knockbackForce * _knockbackEffectiveness;

        _rigidbody.AddForce(pushForce, ForceMode.Impulse);
        
    }

    private async void GetStunned(float duration)
    {
        _enemyController.enabled = false;
        await Task.Delay((int)(duration * 1000));

        if (!_healthSystem.IsDead())
        {
            _enemyController.enabled = true;
        }
    }


    private Vector3 GetHeightIgnoredDirection(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        direction.y = 0;
        return direction.normalized;
    }


    public Vector3 GetMovementInput()
    {
        if (!_canMove)
        {
            return Vector3.zero;
        }

        Vector3 movementInput = GetHeightIgnoredDirection(Position, TargetPosition);

        return movementInput;
    }

    public Vector3 GetLookInput()
    {
        return GetHeightIgnoredDirection(Position, TargetPosition);
    }
}
