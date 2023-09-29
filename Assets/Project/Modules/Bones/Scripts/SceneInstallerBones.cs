using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstallerBones : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private BoneChain _boneChain;
    [SerializeField] private IKBoneChainController _IKBoneChainController;
    [SerializeField] private FABRIKControllerBehaviour _FABRIKControllerBehaviour;

    private void Awake()
    {
        _boneChain.AwakeInit();
        _FABRIKControllerBehaviour.AwakeInit(_boneChain, _target);
        _IKBoneChainController.AwakeInit(_FABRIKControllerBehaviour, _boneChain, _target);
    }


    
}
