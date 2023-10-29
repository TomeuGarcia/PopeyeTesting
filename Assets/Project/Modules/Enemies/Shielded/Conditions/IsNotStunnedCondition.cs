using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsNotStunnedCondition : StateTransitionCondition
{
    [SerializeField] private TurtleStunned _turtleStunned;
    public override bool IsMet() => !_turtleStunned.Stunned;
}
