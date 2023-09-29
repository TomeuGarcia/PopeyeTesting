using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstallerFABRIK : MonoBehaviour
{
    [SerializeField] private BoneChain _boneChain;
    [SerializeField] private Transform _target;

    [SerializeField] private FABRIKControllerBehaviour _FABRIKControllerBehaviour;

    private void Awake()
    {
        _boneChain.AwakeInit();
        _FABRIKControllerBehaviour.AwakeInit(_boneChain, _target);
    }

}
