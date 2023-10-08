using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem : IValueStat
{
    private float _maxStamina;
    private float _currentStamina;
    public float MaxStamina => _maxStamina;


    public StaminaSystem(float maxStamina)
    {
        _maxStamina = maxStamina;
        _currentStamina = maxStamina;
    }
    public StaminaSystem(float maxStamina, float startCurrentStamina)
    {
        _maxStamina = maxStamina;
        _currentStamina = startCurrentStamina;
    }


    public void Spend(float spendAmount)
    {
        _currentStamina -= spendAmount;
        _currentStamina = Mathf.Max(0f, _currentStamina);

        OnValueUpdate?.Invoke();
    }

    public void SpendAll()
    {
        _currentStamina = 0f;

        OnValueUpdate?.Invoke();
    }


    public void Restore(float gainAmount)
    {
        _currentStamina += gainAmount;
        _currentStamina = Mathf.Min(MaxStamina, _currentStamina);

        OnValueUpdate?.Invoke();
    }

    public void RestoreAll()
    {
        _currentStamina = MaxStamina;

        OnValueUpdate?.Invoke();
    }

    public bool HasStaminaLeft()
    {
        return _currentStamina > 0f;
    }
    public bool HasMaxStamina()
    {
        return _currentStamina == _maxStamina;
    }
    public bool HasEnoughStamina(float staminaAmount)
    {
        return _currentStamina >= staminaAmount;
    }


    public override float GetValuePer1Ratio()
    {
        return _currentStamina / _maxStamina;
    }
}
