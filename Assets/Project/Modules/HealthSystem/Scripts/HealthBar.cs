using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _lazyBarFillImage;
    [SerializeField, Range(0.0f, 10.0f)] private float _fullFillDuration = 1.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _lazyFullFillDuration = 2.0f;

    [SerializeField] private Color _healedColor = Color.green;
    [SerializeField] private Color _damagedColor = Color.red;
    private Color _originalColor;

    private HealthSystem _healthSystem;
    private bool _isSubscribed;



    private void OnEnable()
    {
        if (_healthSystem != null)
        {
            SubscribeToEvents();
        }
    }

    private void OnDisable()
    {
        if (_healthSystem != null)
        {
            UnsubscribeToEvents();
        }
    }


    public void Init(HealthSystem healthSystem)
    {
        _healthSystem = healthSystem;
        _isSubscribed = false;

        _originalColor = _fillImage.color;

        SubscribeToEvents();
        InstantUpdateFillImage();
    }


    private void SubscribeToEvents()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        _healthSystem.OnHealthUpdate += UpdateFillImage;
    }
    
    private void UnsubscribeToEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;

        _healthSystem.OnHealthUpdate -= UpdateFillImage;
    }


    private void InstantUpdateFillImage()
    {
        _fillImage.fillAmount = _lazyBarFillImage.fillAmount = _healthSystem.CurrentHealthRatio;
    }

    private void UpdateFillImage()
    {
        float newFillValue = _healthSystem.CurrentHealthRatio;        
        float changeAmount = newFillValue - _fillImage.fillAmount;
        bool wasHealed = changeAmount > 0;
        changeAmount = Mathf.Abs(changeAmount);

        _fillImage.DOComplete();
        _fillImage.DOFillAmount(newFillValue, changeAmount * _fullFillDuration)
            .SetEase(Ease.InOutQuad);

        _lazyBarFillImage.DOComplete();
        _lazyBarFillImage.DOFillAmount(newFillValue, changeAmount * _lazyFullFillDuration)
            .SetEase(Ease.InOutQuad);

        PunchFillImageColor(wasHealed ? _healedColor : _damagedColor, _fullFillDuration);
    }

    
    private void PunchFillImageColor(Color punchColor, float duration)
    {
        duration /= 2;
        _fillImage.DOColor(punchColor, duration)
            .OnComplete(() =>
            {
                _fillImage.DOColor(_originalColor, duration);
            });
    }

}
