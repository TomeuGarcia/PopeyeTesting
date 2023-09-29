using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBoneChainController : MonoBehaviour
{
    private FABRIKControllerBehaviour _FABRIKController;
    private BoneChain _boneChain;
    private Transform _target;

    [SerializeField] private BoneChainPoseController _boneChainPoseController;
    [SerializeField] private bool _instantMovement = true;
    [SerializeField] private bool _skipTargetAnimation = false;

    [SerializeField] AnimationCurve targetAnimationY;

    private bool _isUsingIK;
    private Vector3 _boneChainStartPosition;
    [SerializeField, Range(0.1f, 3.0f)] private float _animationDuration = 0.3f;
    [SerializeField, Range(10f, 30f)] private int _animationSegments = 20;
    [SerializeField, Range(0.1f, 5.0f)] private float _animationLength = 4.0f;
    [SerializeField, Range(0.1f, 5.0f)] private float _animationHeight = 1.0f;
    [SerializeField] private LineRenderer _animationLine;
    [SerializeField] private bool _moveTarget = true;

    public void AwakeInit(FABRIKControllerBehaviour FABRIKController, BoneChain boneChain, Transform target)
    {
        _FABRIKController = FABRIKController;
        _boneChain = boneChain;
        _target = target;

        _boneChainStartPosition = _boneChain.transform.position;

        DisableIKs();
        CurlBones();
        SetupAnimationLine();
    }

    private void OnValidate()
    {
        SetupAnimationLine();
    }


    private void Update()
    {
        _boneChain.UpdateBoneGeneration();

        if (Input.GetKeyDown(KeyCode.S))
        {
            //StraightenBones();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (_isUsingIK)
            {
                DisableIKs();
                CurlBones();                
            }
            else
            {
                EnableIKs();
                StartTargetAnimation();
            }            
        }
    }


    private void StraightenBones()
    {
        if (_instantMovement)
        {
            _boneChainPoseController.StraightenBonesInstantly(_boneChain, _target);
        }
        else
        {
            _boneChainPoseController.StraightenBones(_boneChain, _target);
        }
    }

    private void CurlBones()
    {
        if (_instantMovement)
        {
            _boneChainPoseController.CurlBonesInstantly(_boneChain);
        }
        else
        {
            _boneChainPoseController.CurlBones(_boneChain);
        }
    }


    private void EnableIKs()
    {
        _isUsingIK = true;
        _FABRIKController.enabled = true;
    }
    private void DisableIKs()
    {
        _isUsingIK = false;
        _FABRIKController.enabled = false;
    }

    private void StartTargetAnimation()
    {
        _boneChain.transform.position = _boneChainStartPosition;
        _target.position = _boneChainStartPosition;
        if (_skipTargetAnimation) return;


        Transform movingObject = _moveTarget ? _target : _boneChain.transform;

        //movingObject.DOKill();

        //movingObject.DOBlendableMoveBy(Vector3.right * _animationLength, _animationDuration);
        //movingObject.DOBlendableMoveBy(Vector3.up * _animationHeight, _animationDuration).SetEase(targetAnimationY);

        float t = 0.0f;
        DOTween.To(
            () => t,
            (value) => { 
                t = value; 
                movingObject.position = (Vector3.right * _animationLength * t) + (Vector3.up * _animationHeight * targetAnimationY.Evaluate(t)) + _boneChainStartPosition; 
            },
            1.0f,
            _animationDuration
            );

    }

    private void SetupAnimationLine()
    {
        Vector3[] positions = new Vector3[_animationSegments];

        float segmentStep = _animationDuration / _animationSegments;


        for (int i = 0; i < _animationSegments; ++i)
        {
            float t = (segmentStep * i) / _animationDuration;
            positions[i] = (Vector3.right * _animationLength * t) + (Vector3.up * _animationHeight * targetAnimationY.Evaluate(t));
        }

        _animationLine.positionCount = _animationSegments;
        _animationLine.SetPositions(positions);
    }

}
