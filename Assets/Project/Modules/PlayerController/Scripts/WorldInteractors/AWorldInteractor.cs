using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWorldInteractor : MonoBehaviour
{
    [Header("ACTIVATION INPUT COUNT")]
    [SerializeField, Range(1, 10)] private int _activationInputsCount = 1;
    private int _currentActivationInputsCount = 0;
    public int ActivationInputsCount => _activationInputsCount;


    private void Awake()
    {
        AwakeInit();
    }


    public void AddActivationInput()
    {
        if (++_currentActivationInputsCount == _activationInputsCount)
        {
            EnterActivatedState();
        }        
    }
    
    public void AddDeactivationInput()
    {
        if (--_currentActivationInputsCount == _activationInputsCount - 1)
        {
            EnterDeactivatedState();
        }
    }


    protected abstract void AwakeInit();
    protected abstract void EnterActivatedState();
    protected abstract void EnterDeactivatedState();


}
