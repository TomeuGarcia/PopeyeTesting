using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GroundedAnchor : MonoBehaviour
{
    public PlayerController playerController;
    public Anchor anchor;
    public Transform directionArrow;
    
    //do not let player retrieve while in this state

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MeleeAttack"))
        {
            //transform.rotation = playerController._lookTransform.rotation;
            anchor.ChangeState(Anchor.AnchorStates.OnAir);
            //anchor.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * 30f, ForceMode.Impulse);
            anchor.transform.DOMove(-playerController._lookTransform.forward * 10f + anchor.transform.position, 0.75f, false).SetEase(Ease.OutCirc);
            StartCoroutine(AAAA());
        }
    }

    private IEnumerator AAAA()
    {
        yield return new WaitForSeconds(0.75f);
        anchor.ChangeState(Anchor.AnchorStates.OnGround);
    }

    private void Update()
    {
        directionArrow.transform.position = transform.position;
        transform.rotation = playerController._lookTransform.rotation;
        directionArrow.transform.rotation = playerController._lookTransform.rotation;
        directionArrow.transform.RotateAround(directionArrow.transform.up, 180f * Mathf.Deg2Rad);
    }

    private void OnEnable()
    {
        directionArrow.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        directionArrow.gameObject.SetActive(false);
    }
}
