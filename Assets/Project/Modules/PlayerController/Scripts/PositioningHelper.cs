using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositioningHelper : MonoBehaviour
{
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask _floorLayerMask;
    [SerializeField, Range(0.0f, 10.0f)] private float _floorProbeDistance = 2.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float _skinDistance = 0.01f;

    public static PositioningHelper Instance
    {
        get;
        private set;
    }
    


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }


    public Vector3 GetGoalPositionCheckingObstacles(Vector3 startPosition, Vector3 goalPosition, out float distanceRatio01, float positionOffsetIfCollision = 0.5f)
    {
        Vector3 direction = GetDirectionAlignedWithFloor(startPosition, goalPosition);
        float distance = (goalPosition - startPosition).magnitude;

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, distance, _obstaclesLayerMask, QueryTriggerInteraction.Ignore))
        {
            goalPosition -= direction * positionOffsetIfCollision;
        }

        distanceRatio01 = hit.distance / distance;
        return goalPosition;
    }
    

    public Vector3 GetDirectionAlignedWithFloor(Vector3 startPosition, Vector3 goalPosition)
    {
        Vector3 forward = (goalPosition - startPosition).normalized;

        if (Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, _floorProbeDistance, _floorLayerMask, QueryTriggerInteraction.Ignore))
        {
            Vector3 right = Vector3.Cross(forward, hit.normal).normalized;
            forward = Vector3.Cross(hit.normal, right).normalized;
        }

        return forward;
    }


    private Vector3 GetCollideandSlideResult(Vector3 startPosition, Vector3 goalPosition)
    {
        goalPosition = CollideAndSlide(startPosition, goalPosition, 5);

        return goalPosition - startPosition;
    }    

    private Vector3 CollideAndSlide(Vector3 startPosition, Vector3 goalPosition, int step)
    {
        // Need to revise everything EHEHE

        if (step == 0)
        {
            return Vector3.zero;
        }


        Vector3 displacement = goalPosition - startPosition;
        Vector3 direction = displacement.normalized;
        float distance = displacement.magnitude;


        if (!Physics.Raycast(startPosition, direction, out RaycastHit hit, distance, _obstaclesLayerMask, QueryTriggerInteraction.Ignore))
        {
            return displacement;
        }


        float remainingDistance = distance - hit.distance;
        Vector3 pastCollisionDisplacement = direction * remainingDistance;

        Vector3 projectedDisplacement = Vector3.Project(pastCollisionDisplacement, hit.normal).normalized * remainingDistance;


        startPosition = hit.point + hit.normal * _skinDistance;
        goalPosition = startPosition + projectedDisplacement;

        return displacement + CollideAndSlide(startPosition, goalPosition, step -1);
    }


}
