using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum DamageHitTargetType : int
{
    None = 0,                   // 0
    Player = 1 << 0,            // 1
    Enemy = 1 << 1,             // 2
    Destructible = 1 << 2,      // 4
    Interactable = 1 << 3       // 8 
                                // 16 ...
}



public static class DamageHitTargetTypeUtilities
{
    public static void AddType(ref DamageHitTargetType mask, DamageHitTargetType newType)
    {
        mask |= newType;
    }

    public static void RemoveType(ref DamageHitTargetType mask, DamageHitTargetType newType)
    {
        mask &= ~newType;
    }
}