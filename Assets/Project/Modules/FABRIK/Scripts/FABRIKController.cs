using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FABRIKController 
{
    private float _angleThreshold;
    private float _distanceToEndEffectorTolerance;

    private List<FABRIKJointChain> _jointChains;

    public FABRIKController()
    {
        _angleThreshold = 1.0f;
        _distanceToEndEffectorTolerance = 0.01f;

        _jointChains = new List<FABRIKJointChain>();
    }

    public void AddJointChain(FABRIKJointChain jointChain)
    {
        _jointChains.Add(jointChain);
    }
    public void RemoveJointChains()
    {
        _jointChains.Clear();
    }

    public void Update()
    {
        foreach (FABRIKJointChain jointChain in _jointChains)
        {
            UpdateJointChain(jointChain);
        }
    }

    private void UpdateJointChain(FABRIKJointChain jointChain)
    {
        ResetPositionCopies(jointChain);

        // Update joints' position copies
        if (jointChain.IsTargetUnreachable())
        {
            SetPositionsStraight(jointChain);
        }
        else
        {
            // Check wether the distance between the end effector and the target is greater than tolerance
            while (jointChain.EndEffectorCopyToTargetDistance() > _distanceToEndEffectorTolerance)
            {
                // STAGE 1: FORWARD REACHING
                ForwardReaching(jointChain);

                // STAGE 2: BACKWARD REACHING
                BackwardReaching(jointChain);
            }
        }

        // Update original joint rotations
        UpdateJoints(jointChain);
    }


    private void ResetPositionCopies(FABRIKJointChain jointChain)
    {
        for (int i = 0; i < jointChain.NumberOfJoints; ++i)
        {
            jointChain.positionCopies[i] = jointChain.joints[i].position;
        }
    }

    private void SetPositionsStraight(FABRIKJointChain jointChain)
    {
        for (int i = 0; i < jointChain.NumberOfJoints - 1; ++i)
        {
            // Find the distance between the target and the joint
            float targetToJointDist = Vector3.Distance(jointChain.TargetPosition, jointChain.positionCopies[i]);
            float ratio = jointChain.distances[i] / targetToJointDist;

            // Find the new joint position
            jointChain.positionCopies[i + 1] = (1 - ratio) * jointChain.positionCopies[i] + ratio * jointChain.TargetPosition;
        }
    }


    private void ForwardReaching(FABRIKJointChain jointChain)
    {
        // Set end effector as target
        jointChain.positionCopies[jointChain.NumberOfJoints - 1] = jointChain.TargetPosition;

        for (int i = jointChain.NumberOfJoints - 2; i >= 0; --i)
        {
            // Find the distance between the new joint position (i+1) and the current joint (i)
            float distanceBetweenJoints = Vector3.Distance(jointChain.positionCopies[i], jointChain.positionCopies[i + 1]);
            float ratio = jointChain.distances[i] / distanceBetweenJoints;

            // Find the new joint position
            jointChain.positionCopies[i] = (1 - ratio) * jointChain.positionCopies[i + 1] + ratio * jointChain.positionCopies[i];
        }
    }

    private void BackwardReaching(FABRIKJointChain jointChain)
    {
        // Set the root its initial position
        jointChain.positionCopies[0] = jointChain.joints[0].position;

        for (int i = 1; i < jointChain.NumberOfJoints - 1; ++i)
        {
            // Find the distance between the new joint position (i+1) and the current joint (i)
            float distanceJoints = Vector3.Distance(jointChain.positionCopies[i - 1], jointChain.positionCopies[i]);
            float ratio = jointChain.distances[i - 1] / distanceJoints;

            // Find the new joint position
            jointChain.positionCopies[i] = (1 - ratio) * jointChain.positionCopies[i - 1] + ratio * jointChain.positionCopies[i];
        }
    }

    private void UpdateJoints(FABRIKJointChain jointChain)
    {
        for (int i = 0; i < jointChain.NumberOfJoints - 1; i++)
        {
            Vector3 oldDirection = (jointChain.joints[i + 1].position - jointChain.joints[i].position).normalized;
            Vector3 newDirection = (jointChain.positionCopies[i + 1] - jointChain.positionCopies[i]).normalized;

            Vector3 axis = Vector3.Cross(oldDirection, newDirection).normalized;
            float angle = Mathf.Acos(Vector3.Dot(oldDirection, newDirection)) * Mathf.Rad2Deg;

            if (angle > _angleThreshold)
            {
                jointChain.joints[i].rotation = Quaternion.AngleAxis(angle, axis) * jointChain.joints[i].rotation;
            }
        }
    }

}
