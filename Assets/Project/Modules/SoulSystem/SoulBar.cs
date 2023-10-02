using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulBar : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Image _fillImage;

    private float _maxValue;
    private float _value;

    public void Init(float maxValue, float value)
    {
        _maxValue = maxValue;
        ChangeValue(value);
    }

    public void ChangeValue(float value)
    {
        _value = value;
        _fillImage.fillAmount = _value / _maxValue;
    }
}
