using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WithAnchorState : IPlayerState
{
    private PlayerController _playerController;
    private Anchor _anchor;
    private float _maxMoveSpeed;
    private readonly ActionMovesetInputHandler _movesetInputHandler;

    private bool _transitionToAiming;    
    private bool _queuedAnchorAim;

    private float _deltaTime;


    public WithAnchorState(Player player, Anchor anchor, float maxMoveSpeed, ActionMovesetInputHandler movesetInputHandler)        
    {
        _playerController = player.PlayerController;
        _anchor = anchor;
        _maxMoveSpeed = maxMoveSpeed;
        _movesetInputHandler = movesetInputHandler;
    }


    protected override void DoEnter()
    {
        _playerController.MaxSpeed = _maxMoveSpeed;

        _anchor.SetGrabbedPosition();
        _anchor.CanMeleeAttack = true;

        _transitionToAiming = false;

        _queuedAnchorAim = false;

        if (_movesetInputHandler.IsThrow_HoldPressed())
        {
            DelayedStartAiming(0.5f);
        }
    }

    public override void Exit()
    {
        
    }

    public override bool Update(float deltaTime)
    {
        _deltaTime = deltaTime;

        if (!_anchor.CanMeleeAttack)
        {
            if (_queuedAnchorAim && _movesetInputHandler.IsThrow_Released())
            {
                _queuedAnchorAim = false;
            }
            else if (!_queuedAnchorAim && _movesetInputHandler.IsThrow_Pressed())
            {
                _queuedAnchorAim = true;
            }


            return false;
        }


        if (_movesetInputHandler.IsAim_HoldPressed() || _movesetInputHandler.IsThrow_Pressed() || _transitionToAiming || _queuedAnchorAim)
        {
            _nextState = States.AimingThrowAnchor;
            return true;
        }

        if (_movesetInputHandler.IsMelee2_Pressed())
        {
            //MeleeAttack();

            _nextState = States.SpinAttack;
            return true;
        }

        return false;
    }

    private async void MeleeAttack()
    {
        _playerController.MaxSpeed = 1.0f;

        LungeForward(0.1f);
        await _anchor.MeleeAttack();
        _playerController.MaxSpeed = _maxMoveSpeed;
    }

    private async void LungeForward(float delay)
    {
        await Task.Delay((int)(delay * 1000));
        _playerController.GetPushed(_playerController.LookDirection * 10.0f);

    }


    private async void DelayedStartAiming(float duration)
    {
        bool throwIsHoldPressed = true;
        float timer = 0.0f;

        while (throwIsHoldPressed && timer < duration)
        {
            throwIsHoldPressed = _movesetInputHandler.IsThrow_HoldPressed();
            timer += _deltaTime;
            await Task.Delay((int)(_deltaTime * 1000));
        }

        _transitionToAiming = throwIsHoldPressed && timer >= duration;
    }

}
