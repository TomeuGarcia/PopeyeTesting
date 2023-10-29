using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsStunnedCondition : StateTransitionCondition
{
    [SerializeField] private TurtleDashing _turtleDashing;
    public override bool IsMet() => _turtleDashing.Stunned;
}
