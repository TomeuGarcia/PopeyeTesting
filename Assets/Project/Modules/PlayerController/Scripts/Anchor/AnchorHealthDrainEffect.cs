using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHealthDrainEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;    


    public void Init(Transform followTransform, Vector3 hitTargetPosition, int emissionAmount)
    {
        transform.SetParent(followTransform);
        transform.localPosition = Vector3.zero;

        ParticleSystem.ShapeModule shapeModule = _particleSystem.shape;
        shapeModule.position = hitTargetPosition - followTransform.position;

        ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
        ParticleSystem.Burst burst = emissionModule.GetBurst(0);
        burst.count = emissionAmount;
        emissionModule.SetBurst(0, burst);

        _particleSystem.Play();                
    }


}
