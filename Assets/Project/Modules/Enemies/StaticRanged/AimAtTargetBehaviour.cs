using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtTargetBehaviour : MonoBehaviour
{
    [SerializeField] private float smoothing = 0.1f;
    private bool _resetting = true;

    private Transform _currentTarget = null;

    private Quaternion _initialRotation = new Quaternion();
    private Quaternion _targetRotation = new Quaternion();

    private void Start() => _initialRotation = transform.rotation;

    private void LateUpdate() => Aim();

    public void SetTarget(Transform target)
    {
        _resetting = false;
        _currentTarget = target;
    }

    public void ClearTarget()
    {
        _currentTarget = null;
        _resetting = true;
    }

    public void SetTarget(TargetableBehaviour target) => SetTarget(target.targetPoint);

    private void Aim()
    {
        if (_currentTarget == null)
        {
            if (!_resetting)
            {
                return;
            }

            _targetRotation = _initialRotation;
        }
        else
        {
            _targetRotation = _resetting ? _initialRotation : Quaternion.LookRotation(_currentTarget.position-transform.position);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * smoothing);
    }
}
