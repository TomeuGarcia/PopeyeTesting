using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProximityTargetGetterBehaviour : MonoBehaviour
{
   [SerializeField] private UnityTargetableEvent onTargetFound = new UnityTargetableEvent();
   [SerializeField] private UnityEvent onTargetLost = new UnityEvent();
   
   public TargetableBehaviour CurrentTarget { get; private set; }
   public bool HasTarget => CurrentTarget != null;

   private void OnTriggerEnter(Collider other)
   {
      if (HasTarget) { return; }

      if (!other.TryGetComponent(out TargetableBehaviour target))
      {
         return;
      }

      CurrentTarget = target;
      onTargetFound.Invoke(CurrentTarget);
   }

   private void OnTriggerExit(Collider other)
   {
      if (!HasTarget) { return; }
      if(!other.TryGetComponent(out TargetableBehaviour target))
      {
         return;
      }
      if (CurrentTarget != target) { return; }

      CurrentTarget = null;
      onTargetLost.Invoke();
   }
}
