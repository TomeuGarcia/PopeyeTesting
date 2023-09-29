using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class AnchorDamageDealer : MonoBehaviour
{
    [Header("THROW HIT")]
    [SerializeField, Range(0.0f, 20.0f)] private float _throwHitDamage = 5.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _throwHitKnockbackForce = 80.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _throwHitStunDuration = 0.2f;

    [Header("GROUND HIT")]
    [SerializeField] private TriggerNotifier _groundHitNotifier;
    [SerializeField, Range(0.0f, 20.0f)] private float _groundHitDamage = 10.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _groundHitKnockbackForce = 120.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _groundHitStunDuration = 0.4f;
    [SerializeField, Range(0.0f, 10.0f)] private float _groundHitRadius = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _groundHitDuration = 0.4f;
    private Vector3 _groundHitScale;
    [SerializeField] private AnimationCurve _groundHitSizeCurve;

    [Header("MELEE HIT")]
    [SerializeField, Range(0.0f, 20.0f)] private float _meleeHitDamage = 5.0f;
    [SerializeField, Range(0.0f, 500.0f)] private float _meleeHitKnockbackForce = 80.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float _meleeHitStunDuration = 0.4f;
    [SerializeField] private TriggerNotifier _meleeHitBoxNotifier;

    [Header("PREFABS")]
    [SerializeField] private DamageHitEffect _hitEffectPrefab;
    [SerializeField] private AnchorHealthDrainEffect _healthDrainEffectPrefab;


    private DamageHit _throwHit;
    private DamageHit _groundHit;
    private DamageHit _meleeHit;


    public delegate void AnchorDamageDealerEvent(DamageHit damageHit);
    public AnchorDamageDealerEvent OnDamageDealtEvent;


    private void Awake()
    {
        _meleeHitBoxNotifier.DisableCollider();
        _groundHitNotifier.DisableCollider();

        _throwHit = new DamageHit(_throwHitDamage, Vector3.zero, _throwHitKnockbackForce, _throwHitStunDuration);
        _groundHit = new DamageHit(_groundHitDamage, Vector3.zero, _groundHitKnockbackForce, _groundHitStunDuration);
        _meleeHit = new DamageHit(_meleeHitDamage, Vector3.zero, _meleeHitKnockbackForce, _meleeHitStunDuration);

        OnValidate();
    }

    private void OnValidate()
    {
        if (_throwHit != null)
        {
            _throwHit.Damage = _throwHitDamage;
            _throwHit.KnockbackForce = _throwHitKnockbackForce;
            _throwHit.StunDuration = _throwHitStunDuration;
        }
        
        if (_groundHit != null)
        {
            _groundHit.Damage = _groundHitDamage;
            _groundHit.KnockbackForce = _groundHitKnockbackForce;
            _groundHit.StunDuration = _groundHitStunDuration;
        }
        
        if (_meleeHit != null)
        {
            _meleeHit.Damage = _meleeHitDamage;
            _meleeHit.KnockbackForce = _meleeHitKnockbackForce;
            _meleeHit.StunDuration = _meleeHitStunDuration;
        }
        

        _groundHitScale = Vector3.one * (_groundHitRadius * 2.0f);
    }


    public void DealThrowHitDamage(GameObject hitObject, Vector3 dealerPosition)
    {
        _throwHit.Position = dealerPosition;

        if (TryDealDamage(hitObject, _throwHit))
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
        
        PlayGroundHitAreaAnimation(sizeMultiplier);

        _groundHitNotifier.OnEnter += TryDealGroundDamage;
        _groundHitNotifier.EnableCollider();

        await Task.Delay((int)(_groundHitDuration / 2 * 1000));

        _groundHitNotifier.OnEnter -= TryDealGroundDamage;
        _groundHitNotifier.DisableCollider();
    }

    private void TryDealGroundDamage(Collider collider)
    {
        TryDealDamage(collider.gameObject, _groundHit);
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
        if (TryDealDamage(collider.gameObject, _meleeHit))
        {
            SpawnHitEffect(collider.gameObject);
        }
    }


    private bool TryDealDamage(GameObject hitObject, DamageHit damageHit)
    {
        if (!hitObject.TryGetComponent<IDamageHitTarget>(out IDamageHitTarget hitTarget))
        {
            return false;
        }

        if (!hitTarget.CanBeDamaged())
        {
            return false;
        }

        hitTarget.TakeHit(damageHit);
        OnDamageDealtEvent?.Invoke(damageHit);

        SpawnDrainHealthEffect(hitObject, damageHit);

        return true;
    }


    private void PlayGroundHitAreaAnimation(float sizeMultiplier)
    {
        DamageHitEffect hitEffect = Instantiate(_hitEffectPrefab, transform.position, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        hitEffect.LocalScale = _groundHitScale * sizeMultiplier;
        hitEffect.Duration = _groundHitDuration;

        CameraShake.Instance.PlayShake(0.05f, 0.3f);
    }

    private void SpawnHitEffect(GameObject hitTarget)
    {
        if (Physics.Raycast(transform.position, (hitTarget.transform.position - transform.position).normalized, out RaycastHit hit, 5.0f))
        {
            DamageHitEffect damageHitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
            damageHitEffect.transform.SetParent(hitTarget.transform);
        }                  
    }

    private void SpawnDrainHealthEffect(GameObject hitTarget, DamageHit damageHit)
    {
        int emissionCount = (int)(damageHit.Damage * 0.5f);
        AnchorHealthDrainEffect healthDrainEffect = Instantiate(_healthDrainEffectPrefab);
        healthDrainEffect.Init(transform, hitTarget.transform.position, emissionCount);
    }


}
