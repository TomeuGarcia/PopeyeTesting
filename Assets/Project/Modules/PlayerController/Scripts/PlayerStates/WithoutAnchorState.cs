using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithoutAnchorState : IPlayerState
{
    private Player _player;
    private PlayerController _playerController;
    private Anchor _anchor;
    private float _maxMoveSpeed;
    private float _maxMoveSpeedPullingAnchor;
    private readonly ActionMovesetInputHandler _movesetInputHandler;

    private bool _ownerIsBeingAttracted;
    private float _attractDuration;
    private float _attractInvulnerableDuration;


    public WithoutAnchorState(Player player, Anchor anchor, float maxMoveSpeed, float maxMoveSpeedPullingAnchor,
        ActionMovesetInputHandler movesetInputHandler)
    {
        _player = player;
        _playerController = player.PlayerController;
        _anchor = anchor;
        _maxMoveSpeed = maxMoveSpeed;
        _maxMoveSpeedPullingAnchor = maxMoveSpeedPullingAnchor;
        _movesetInputHandler = movesetInputHandler;
        _attractDuration = 0.3f;
        _attractInvulnerableDuration = 0.5f;
    }


    protected override void DoEnter()
    {
        _playerController.MaxSpeed = _maxMoveSpeed;
        _ownerIsBeingAttracted = false;

        if (_movesetInputHandler.IsGrab_HoldPressed())
        {
            _anchor.SetPullMovable();
            _playerController.MaxSpeed = _maxMoveSpeedPullingAnchor;
        }
    }

    public override void Exit()
    {

    }

    public override bool Update()
    {
        if (_ownerIsBeingAttracted)
        {
            return false;
        }


        if (_movesetInputHandler.IsGrab_Pressed())
        {
            if (CanGrabAnchor())
            {
                _anchor.ReturnToOwner();
                _nextState = States.WithAnchor;
                return true;
            }

            _anchor.SetPullMovable();
            _playerController.MaxSpeed = _maxMoveSpeedPullingAnchor;
        }
        else if (_movesetInputHandler.IsGrab_HoldPressed())
        {
            if (CanGrabAnchor())
            {                
                _anchor.ReturnToOwner();
                _nextState = States.WithAnchor;
                return true;
            }

            _anchor.GetPulledTowardsOwner();
        }
        else if (_movesetInputHandler.IsGrab_Released())
        {
            _anchor.SnapToFloorAndSetStill();
            _playerController.MaxSpeed = _maxMoveSpeed;
        }

        if (_movesetInputHandler.IsMove_Released() && _anchor.IsOwnerTensionLimit())
        {
            AnchorAttract();
        }

        // TODO
        /*
        if (_movesetInputHandler.IsPullAttack_Pressed())
        {
            _nextState = States.PlacedAnchorPullAttack;
            return true;
        }
        */


        return false;
    }


    private bool CanGrabAnchor()
    {
        Vector3 anchorPlanePosition = _anchor.Position;
        anchorPlanePosition.y = 0f;

        Vector3 playerPlanePosition = _playerController.transform.position;
        playerPlanePosition.y = 0f;

        bool isWithinGrabDistance = Vector3.Distance(anchorPlanePosition, playerPlanePosition) < 3.0f;

        return isWithinGrabDistance && _anchor.CanBeGrabbed();
    }

    private async void AnchorAttract()
    {
        _ownerIsBeingAttracted = true;
        _playerController.enabled = false;

        _player.SetInvulnerableForDuration(_attractInvulnerableDuration);
        _player.DropTargetForEnemiesForDuration(_attractInvulnerableDuration);
        await _anchor.AttractOwner(_attractDuration);

        _playerController.enabled = true;
        _ownerIsBeingAttracted = false;
    }

}
