using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSnapper : MonoBehaviour
{    
    private AnchorSnapTarget _currentSnapTarget;
    [SerializeField] private LayerMask _snapTargetLayerMask;
    [SerializeField, Range(0.0f, 100.0f)] private float _snapTargetProbeDistance = 50.0f;


    public bool HasSnapTarget(Vector3[] trajectoryPath)
    {
        for (int i = 0; i < trajectoryPath.Length - 1; ++i)
        {
            Vector3 startPosition = trajectoryPath[i];
            Vector3 endPosition = trajectoryPath[i + 1];
            Vector3 direction = (endPosition - startPosition).normalized;


            if (Physics.Raycast(startPosition, direction, out RaycastHit raycastHit, _snapTargetProbeDistance,
                _snapTargetLayerMask, QueryTriggerInteraction.Collide))
            {
                if (raycastHit.collider.TryGetComponent<AnchorSnapTarget>(out _currentSnapTarget))
                {
                    if (_currentSnapTarget.CanSnapFromPosition(raycastHit.point))
                    {
                        return true;
                    }
                }
            }
        }       


        return false;
    }
    
    public bool HasSnapTarget_OnlyLast(Vector3[] trajectoryPath)
    {
        Vector3 startPosition = trajectoryPath[trajectoryPath.Length - 2];
        Vector3 endPosition = trajectoryPath[trajectoryPath.Length - 1];
        Vector3 direction = (endPosition - startPosition).normalized;


        if (!Physics.Raycast(startPosition, direction, out RaycastHit raycastHit, _snapTargetProbeDistance, 
            _snapTargetLayerMask, QueryTriggerInteraction.Collide))
        {
            return false;
        }


        if (!raycastHit.collider.TryGetComponent<AnchorSnapTarget>(out AnchorSnapTarget anchorSnapTarget))
        {            
            return false;
        }

        if (!_currentSnapTarget.CanSnapFromPosition(raycastHit.point))
        {
            return false;
        }

        _currentSnapTarget = anchorSnapTarget;

        return true;
    }


    public void CorrectTrajectoryPath(Vector3[] trajectoryPath)
    {
        Vector3 snapPosition = _currentSnapTarget.SnapPosition;
        float step = 1.0f / (trajectoryPath.Length - 1);
        float t = 0.0f;

        for (int i = 0; i < trajectoryPath.Length; ++i)
        {
            trajectoryPath[i] = Vector3.Lerp(trajectoryPath[i], snapPosition, t);
            t += step;
        }
    }


    public void ConfirmSnapping(float durationBeforeReachingTarget)
    {
        _currentSnapTarget.PlaySnapAnimation(durationBeforeReachingTarget).Forget();
    }

    public void ClearState()
    {
        _currentSnapTarget = null;
    }

}
