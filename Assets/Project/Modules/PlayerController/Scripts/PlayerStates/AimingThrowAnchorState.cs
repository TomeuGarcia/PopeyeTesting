using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingThrowAnchorState : IPlayerState
{
    private PlayerController _playerController;
    private Anchor _anchor;
    private float _maxMoveSpeed;
    private readonly ActionMovesetInputHandler _movesetInputHandler;
    private float _maxAimDuration;
    private float _aimTimer;

    private float ThrowStrength01 => _aimTimer / _maxAimDuration;

    private const float THROW_TIMER_THRESHOLD = 0.1f;

    public AimingThrowAnchorState(Player player, Anchor anchor, float maxMoveSpeed, ActionMovesetInputHandler movesetInputHandler, float maxAimDuration)
    {
        _playerController = player.PlayerController;
        _anchor = anchor;
        _maxMoveSpeed = maxMoveSpeed;
        _movesetInputHandler = movesetInputHandler;
        _maxAimDuration = maxAimDuration;
    }


    protected override void DoEnter()
    {
        _playerController.MaxSpeed = _maxMoveSpeed;
        _aimTimer = 0.0f;

        _anchor.SetAimingPosition();
    }

    public override void Exit()
    {
        _anchor.HideTrajectory();
    }

    public override bool Update(float deltaTime)
    {
        if (_movesetInputHandler.IsThrow_HoldPressed())
        {
            if (_aimTimer < _maxAimDuration)
            {
                _aimTimer += deltaTime;
            }
            _anchor.ShowTrajectory();
            AimAnchor();            
        }
        else if (_movesetInputHandler.IsThrow_Released())
        {            
            if (_aimTimer < THROW_TIMER_THRESHOLD)
            {
                _nextState = States.WithAnchor;
                return true;
            }

            ThrowAnchor();
            _nextState = States.ThrowingAnchor;            
            return true;
        }

        if (_movesetInputHandler.IsAim_Released())
        {
            _nextState = States.WithAnchor;
            return true;
        }

        return false;
    }

    private void AimAnchor()
    {        
        _anchor.ComputeTrajectory(ThrowStrength01, _playerController.LookDirection);
    }

    private void ThrowAnchor()
    {
        _anchor.GetThrown(ThrowStrength01, _playerController.LookDirection);
    }

}
