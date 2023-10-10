using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class TimerPlayerPressurePlate : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private MeshRenderer _buttonMesh;
    private Material _timerMaterial;
    [SerializeField] private Transform _buttonTransform;

    [Header("TIMER")]
    [SerializeField, Range(0.0f, 30.0f)] private float _pressedDuration = 3.0f;
    [SerializeField] private bool _triggerAllOnce = false;
    private bool _triggeredAllOnceAlready;
    private bool _isPressed;


    [Header("WORLD INTERACTORS")]
    [SerializeField] private AWorldInteractor[] _worldInteractors;

    private int _triggeredCount;

    CancellationTokenSource _pressedCancellationSource;


    private void OnEnable()
    {
        if (_triggerAllOnce)
        {
            foreach (AWorldInteractor worldInteractor in _worldInteractors)
            {
                worldInteractor.OnEnterActivated += CancelTimerAndButton;
            }
        }        
    }
    
    private void OnDisable()
    {
        if (_triggerAllOnce)
        {
            foreach (AWorldInteractor worldInteractor in _worldInteractors)
            {
                worldInteractor.OnEnterActivated -= CancelTimerAndButton;
            }
        }        
    }


    private void Awake()
    {
        _triggeredCount = 0;
        _isPressed = false;

        _timerMaterial = _buttonMesh.material;
        _timerMaterial.SetFloat("_StartTime", -_pressedDuration);
        _timerMaterial.SetFloat("_FillDuration", _pressedDuration);
        _timerMaterial.SetFloat("_CanUnfill", 1.0f);

        _triggeredAllOnceAlready = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_triggeredAllOnceAlready) return;

        if (other.gameObject.CompareTag(TagUtilities.PLAYER_TAG) || other.gameObject.CompareTag(TagUtilities.ANCHOR_TAG))
        {
            if (_triggeredCount++ > 0) return;

            if (_isPressed)
            {
                DeactivateWorldInteractors();
                _pressedCancellationSource.Cancel();
                _isPressed = false;
            }

            SetTriggeredState();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggeredAllOnceAlready) return;

        if (other.gameObject.CompareTag(TagUtilities.PLAYER_TAG) || other.gameObject.CompareTag(TagUtilities.ANCHOR_TAG))
        {
            if (--_triggeredCount > 0) return;
        }
    }



    private void SetTriggeredState()
    {
        PlayTriggerAnimation();
        ActivateWorldInteractors();

        _pressedCancellationSource = new CancellationTokenSource();
        StartPressedTimer().Forget();
    }


    protected void PlayTriggerAnimation()
    {
        _timerMaterial.SetFloat("_StartTime", Time.time);        
        _buttonTransform.DOLocalMove(Vector3.down * 0.05f, 0.2f);        
    }

    protected void PlayUntriggerAnimation()
    {
        _buttonTransform.DOLocalMove(Vector3.up * 0.05f, 0.2f);
    }


    private async UniTaskVoid StartPressedTimer()
    {
        _isPressed = true;
        await UniTask.Delay(MathUtilities.SecondsToMilliseconds(_pressedDuration), cancellationToken: _pressedCancellationSource.Token);

        if (_isPressed && !_triggeredAllOnceAlready)
        {
            _isPressed = false;
            DeactivateWorldInteractors();
            PlayUntriggerAnimation();
        }
    }


    private void DeactivateWorldInteractors()
    {
        foreach (AWorldInteractor worldInteractor in _worldInteractors)
        {
            worldInteractor.AddDeactivationInput();
        }
    }

    private void ActivateWorldInteractors()
    {
        foreach (AWorldInteractor worldInteractor in _worldInteractors)
        {
            worldInteractor.AddActivationInput();
        }
    }


    private void CancelTimerAndButton()
    {
        _triggeredAllOnceAlready = true;
        if (_pressedCancellationSource != null)
        {
            _pressedCancellationSource.Cancel();
        }

        _timerMaterial.SetFloat("_CanUnfill", 0.0f);
    }
}
