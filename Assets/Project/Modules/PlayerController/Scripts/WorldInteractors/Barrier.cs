using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : AWorldInteractor
{
    [Header("ACTIVATED")]
    [SerializeField] private Transform _activatedStateSpot;
    [SerializeField, Range(0.0f, 10.0f)] private float _activateDuration = 0.5f;

    [Header("DEACTIVATED")]
    [SerializeField] private Transform _deactivatedStateSpot;
    [SerializeField, Range(0.0f, 10.0f)] private float _deactivateDuration = 0.5f;

    [Header("REFERNCES")]
    [SerializeField] private Transform _barrierTransform;
    [SerializeField] private Collider _collider;
    [SerializeField] private bool _startActivated = false;

    private bool _isActivated = false;

    protected override void AwakeInit()
    {
        _isActivated = _startActivated;
        if (_startActivated)
        {
            SetStateInstantly(_activatedStateSpot);
        }
        else
        {
            SetStateInstantly(_deactivatedStateSpot);
        }

        SetCollisionEnabled(!_startActivated);
    }

    protected override void EnterActivatedState()
    {
        SetState(_activatedStateSpot, _activateDuration);
        SetCollisionEnabled(true);
        _isActivated = true;
    }

    protected override void EnterDeactivatedState()
    {
        SetState(_deactivatedStateSpot, _deactivateDuration);
        SetCollisionEnabledDelayed(false, _deactivateDuration).Forget();
        _isActivated = false;
    }


    private void SetStateInstantly(Transform goalStateSpot)
    {
        _barrierTransform.position = goalStateSpot.position;
        _barrierTransform.rotation = goalStateSpot.rotation;
    }
    
    private void SetState(Transform goalStateSpot, float duration)
    {
        _barrierTransform.DOMove(goalStateSpot.position, duration);
        _barrierTransform.DORotateQuaternion(goalStateSpot.rotation, duration);
    }


    private void SetCollisionEnabled(bool isEnabled)
    {
        if (_collider != null)
        {
            _collider.enabled = isEnabled;
        }
    }
    private async UniTaskVoid SetCollisionEnabledDelayed(bool isEnabled, float delay)
    {
        await UniTask.Delay((int)(delay * 1000));
        if (_collider != null)
        {
            _collider.enabled = isEnabled;
        }
    }

}
