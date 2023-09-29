using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNotifier : MonoBehaviour
{
    [SerializeField] private Collider _collider;

    public delegate void TriggerNotifierEvent(Collider otherCollider);
    public TriggerNotifierEvent OnEnter;
    public TriggerNotifierEvent OnStay;
    public TriggerNotifierEvent OnExit;


    private void OnTriggerEnter(Collider otherCollider)
    {
        OnEnter?.Invoke(otherCollider);
    }

    private void OnTriggerStay(Collider otherCollider)
    {
        OnStay?.Invoke(otherCollider);
    }

    private void OnTriggerExit(Collider otherCollider)
    {        
        OnExit?.Invoke(otherCollider);
    }


    public void EnableCollider()
    {
        _collider.enabled = true;
    }
    
    public void DisableCollider()
    {
        _collider.enabled = false;
    }

}
