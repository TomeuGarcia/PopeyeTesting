using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWorldInteractor : MonoBehaviour
{

    private void Awake()
    {
        AwakeInit();
    }


    protected abstract void AwakeInit();
    public abstract void EnterActivatedState();
    public abstract void EnterDeactivatedState();


}
