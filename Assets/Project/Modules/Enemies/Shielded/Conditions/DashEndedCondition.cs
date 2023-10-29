using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEndedCondition : StateTransitionCondition
{
    [SerializeField] private TurtleDashing _turtleDashing;
    public override bool IsMet() => _turtleDashing.DashEnded;
}
