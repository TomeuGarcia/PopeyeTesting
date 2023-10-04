using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundedAnchor : MonoBehaviour
{
    [FormerlySerializedAs("playerController")] public PlayerController _playerController;
    public Anchor anchor;
    public Transform directionArrow;
    public AnchorHealthDrainer anchorHealthDrainer;
    
    public Player _player;
    
    //do not let player retrieve while in this state

    private float duration = 0.5f;
    private float distance = 10.0f;
    private float height = 3f;


    private bool damagingThrow = false;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MeleeAttack"))
        {
            anchorHealthDrainer.OnMeleeAttackToAnchor();

            AnchorAttract();
            
            if (damagingThrow)
            {
                DamagingThrow();
            }
            else
            {
                NonDamagingThrow();
            }
        }
    }
    
    public async void AnchorAttract()
    {
        await Task.Delay(400);

        _playerController.enabled = false;

        _player.SetInvulnerableForDuration(0.5f);
        _player.DropTargetForEnemiesForDuration(0.5f);
        await anchor.AttractOwner(0.3f);

        _playerController.enabled = true;
    }

    private void DamagingThrow()
    {
        anchor.ChangeState(Anchor.AnchorStates.OnAir);
        anchor.transform.DOMove(_playerController.LookDirection * 10f + anchor.transform.position, duration, false).SetEase(Ease.OutCirc);

        StartCoroutine(AAAA());
    }

    private void NonDamagingThrow()
    {
        Vector3 offsetToEnd = (_playerController.LookDirection) * distance + anchor.transform.position - anchor.transform.position;

        anchor.transform.DOBlendableMoveBy(offsetToEnd, duration)
            .SetEase(Ease.InOutSine);
        
        Vector3 moveArchDirection = Vector3.up;

        float firstDuration = duration / 4.0f;
        float secondDuration = duration - firstDuration;
        anchor.transform.DOBlendableMoveBy(moveArchDirection * height, firstDuration)
            .OnComplete(() => {
                anchor.transform.DOBlendableMoveBy(-moveArchDirection * height, secondDuration)
                    .SetEase(Ease.InOutSine);
            });
    }
    
    private IEnumerator AAAA()
    {
        yield return new WaitForSeconds(duration);
        anchor.ChangeState(Anchor.AnchorStates.OnGround);
    }

    private void Update()
    {
        directionArrow.transform.position = transform.position;
        transform.rotation = _playerController._lookTransform.rotation;
        directionArrow.transform.rotation = _playerController._lookTransform.rotation;
        

        if (Input.GetKeyDown(KeyCode.M))
        {
            damagingThrow = !damagingThrow;
        }
    }

    private void OnEnable()
    {
        directionArrow.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (directionArrow == null)
        {
            return;
        }
        directionArrow.gameObject.SetActive(false);
    }
}
