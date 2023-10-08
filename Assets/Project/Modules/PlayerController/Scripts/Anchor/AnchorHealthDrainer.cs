using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHealthDrainer : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Anchor _anchor;
    [SerializeField] private AnchorDamageDealer _anchorDamageDealer;
    [SerializeField] private MeshRenderer _anchorMesh;
    private Material _anchorMaterial;
    [SerializeField] private LineRenderer _ownerBinderLine;
    private Material _chainMaterial;

    [Header("HEALING")]
    [SerializeField, Range(0.0f, 100.0f)] private float _healAmount = 10.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _healDuration = 3.0f;
    private float _healTimer;
    [SerializeField] private AnimationCurve _chargedCurve;

    [Header("PREFABS")]
    [SerializeField] private AnchorHealthDrainEffect _healthDrainEffectPrefab;


    private Player _player;
    private StaminaSystem _staminaSystem;
    private bool _canHeal;



    public void AwakeInit(StaminaSystem staminaSystem)
    {
        _staminaSystem = staminaSystem;
        _staminaSystem.OnValueUpdate += CheckReset;


        _canHeal = false;

        _anchorMaterial = _anchorMesh.material;
        _anchorMaterial.SetFloat("_IsCharged", 0.0f);

        _chainMaterial = _ownerBinderLine.material;
        SetChainVisuallyCharged(0.0f);
    }

    private void OnDestroy()
    {
        _staminaSystem.OnValueUpdate -= CheckReset;
    }

    private void Update()
    {
        UpdateHealTimer();
    }


    public void Init(Player healTarget)
    {
        _player = healTarget;
    }

    private void UpdateHealTimer()
    {
        if (_canHeal && _player.CanBeHealed())
        {
            float add = _anchor.IsOwnerTensionLimit() ? Time.deltaTime : -Time.deltaTime;
            _healTimer += add;
            _healTimer = Mathf.Max(_healTimer, 0.0f);

            if (_healTimer > _healDuration)
            {
                HealOwner();
            }

            SetChainVisuallyCharged(Mathf.Min(_healTimer / _healDuration, 1.0f));
        }     
    }

    private void CheckReset()
    {
        if (_canHeal && !_staminaSystem.HasMaxStamina())
        {
            _canHeal = false;
            _healTimer = 0.0f;
            SetChainVisuallyCharged(0.0f);
            _anchorMaterial.SetFloat("_IsCharged", 0.0f);
        }      
    }


    public void IncrementDrainedHealth(float restoreAmount)
    {
        _staminaSystem.Restore(restoreAmount);

        _canHeal = _staminaSystem.HasMaxStamina();

        _anchorMaterial.SetFloat("_IsCharged", _canHeal ? 1.0f : 0.0f);
    }
    
    public void OnMeleeAttackToAnchor()
    {
        _anchorMaterial.SetFloat("_IsCharged", _canHeal ? 1.0f : 0.0f);
    }

    private void HealOwner()
    {
        _canHeal = false;
        _healTimer = 0;        
        _staminaSystem.SpendAll();

        _player.Heal(_healAmount);

        SpawnDrainHealthEffect();

        _anchorMaterial.SetFloat("_IsCharged", 0.0f);
        SetChainVisuallyCharged(0.0f);
    }

    public void SpawnDrainHealthEffect()
    {
        int emissionCount = (int)(_healAmount * 1.0f);
        AnchorHealthDrainEffect healthDrainEffect = Instantiate(_healthDrainEffectPrefab);
        healthDrainEffect.Init(_player.transform, transform.position, emissionCount);
    }


    private void SetChainVisuallyCharged(float chargedPer1)
    {
        _chainMaterial.SetFloat("_ChargedPer1", _chargedCurve.Evaluate(chargedPer1));
    }

}
