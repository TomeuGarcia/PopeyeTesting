using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageHitTarget, IMovementInputHandler
{
    [Header("COMPONENTS")]
    [SerializeField] private EnemyStateMachine _stateMachine;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private PlayerController _enemyController;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("MOVE SPEEDS")]
    [SerializeField, Range(0.0f, 100.0f)] private float _maxMoveSpeed = 16.0f;
    public float MaxMoveSpeed => _maxMoveSpeed;

    [Header("KNOCKBACK")]
    [SerializeField, Range(0.0f, 1.0f)] private float _knockbackEffectiveness = 1.0f;

    [Header("HEALTH")]
    [SerializeField, Range(0.0f, 100.0f)] private float _maxHealth = 50.0f;
    private HealthSystem _healthSystem;


    public Vector3 Position => transform.position;
    public Vector3 LookDirection => _enemyController.LookDirection;
    public Vector3 TargetPosition => _lookTarget.position;

    private bool _canMove;
    private int _disabledMovementCount;


    private void Awake()
    {
        _enemyController.MovementInputHandler = this;
        _enemyController.MaxSpeed = _maxMoveSpeed;
        _canMove = true;
        _disabledMovementCount = 0;

        _healthSystem = new HealthSystem(_maxHealth);

        _stateMachine.AwakeInit(this);
    }

    private void Update()
    {
        if (transform.position.y < -1)
        {
            Respawn();
        }
    }


    public void TakeHit(DamageHit damageHit)
    {
        TakeKnockback(damageHit.Position, damageHit.KnockbackForce);

        _healthSystem.TakeDamage(damageHit.Damage);
        if (!_healthSystem.IsDead())
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

    public void StartDeathAnimation()
    {
        float deathDuration = 1.0f;
        _enemyController.enabled = false;
        transform.DOBlendableRotateBy(Vector3.right * 180f, deathDuration).OnComplete(() =>
        {
            Respawn();
        });

        GetStunned(deathDuration);
    }

    private void Respawn()
    {
        _enemyController.enabled = true;
        transform.position = new Vector3(-10, 10, 10);
        _healthSystem.HealToMax();
        transform.rotation = Quaternion.identity;

        _stateMachine.ResetStateMachine();
    }
    
    private void TakeKnockback(Vector3 originPosition, float knockbackForce)
    {
        transform.DOKill();

        Vector3 direction = PositioningHelper.Instance.GetDirectionAlignedWithFloor(originPosition, Position);
        Vector3 pushForce = direction * knockbackForce * _knockbackEffectiveness;

        _rigidbody.AddForce(pushForce, ForceMode.Impulse);
        
    }

    private async void GetStunned(float duration)
    {
        DisableMovement();
        await Task.Delay((int)(duration * 1000));

        if (!_healthSystem.IsDead())
        {
            EnableMovement();
        }
    }

    public void SetMaxMoveSpeed(float maxMoveSpeed)
    {
        _enemyController.MaxSpeed = maxMoveSpeed;
    }
    public void SetCanRotate(bool canRotate)
    {
        _enemyController.CanRotate = canRotate;
    }

    private void EnableMovement()
    {
        --_disabledMovementCount;

        if (_disabledMovementCount == 0)
        {
            _enemyController.enabled = true;
        }
    }
    private void DisableMovement()
    {
        if (_disabledMovementCount == 0)
        {
            _enemyController.enabled = false;
        }

        ++_disabledMovementCount;
    }

    public bool IsTargetOnReachableHeight()
    {
        return Mathf.Abs(Position.y - TargetPosition.y) < 0.5f;
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
