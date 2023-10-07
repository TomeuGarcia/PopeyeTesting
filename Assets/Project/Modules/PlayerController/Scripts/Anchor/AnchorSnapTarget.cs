using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSnapTarget : MonoBehaviour
{

    [SerializeField] private Transform _snapSpot;
    [SerializeField] private Transform _clawsTransform;
    public Vector3 SnapPosition => _snapSpot.position;



    public bool CanSnapFromPosition(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        float dot = Vector3.Dot(direction, transform.up);

        return dot > 0.0f;
    }


    public async UniTaskVoid PlaySnapAnimation(float delay)
    {
        await UniTask.Delay(MathUtilities.SecondsToMilliseconds(delay*0.9f));

        _clawsTransform.DOPunchScale(new Vector3(0.5f, 0.1f, 0.5f), 0.25f, 5);
    }


}
