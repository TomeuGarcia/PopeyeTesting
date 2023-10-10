using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetableBehaviour : MonoBehaviour
{
 [SerializeField] public Transform targetPoint = null;
 //[SerializeField] private BelongsToBehaviour belongsToBehaviour = null;

 public Transform TargetPoint => targetPoint;
 //public BelongsToBehaviour BelongsToBehaviour => belongsToBehaviour;
}


[Serializable]
public class UnityTargetableEvent : UnityEvent<TargetableBehaviour> { }
