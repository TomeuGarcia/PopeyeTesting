using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSnapper : MonoBehaviour
{
    // TODO

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.TryGetComponent<AnchorSnapTarget>(out AnchorSnapTarget anchorSnapTarget))
        {
            //anchorSnapTarget.SnapPosition;
        }

    }
}
