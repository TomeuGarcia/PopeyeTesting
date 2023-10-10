using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasNotTargetCondition : StateTransitionCondition
{
    [SerializeField] private ProximityTargetGetterBehaviour targetGetterBehaviour = null;
    [SerializeField] private bool shouldHaveTarget = true;

    public override bool IsMet() => targetGetterBehaviour.HasTarget == shouldHaveTarget;
}
