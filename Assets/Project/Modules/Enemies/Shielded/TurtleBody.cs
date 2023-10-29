using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleBody : MonoBehaviour
{
    [SerializeField] private GameObject dashingBody;
    [SerializeField] private GameObject stunnedBody;
    [SerializeField] private GameObject idleBody;

    public void ActivateDashingVisuals()
    {
        dashingBody.SetActive(true);
        stunnedBody.SetActive(false);
        idleBody.SetActive(false);
    }
    
    public void ActivateIdleVisuals()
    {
        dashingBody.SetActive(false);
        stunnedBody.SetActive(false);
        idleBody.SetActive(true);
    }
    
    public void ActivateStunnedVisuals()
    {
        dashingBody.SetActive(false);
        stunnedBody.SetActive(true);
        idleBody.SetActive(false);
    }

}
