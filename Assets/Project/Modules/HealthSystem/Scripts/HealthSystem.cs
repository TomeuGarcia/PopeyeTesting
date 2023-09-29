using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HealthSystem
{
    private float _maxHealth;
    private float _currentHealth;
    public float MaxHealth => _maxHealth;
    public float CurrentHealthRatio => _currentHealth / _maxHealth;

    private bool _isInvulnerable;
    public bool IsInvulnerable
    { 
        get { return _isInvulnerable; } 
        set { _isInvulnerable = value;}
    }


    public delegate void HealthSystemEvent();
    public HealthSystemEvent OnHealthUpdate;


    public HealthSystem(float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = MaxHealth;
        _isInvulnerable = false;
    }


    public void TakeDamage(float damageAmount)
    {
        if (IsInvulnerable) { return; }

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Max(0, _currentHealth);

        OnHealthUpdate?.Invoke();
    }
    
    public void Kill()
    {
        _currentHealth = 0f;

        OnHealthUpdate?.Invoke();
    }


    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Min(MaxHealth, _currentHealth);

        OnHealthUpdate?.Invoke();
    }
    
    public void HealToMax()
    {
        _currentHealth = MaxHealth;

        OnHealthUpdate?.Invoke();
    }

    public bool IsDead()
    {
        return _currentHealth == 0f;
    }

    public async void SetInvulnerableForDuration(float duration, bool setVulnerableEvenIfDead = false)
    {
        _isInvulnerable = true;

        await Task.Delay((int)(duration * 1000));

        if (!IsDead() || setVulnerableEvenIfDead)
        {
            _isInvulnerable = false;
        }        
    }


}
