using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleEnemyStatesCondition : StateTransitionCondition
{
    [SerializeField] private ProximityTargetGetterBehaviour targetGetterBehaviour = null;
    [SerializeField] private float minThresholdDistance;
    [SerializeField] private float maxThresholdDistance;
    [SerializeField] private Transform turtleTransform;

    public override bool IsMet()
    {
        if (targetGetterBehaviour.HasTarget)
        {
           float distance = Vector3.Distance(targetGetterBehaviour.CurrentTarget.transform.position, turtleTransform.position);
           if (distance > minThresholdDistance && distance < maxThresholdDistance)
           {
               return true;
           }
        }

        return false;
    }
}
