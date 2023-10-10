using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }


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
    }



    public bool TryDealDamage(GameObject hitObject, DamageHit damageHit, out DamageHitResult damageHitResult)
    {
        damageHitResult = null;
        if (!hitObject.TryGetComponent<IDamageHitTarget>(out IDamageHitTarget hitTarget))
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



}
