using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour, IHealthTarget, IDamageHitTarget
{
    [Header("ANCHOR")]
    [SerializeField] private Anchor _anchor;
    [SerializeField] private AnchorHealthDrainer _anchorHealthDrainer;

    [Header("COMPONENTS")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _targetForEnemies;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private Animator _animator;
    private Material _meshMaterial;
    private Color _defaultMeshColor;

    [Header("HEALTH")]
    [SerializeField, Range(0.0f, 100.0f)] private float _maxHealth = 50.0f;
    [SerializeField] private ValueStatBar _healthBar;
    private HealthSystem _healthSystem;


    public Vector3 Position => transform.position;
    public PlayerController PlayerController => _playerController;

    [HideInInspector] public Vector3 _respawnPosition;


    private ActionMovesetInputHandler _movesetInputHandler;


    public void AwakeInit(ActionMovesetInputHandler movesetInputHandler)
    {
        _healthSystem = new HealthSystem(_maxHealth);
        _healthBar.Init(_healthSystem);

        _meshMaterial = _mesh.material;
        _defaultMeshColor = _meshMaterial.GetColor("_Color");

        _anchorHealthDrainer.Init(this);

        _respawnPosition = Position;

        _movesetInputHandler = movesetInputHandler;
    }

    private void Update()
    {
        if (_movesetInputHandler.IsElectricChainAbility_Pressed())
        {
            _anchor.AnchorElectricChain.ToggleElectricMode();
        }
    }



    public DamageHitTargetType GetDamageHitTargetType()
    {
        return DamageHitTargetType.Player;
    }

    public DamageHitResult TakeHitDamage(DamageHit damageHit)
    {
        Vector3 pushDirection = Position - damageHit.Position;
        pushDirection.y = 0;
        pushDirection = pushDirection.normalized;
        Vector3 pushForce = pushDirection * damageHit.KnockbackMagnitude;
        _playerController.GetPushed(pushForce);

        float receivedDamage = _healthSystem.TakeDamage(damageHit.Damage);
        if (_healthSystem.IsDead())
        {
            Die();
        }

        SetInvulnerableForDuration(1.5f);
        TakeDamageAnimation();

        return new DamageHitResult(receivedDamage);
    }

    public bool CanBeDamaged(DamageHit damageHit)
    {
        return !_healthSystem.IsDead() && !_healthSystem.IsInvulnerable;
    }

    public bool IsDead()
    {
        return _healthSystem.IsDead();
    }

    
    public bool CanBeHealed()
    {
        return !IsDead();
    }


    private async void TakeDamageAnimation()
    {
        _meshMaterial.SetColor("_Color", Color.red);
        _healthSystem.IsInvulnerable = true;

        await Task.Delay(300);

        if (!_healthSystem.IsDead())
        {
            _meshMaterial.SetColor("_Color", _defaultMeshColor);
            _healthSystem.IsInvulnerable = false;
        }        
    }


    private async void Die()
    {
        float duration = 1.0f;

        CameraShake.Instance.PlayShake(0.5f, duration);

        await Task.Delay((int)(duration * 1000));


        await Task.Delay(3000);

        _healthSystem.HealToMax();
        _healthSystem.IsInvulnerable = false;
        _meshMaterial.SetColor("_Color", _defaultMeshColor);
    }

    public void SetInvulnerableForDuration(float duration)
    {
        _healthSystem.SetInvulnerableForDuration(duration);
    }

    public async void DropTargetForEnemiesForDuration(float duration)
    {
        _targetForEnemies.SetParent(null);

        await Task.Delay((int)(duration * 1000));

        _targetForEnemies.SetParent(transform);
        _targetForEnemies.localPosition = Vector3.zero;
    }

    public void Heal(float healAmount)
    {
        _healthSystem.Heal(healAmount);
    }

    public void HealToMax()
    {
        _healthSystem.HealToMax();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public async Task MeleeAttack()
    {
        //play anim
        _animator.SetTrigger("Attack");
        await Task.Delay((int)((0.5f)*1000));
    }


    public void Respawn()
    {
        _playerController.ResetRigidbody();

        transform.DOKill();
        transform.position = _respawnPosition;
    }


}
