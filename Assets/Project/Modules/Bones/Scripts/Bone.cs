using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{
    [SerializeField] private Transform _boneRoot;
    [SerializeField] private Transform _boneEnd;
    public Transform BoneRoot => _boneRoot;
    public Transform BoneEnd => _boneEnd;


    public void SetLocalRotation(Quaternion localRotation)
    {
        _boneRoot.localRotation = localRotation;
    }
    public void SetWorldRotation(Quaternion worldRotation)
    {
        _boneRoot.rotation = worldRotation;
    }
    public void SetLocalPosition(Vector3 localPosition)
    {
        _boneRoot.localPosition = localPosition;
    }

    public void SetLocalRotation(Quaternion localRotation, float duration, Ease ease = Ease.InOutSine)
    {
        _boneRoot.DOLocalRotateQuaternion(localRotation, duration).SetEase(ease);
    }
    public void SetWorldRotation(Quaternion worldRotation, float duration, Ease ease = Ease.InOutSine)
    {
        _boneRoot.DORotateQuaternion(worldRotation, duration).SetEase(ease);
    }
    public void SetLocalPosition(Vector3 localPosition, float duration, Ease ease = Ease.InOutSine)
    {
        _boneRoot.DOLocalMove(localPosition, duration).SetEase(ease);
    }

    public void LookAt(Transform target)
    {
        Quaternion rotation = GetRotationToTarget(target.position);

        SetWorldRotation(rotation);
    }
    public void LookAt(Transform target, float duration, Ease ease = Ease.InOutSine)
    {
        Quaternion rotation = GetRotationToTarget(target.position);

        SetWorldRotation(rotation, duration, ease);
    }

    private Quaternion GetRotationToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - _boneRoot.position).normalized;

        float dot = Vector3.Dot(direction, _boneRoot.forward);

        if (dot > 0.99f)
        {
            return _boneRoot.rotation;
        }

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;        
        Vector3 axis = Vector3.Cross(_boneRoot.forward, direction).normalized;

        Quaternion offsetRotation = Quaternion.AngleAxis(angle, axis);

        return offsetRotation * _boneRoot.rotation;
    }
    

}
