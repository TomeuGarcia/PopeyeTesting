using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPressurePlate : MonoBehaviour, IDamageHitTarget
{
    [Header("REFERENCES")]
    [SerializeField] private Material _triggeredMaterial;
    [SerializeField] private Material _notTriggeredMaterial;
    [SerializeField] private MeshRenderer _buttonMesh;
    [SerializeField] private Transform _buttonTransform;
    [SerializeField] protected BoxCollider _collider;

    [Header("WORLD INTERACTORS")]
    [SerializeField] private AWorldInteractor[] _worldInteractors;

    protected bool _isTriggered;



    private void Awake()
    {
        _buttonMesh.material = _notTriggeredMaterial;
        _isTriggered = false;
    }

    public bool CanBeDamaged(DamageHit damageHit)
    {
        return CanBeTriggered(damageHit);
    }

    public bool IsDead()
    {
        return false;
    }

    public DamageHitTargetType GetDamageHitTargetType()
    {
        return DamageHitTargetType.Interactable;
    }

    public DamageHitResult TakeHitDamage(DamageHit damageHit)
    {
        OnTakeAnchorHit();

        return new DamageHitResult(0);
    }

    protected virtual bool CanBeTriggered(DamageHit damageHit)
    {
        if (!_collider.bounds.Contains(damageHit.Position))
        {
            return false;
        }

        return !_isTriggered && damageHit.Damage > 10;
    }

    protected virtual void OnTakeAnchorHit()
    {
        PlayTriggerAnimation();
        _isTriggered = true;

        ActivateWorldInteractors();
    }


    protected void PlayTriggerAnimation()
    {
        _buttonMesh.material = _triggeredMaterial;
        _buttonTransform.DOLocalMove(Vector3.down * 0.05f, 0.2f);
    }
    protected void PlayUntriggerAnimation()
    {
        _buttonMesh.material = _notTriggeredMaterial;
        _buttonTransform.DOLocalMove(Vector3.zero, 0.2f);
    }


    protected void DeactivateWorldInteractors()
    {
        foreach (AWorldInteractor worldInteractor in _worldInteractors)
        {
            worldInteractor.AddDeactivationInput();
        }
    }

    protected void ActivateWorldInteractors()
    {
        foreach (AWorldInteractor worldInteractor in _worldInteractors)
        {
            worldInteractor.AddActivationInput();
        }
    }
}
