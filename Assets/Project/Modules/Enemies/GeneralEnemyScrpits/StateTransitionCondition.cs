using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateTransitionCondition : MonoBehaviour
{
    public abstract bool IsMet();
}
