using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneChain : MonoBehaviour
{
    [SerializeField] private Bone _bonePrefab;
    [SerializeField] private Bone _boneEndEffectorPrefab;
    [SerializeField, Range(1, 30)] private int _numberOfBones = 1;
    private int _oldNumberOfBones;


    [SerializeField] private Transform _firstBoneParent;
    [SerializeField] public bool autoUpdateBoneGeneration = true;
    public Bone[] Bones { get; private set; }

    public delegate void BoneArmEvent();
    public BoneArmEvent OnGenerationUpdate;



    public void AwakeInit()
    {
        _oldNumberOfBones = -1;
        Bones = new Bone[0];

        UpdateBoneGeneration();
    }

    private void Update()
    {
        if (autoUpdateBoneGeneration)
        {
            UpdateBoneGeneration();
        }        
    }


    public void UpdateBoneGeneration()
    {
        if (_bonePrefab == null || _numberOfBones < 1 || _firstBoneParent == null) return;

        if (_numberOfBones == _oldNumberOfBones) return;

        DestroyBones();
        SpawnBones();

        _oldNumberOfBones = _numberOfBones;

        OnGenerationUpdate?.Invoke();
    }


    private void SpawnBones()
    {
        Bones = new Bone[_numberOfBones];

        Bones[0] = Instantiate(_bonePrefab, _firstBoneParent);

        for (int i = 1; i < _numberOfBones - 1; ++i)
        {
            Bones[i] = Instantiate(_bonePrefab, Bones[i - 1].BoneEnd);
        }
        if (_numberOfBones > 1)
        {
            Bones[_numberOfBones - 1] = Instantiate(_boneEndEffectorPrefab, Bones[_numberOfBones - 2].BoneEnd);
        }        
    }

    private void DestroyBones()
    {
        if (Bones.Length < 1) return;

        Destroy(Bones[0].gameObject);
    }

}
