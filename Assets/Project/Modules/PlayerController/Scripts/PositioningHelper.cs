using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositioningHelper : MonoBehaviour
{
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private LayerMask _floorLayerMask;
    [SerializeField, Range(0.0f, 10.0f)] private float _floorProbeDistance = 2.0f;

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


}
