using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedAnchorPullAttackState : IPlayerState
{
    private PlayerController _playerController;
    private Anchor _anchor;
    private float _maxMoveSpeed;
    private readonly ActionMovesetInputHandler _movesetInputHandler;

    private float _attackChargeDuration;
    private float _attackChargeTimer;

    private float _attackExecuteDuration;
    private float _attackExecuteTimer;


    private AttackStage _currentStage;
    private enum AttackStage
    {
        Charging,
        Executing
    }


    public PlacedAnchorPullAttackState(Player player, Anchor anchor, float maxMoveSpeed, ActionMovesetInputHandler movesetInputHandler)
    {
        _playerController = player.PlayerController;
        _anchor = anchor;
        _maxMoveSpeed = maxMoveSpeed;
        _movesetInputHandler = movesetInputHandler;

        _attackChargeDuration = 0.2f;
        _attackExecuteDuration = 0.3f;
    }


    protected override void DoEnter()
    {
        _playerController.MaxSpeed = _maxMoveSpeed;
        //_playerController.CanRotate = false;

        _attackExecuteTimer = _attackChargeTimer = 0.0f;

        _currentStage = AttackStage.Charging;

    }

    public override void Exit()
    {
        //_playerController.CanRotate = true;
    }

    public override bool Update(float deltaTime)
    {
        if (_currentStage == AttackStage.Charging)
        {

            if (_movesetInputHandler.IsPullAttack_HoldPressed())
            {
                _attackChargeTimer += deltaTime;
            }
            else
            {
                _nextState = States.WithoutAnchor;
                return true;
            }

            if (_attackChargeTimer > _attackChargeDuration)
            {                
                _anchor.StartChargedPullAttack(_attackExecuteDuration);
                _currentStage = AttackStage.Executing;
            }


        }
        else if (_currentStage == AttackStage.Executing)
        {
            _attackExecuteTimer += deltaTime;

            if (_attackExecuteTimer > _attackExecuteDuration)
            {
                _nextState = States.WithoutAnchor;
                return true;
            }

        }
               

        return false;
    }


    
}
