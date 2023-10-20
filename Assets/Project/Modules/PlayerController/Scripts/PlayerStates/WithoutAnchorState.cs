using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private float _canGrabAnchorDistance;

    private bool _queuedPullAttack;


    public WithoutAnchorState(Player player, Anchor anchor, float maxMoveSpeed, float maxMoveSpeedPullingAnchor,
        ActionMovesetInputHandler movesetInputHandler)
    {
        _player = player;
        _playerController = player.PlayerController;
        _anchor = anchor;
        _maxMoveSpeed = maxMoveSpeed;
        _maxMoveSpeedPullingAnchor = maxMoveSpeedPullingAnchor;
        _movesetInputHandler = movesetInputHandler;
        _attractDuration = 0.4f;
        _attractInvulnerableDuration = 0.6f;
        _canGrabAnchorDistance = 3.0f;
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

        _queuedPullAttack = _movesetInputHandler.IsPullAttack_HoldPressed();
    }

    public override void Exit()
    {

    }

    public override bool Update(float deltaTime)
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

        if (_movesetInputHandler.IsMeleeAttack_Pressed())
        {
            MeleeAttack();
        }
        
        
        if (_movesetInputHandler.IsPullAttack_Pressed())
        {
            _anchor.SnapToFloorAndSetStill();

            _nextState = States.SpinAttack;
            return true;
        }
        

        /*
        if ((_movesetInputHandler.IsPullAttack_HoldPressed() || _queuedPullAttack) && _anchor.CanDoChargedPullAttack())
        {
            _anchor.SnapToFloorAndSetStill();

            _nextState = States.PlacedAnchorPullAttack;
            return true;
        }
        */

        if (_movesetInputHandler.IsExplosionAbility_Pressed() && _anchor.CanUseExplosionAbility())
        {
            _anchor.UseExplosionAbility();
        }
        

        return false;
    }


    private bool CanGrabAnchor()
    {
        Vector3 anchorPlanePosition = _anchor.Position;
        anchorPlanePosition.y = 0f;

        Vector3 playerPlanePosition = _playerController.transform.position;
        playerPlanePosition.y = 0f;

        bool isWithinGrabDistance = Vector3.Distance(anchorPlanePosition, playerPlanePosition) < _canGrabAnchorDistance;

        return isWithinGrabDistance && _anchor.CanBeGrabbed();
    }

    public async void AnchorAttract()
    {
        _ownerIsBeingAttracted = true;
        _playerController.enabled = false;

        _player.SetInvulnerableForDuration(_attractInvulnerableDuration);
        _player.DropTargetForEnemiesForDuration(_attractInvulnerableDuration);
        await _anchor.AttractOwner(_attractDuration);

        _playerController.enabled = true;
        _ownerIsBeingAttracted = false;
    }
        
    private async void MeleeAttack()
    {
        _playerController.MaxSpeed = 0.0f;
        _playerController.CanRotate = false;

        LungeForward(0.1f);
        await _player.MeleeAttack();
        _playerController.MaxSpeed = _maxMoveSpeed;
        _playerController.CanRotate = true;
    }


    private async void LungeForward(float delay)
    {
        await Task.Delay((int)(delay * 1000));
        _playerController.GetPushed(_playerController.LookDirection * 10.0f);

    }
}
