using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrackerBehaviour : MonoBehaviour
{
   [SerializeField] private LineRenderer lineRenderer = null;
   [SerializeField] private Transform fromTransform = null;
   private Transform _toTransform = null;

   public void SetTarget(Transform transform)
   {
      _toTransform = transform;
      lineRenderer.positionCount = 2;
   }

   public void SetTarget(TargetableBehaviour target) => SetTarget(target.targetPoint);

   public void ClearTarget()
   {
      lineRenderer.positionCount = 0;
      _toTransform = null;
   }

   private void LateUpdate()
   {
      if (_toTransform == null || fromTransform == null) { return; }
      
      lineRenderer.SetPosition(0,fromTransform.position);
      lineRenderer.SetPosition(1,_toTransform.position);
   }
}
