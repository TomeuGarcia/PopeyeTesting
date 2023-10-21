using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }


    private DamageHitTargetType _damageOnlyPlayerPreset;
    private DamageHitTargetType _damageOnlyEnemiesPreset;
    private DamageHitTargetType _damageEnemiesAndDestructiblesPreset;
    private DamageHitTargetType _damageEnemiesDestructiblesAndInteractablesPreset;
    private DamageHitTargetType _damagePlayerAndEnemiesPreset;
    
    public DamageHitTargetType DamageOnlyPlayerPreset => _damageOnlyPlayerPreset;
    public DamageHitTargetType DamageOnlyEnemiesPreset => _damageOnlyEnemiesPreset;
    public DamageHitTargetType DamageEnemiesAndDestructiblesPreset => _damageEnemiesAndDestructiblesPreset;
    public DamageHitTargetType DamageEnemiesDestructiblesAndInteractablesPreset => _damageEnemiesDestructiblesAndInteractablesPreset;
    public DamageHitTargetType DamagePlayerAndEnemiesPreset => _damagePlayerAndEnemiesPreset;





    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        Instance = this;

        AwakeInit();
    }


    private void AwakeInit()
    {
        DamageHitTargetTypeUtilities.AddType(ref _damageOnlyPlayerPreset, DamageHitTargetType.Player);

        DamageHitTargetTypeUtilities.AddType(ref _damageOnlyEnemiesPreset, DamageHitTargetType.Enemy);

        DamageHitTargetTypeUtilities.AddType(ref _damageEnemiesAndDestructiblesPreset, DamageHitTargetType.Enemy);
        DamageHitTargetTypeUtilities.AddType(ref _damageEnemiesAndDestructiblesPreset, DamageHitTargetType.Destructible);
        
        DamageHitTargetTypeUtilities.AddType(ref _damageEnemiesDestructiblesAndInteractablesPreset, DamageHitTargetType.Enemy);
        DamageHitTargetTypeUtilities.AddType(ref _damageEnemiesDestructiblesAndInteractablesPreset, DamageHitTargetType.Destructible);
        DamageHitTargetTypeUtilities.AddType(ref _damageEnemiesDestructiblesAndInteractablesPreset, DamageHitTargetType.Interactable);

        DamageHitTargetTypeUtilities.AddType(ref _damagePlayerAndEnemiesPreset, DamageHitTargetType.Player);
        DamageHitTargetTypeUtilities.AddType(ref _damagePlayerAndEnemiesPreset, DamageHitTargetType.Enemy);
    }



    public bool TryDealDamage(GameObject hitObject, DamageHit damageHit, out DamageHitResult damageHitResult)
    {
        damageHitResult = null;
        if (!hitObject.TryGetComponent<IDamageHitTarget>(out IDamageHitTarget hitTarget))
        {
            return false;
        }

        if (DamageHitIgnoresDamageTarget(damageHit, hitTarget))
        {
            return false;
        }


        if (!hitTarget.CanBeDamaged(damageHit))
        {
            return false;
        }

        damageHitResult = hitTarget.TakeHitDamage(damageHit);
        return true;
    }


    private bool DamageHitIgnoresDamageTarget(DamageHit damageHit, IDamageHitTarget hitTarget)
    {
        DamageHitTargetType damageHitTypeMask = damageHit.DamageHitTargetTypeMask;
        DamageHitTargetType damageTargetType = hitTarget.GetDamageHitTargetType();

        return !damageHitTypeMask.HasFlag(damageTargetType);
    }

}
