using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSnapTarget : MonoBehaviour
{

    [SerializeField] private Transform _snapSpot;
    [SerializeField] private Transform _clawsTransform;
    [SerializeField] private Transform[] _claws;
    public Vector3 SnapPosition => _snapSpot.position;
    public Vector3 LookDirection => transform.up;
    public Vector3 UpDirection => -transform.right;



    public bool CanSnapFromPosition(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        float dot = Vector3.Dot(direction, LookDirection);

        return dot > 0.0f;
    }

    public Quaternion GetSnapRotation()
    {
        return Quaternion.AngleAxis(-45.0f, LookDirection) * Quaternion.LookRotation(-LookDirection, UpDirection);
    }


    public async UniTaskVoid PlaySnapAnimation(float delay)
    {
        float delay1 = delay * 0.7f;
        float delay2 = delay * 0.2f;

        for (int i = 0; i < _claws.Length; ++i)
        {
            Quaternion rotation = _claws[i].localRotation * Quaternion.Euler(0f, 0f, 20f);
            _claws[i].DOLocalRotateQuaternion(rotation, delay1);
            
        }

        await UniTask.Delay(MathUtilities.SecondsToMilliseconds(delay1));

        for (int i = 0; i < _claws.Length; ++i)
        {
            Quaternion rotation = _claws[i].localRotation * Quaternion.Euler(0f, 0f, -20f);
            _claws[i].DOLocalRotateQuaternion(rotation, delay2);
        }

        await UniTask.Delay(MathUtilities.SecondsToMilliseconds(delay2));

        _clawsTransform.DOPunchScale(new Vector3(0.2f, 0.1f, 0.2f), 0.25f, 5);
    }


}
