using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FABRIKJointChain 
{
    private Transform _target;

    public Transform[] joints;
    public Vector3[] positionCopies;
    public float[] distances;

    public int NumberOfJoints => joints.Length;
    public Vector3 TargetPosition => _target.position;



    public FABRIKJointChain(Transform[] joints, Transform target)
    {
        _target = target;

        this.joints = joints;
        positionCopies = new Vector3[joints.Length];

        distances = new float[joints.Length - 1];
        for (int i = 0; i < distances.Length; ++i)
        {
            distances[i] = Vector3.Distance(joints[i].position, joints[i + 1].position);
        }
    }


    private float RootCopyToTargetDistance()
    {
        return Vector3.Distance(positionCopies[0], _target.position);
    }
    
    public float EndEffectorCopyToTargetDistance()
    {
        return Vector3.Distance(positionCopies[NumberOfJoints-1], _target.position);
    }

    private float DistancesSum()
    {
        return distances.Sum();
    }
    
    public bool IsTargetUnreachable()
    {
        return RootCopyToTargetDistance() > DistancesSum();
    }

    

}
