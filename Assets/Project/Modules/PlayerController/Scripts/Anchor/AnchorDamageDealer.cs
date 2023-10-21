using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class AnchorDamageDealer : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Anchor _anchor;
    [SerializeField] private AnchorHealthDrainer _anchorHealthDrainer;

    [Header("THROW HIT")]
    [SerializeField, Range(0.0f, 20.0f)] private float _throwHitDamage = 5.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _throwHitKnockbackForce = 80.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _throwHitStunDuration = 0.2f;

    [Header("GROUND HIT")]
    [SerializeField] private TriggerNotifier _groundHitNotifier;
    [SerializeField, Range(0.0f, 30.0f)] private float _groundHitDamage = 10.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _groundHitKnockbackForce = 120.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _groundHitStunDuration = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] private float _groundHitRadius = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _groundHitDuration = 0.4f;
    private Vector3 _groundHitScale;
    [SerializeField] private AnimationCurve _groundHitSizeCurve;

    [Header("MELEE HIT")]
    [SerializeField, Range(0.0f, 30.0f)] private float _meleeHitDamage = 5.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _meleeHitKnockbackForce = 80.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _meleeHitStunDuration = 0.4f;
    [SerializeField] private TriggerNotifier _meleeHitBoxNotifier;

    [Header("PULL BACK HIT")]
    [SerializeField, Range(0.0f, 30.0f)] private float _pullBackHitDamage = 5.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _pullBackHitKnockbackForce = 80.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _pullBackHitStunDuration = 0.4f;
    
    [Header("EXPLOSION HIT")]
    [SerializeField, Range(0.0f, 30.0f)] private float _explosionHitDamage = 20.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _explosionHitKnockbackForce = 80.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _explosionHitStunDuration = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] private float _explosionHitSize = 2.0f;

    [Header("PREFABS")]
    [SerializeField] private DamageHitEffect _hitEffectPrefab;
    [SerializeField] private DamageHitEffect _explosionEffectPrefab;
    [SerializeField] private AnchorHealthDrainEffect _healthDrainEffectPrefab;


    private DamageHit _throwHit;
    private DamageHit _groundHit;
    private DamageHit _meleeHit;
    private DamageHit _pullBackHit;
    private DamageHit _explosionHit;

    private void Awake()
    {
        _meleeHitBoxNotifier.DisableCollider();
        _groundHitNotifier.DisableCollider();
    }

    private void Start()
    {
        _throwHit = new DamageHit(CombatManager.Instance.DamageEnemiesAndDestructiblesPreset, 
            _throwHitDamage, _throwHitKnockbackForce, _throwHitStunDuration);

        _groundHit = new DamageHit(CombatManager.Instance.DamageEnemiesDestructiblesAndInteractablesPreset, 
            _groundHitDamage, _groundHitKnockbackForce, _groundHitStunDuration);

        _meleeHit = new DamageHit(CombatManager.Instance.DamageOnlyEnemiesPreset, 
            _meleeHitDamage, _meleeHitKnockbackForce, _meleeHitStunDuration);

        _pullBackHit = new DamageHit(CombatManager.Instance.DamageOnlyEnemiesPreset, 
            _pullBackHitDamage, _pullBackHitKnockbackForce, _pullBackHitStunDuration);

        _explosionHit = new DamageHit(CombatManager.Instance.DamageEnemiesAndDestructiblesPreset, 
            _explosionHitDamage, _explosionHitKnockbackForce, _explosionHitStunDuration);

        OnValidate();
    }

    private void OnValidate()
    {
        if (_throwHit != null)
        {
            _throwHit.Damage = _throwHitDamage;
            _throwHit.KnockbackMagnitude = _throwHitKnockbackForce;
            _throwHit.StunDuration = _throwHitStunDuration;
        }
        
        if (_groundHit != null)
        {
            _groundHit.Damage = _groundHitDamage;
            _groundHit.KnockbackMagnitude = _groundHitKnockbackForce;
            _groundHit.StunDuration = _groundHitStunDuration;
        }
        
        if (_meleeHit != null)
        {
            _meleeHit.Damage = _meleeHitDamage;
            _meleeHit.KnockbackMagnitude = _meleeHitKnockbackForce;
            _meleeHit.StunDuration = _meleeHitStunDuration;
        }
        

        _groundHitScale = Vector3.one * (_groundHitRadius * 2.0f);
    }


    public void DealThrowHitDamage(GameObject hitObject, Vector3 dealerPosition)
    {
        _throwHit.Position = dealerPosition;
        _throwHit.KnockbackDirection = PositioningHelper.Instance.GetDirectionAlignedWithFloor(dealerPosition, hitObject.transform.position);

        if (TryDealDamage(hitObject, _throwHit, true))
        {
            SpawnHitEffect(hitObject);
        }        
    }

    public void DealPullBackDamage(GameObject hitObject, Vector3 dealerPosition)
    {
        _pullBackHit.Position = dealerPosition;
        _pullBackHit.KnockbackDirection = -PositioningHelper.Instance.GetDirectionAlignedWithFloor(dealerPosition, hitObject.transform.position);

        if (TryDealDamage(hitObject, _pullBackHit, true))
        {
            SpawnHitEffect(hitObject);
        }
    }

    public async void DealGroundHitDamage(Vector3 dealerPosition, float sizeMultiplier)
    {
        _groundHit.Position = dealerPosition;


        sizeMultiplier = _groundHitSizeCurve.Evaluate(sizeMultiplier);
        _groundHitNotifier.transform.position = dealerPosition;
        _groundHitNotifier.transform.localScale = Vector3.one * (_groundHitRadius * sizeMultiplier);
        
        PlayGroundHitAreaAnimation(_groundHitScale * sizeMultiplier, _groundHitDuration, 0.3f);

        _groundHitNotifier.OnEnter += TryDealGroundDamage;
        _groundHitNotifier.EnableCollider();

        await Task.Delay((int)(_groundHitDuration / 2 * 1000));

        _groundHitNotifier.OnEnter -= TryDealGroundDamage;
        _groundHitNotifier.DisableCollider();
    }

    private void TryDealGroundDamage(Collider collider)
    {
        _groundHit.KnockbackDirection = PositioningHelper.Instance.GetDirectionAlignedWithFloor(_anchor.Position, collider.transform.position);
        TryDealDamage(collider.gameObject, _groundHit, true);
    }

    public async void DealExplosionDamage(Vector3 dealerPosition)
    {
        _explosionHit.Position = dealerPosition;

        Vector3 scale = Vector3.one * _explosionHitSize;
        float duration = 0.5f;

        _groundHitNotifier.transform.position = dealerPosition;
        _groundHitNotifier.transform.localScale = scale / 2;

        PlayExplosionHitAreaAnimation(scale, duration, 0.5f);

        _groundHitNotifier.OnEnter += TryDealExplosionDamage;
        _groundHitNotifier.EnableCollider();

        await Task.Delay(MathUtilities.SecondsToMilliseconds(duration));

        _groundHitNotifier.OnEnter -= TryDealExplosionDamage;
        _groundHitNotifier.DisableCollider();
    }

    private void TryDealExplosionDamage(Collider collider)
    {
        _explosionHit.KnockbackDirection = PositioningHelper.Instance.GetDirectionAlignedWithFloor(_anchor.Position, collider.transform.position);
        TryDealDamage(collider.gameObject, _explosionHit, false);
    }


    public async void StartMeleeHitDamage(Vector3 dealerPosition, float delay, float duration)
    {
        await Task.Delay((int)(delay * 1000));

        _meleeHit.Position = dealerPosition;

        _meleeHitBoxNotifier.OnEnter += TryDealMeleeDamage;
        _meleeHitBoxNotifier.EnableCollider();

        await Task.Delay((int)(duration * 1000));

        _meleeHitBoxNotifier.OnEnter -= TryDealMeleeDamage;
        _meleeHitBoxNotifier.DisableCollider();
    }

    private void TryDealMeleeDamage(Collider collider)
    {
        _meleeHit.KnockbackDirection = PositioningHelper.Instance.GetDirectionAlignedWithFloor(_anchor.OwnerPosition, collider.transform.position);
        if (TryDealDamage(collider.gameObject, _meleeHit, true))
        {
            SpawnHitEffect(collider.gameObject);
        }
    }


    private bool TryDealDamage(GameObject hitObject, DamageHit damageHit, bool restoresStamina)
    {
        if (!CombatManager.Instance.TryDealDamage(hitObject, damageHit, out DamageHitResult damageHitResult))
        {
            return false;
        }
        
        if (damageHitResult.ReceivedDamage > 0 && restoresStamina)
        {
            SpawnDrainHealthEffect(hitObject, damageHitResult.ReceivedDamage);
            _anchorHealthDrainer.IncrementDrainedHealth(damageHitResult.ReceivedDamage);
        }

        return true;
    }


    private void PlayGroundHitAreaAnimation(Vector3 scale, float duration, float shakeDuration)
    {
        DamageHitEffect hitEffect = Instantiate(_hitEffectPrefab, transform.position, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        hitEffect.LocalScale = scale;
        hitEffect.Duration = duration;

        CameraShake.Instance.PlayShake(0.05f, shakeDuration);
    }
    
    private void PlayExplosionHitAreaAnimation(Vector3 scale, float duration, float shakeDuration)
    {
        DamageHitEffect hitEffect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        hitEffect.LocalScale = scale;
        hitEffect.Duration = duration;

        CameraShake.Instance.PlayShake(0.05f, shakeDuration);
    }

    private void SpawnHitEffect(GameObject hitTarget)
    {
        if (Physics.Raycast(transform.position, (hitTarget.transform.position - transform.position).normalized, out RaycastHit hit, 5.0f))
        {
            DamageHitEffect damageHitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
            damageHitEffect.transform.SetParent(hitTarget.transform);
        }                  
    }

    private void SpawnDrainHealthEffect(GameObject hitTarget, float damageDealt)
    {
        int emissionCount = (int)(damageDealt * 0.5f);
        AnchorHealthDrainEffect healthDrainEffect = Instantiate(_healthDrainEffectPrefab);
        healthDrainEffect.Init(transform, hitTarget.transform.position, emissionCount);
    }


}
