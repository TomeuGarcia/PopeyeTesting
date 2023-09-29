using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoneChainHelper 
{
    public static FABRIKJointChain FABRIKJointChainFromBoneArm(BoneChain boneChain, Transform target)
    {
        Bone[] bones = boneChain.Bones;

        Transform[] joints = new Transform[bones.Length];
        for (int i = 0; i < bones.Length; ++i)
        {
            joints[i] = bones[i].BoneRoot;
        }

        return new FABRIKJointChain(joints, target);
    }   
}
