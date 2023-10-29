using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleStunned : MonoBehaviour
{
    private bool _stunned = true;
    private Quaternion _targetRotation;
    private Quaternion _originalRotation;
    [SerializeField] private float smoothing = 0.1f;
    [SerializeField] private float timeStunned= 1f;
    private float _stunnedTimer= 0f;
    
    public bool Stunned => _stunned;

    private void Awake()
    {
        _targetRotation = Quaternion.identity;
    }

    public void StunnedAnim()
    {
        _stunned = true;
        _originalRotation = transform.rotation;
        _targetRotation = _originalRotation * Quaternion.Euler(0, 0, 180);
        _stunnedTimer = 0;

    }

    private void Update()
    {
        if (_stunned)
        {
            if (_targetRotation != transform.rotation)
            {
                if (_stunnedTimer < timeStunned)
                {
                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * smoothing);
                    
                }

                else if (_stunnedTimer >= timeStunned)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, _originalRotation, Time.deltaTime * smoothing);
                }

                if (_stunnedTimer >= timeStunned * 2)
                {
                    _stunned = false;
                }
            }
            _stunnedTimer += Time.deltaTime;
        }
        
    }
}
