using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueStatBar : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private RectTransform _mainTransform;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _lazyBarFillImage;
    [SerializeField, Range(0.0f, 10.0f)] private float _fullFillDuration = 1.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _lazyFullFillDuration = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _colorPunchMinDuration = 0.4f;

    [SerializeField] private Color _originalColor = Color.blue;
    [SerializeField] private Color _lazyColor = Color.black;
    [SerializeField] private Color _incrementColor = Color.green;
    [SerializeField] private Color _decrementColor = Color.red;    

    private IValueStat _valueStat;
    private bool _isSubscribed;



    private void OnValidate()
    {
        if (_fillImage != null)
        {
            _fillImage.color = _originalColor;
        }

        if (_lazyBarFillImage != null)
        {
            _lazyBarFillImage.color = _lazyColor;
        }
    }

    private void OnEnable()
    {
        if (_valueStat != null)
        {
            SubscribeToEvents();
        }
    }

    private void OnDisable()
    {
        if (_valueStat != null)
        {
            UnsubscribeToEvents();
        }
    }

    public void Init(IValueStat valueStat)
    {
        _valueStat = valueStat;
        _isSubscribed = false;

        OnValidate();

        SubscribeToEvents();
        InstantUpdateFillImage();
    }


    private void SubscribeToEvents()
    {
        if (_isSubscribed) return;
        _isSubscribed = true;

        _valueStat.OnValueUpdate += UpdateFillImage;
    }
    
    private void UnsubscribeToEvents()
    {
        if (!_isSubscribed) return;
        _isSubscribed = false;

        _valueStat.OnValueUpdate -= UpdateFillImage;
    }


    private void InstantUpdateFillImage()
    {
        _fillImage.fillAmount = _lazyBarFillImage.fillAmount = _valueStat.GetValuePer1Ratio();
    }

    private void UpdateFillImage()
    {
        float newFillValue = _valueStat.GetValuePer1Ratio();        
        float changeAmount = newFillValue - _fillImage.fillAmount;
        bool wasDamaged = changeAmount < 0;
        changeAmount = Mathf.Abs(changeAmount);
        
        FillBar(_fillImage, newFillValue, changeAmount * _fullFillDuration);
        FillBar(_lazyBarFillImage, newFillValue, changeAmount * _lazyFullFillDuration);

        PunchFillImageColor(wasDamaged ? _decrementColor : _incrementColor, changeAmount * _fullFillDuration);
    }

    private void FillBar(Image fillImage, float newFillValue, float duration)
    {
        fillImage.DOComplete();
        fillImage.DOFillAmount(newFillValue, duration)
            .SetEase(Ease.InOutQuad);
    }
    
    private void PunchFillImageColor(Color punchColor, float duration)
    {
        duration = Mathf.Max(duration, _colorPunchMinDuration);
        duration /= 2;
        _fillImage.DOColor(punchColor, duration)
            .OnComplete(() =>
            {
                _fillImage.DOColor(_originalColor, duration);
            });
    }

    public void PlayErrorAnimation()
    {
        _mainTransform.DOComplete();
        _mainTransform.DOPunchPosition(Vector3.right * 20.0f, 0.5f);
    }

}
