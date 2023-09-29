using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedAnchorPullAttackState : IPlayerState
{
    private PlayerController _playerController;
    private Anchor _anchor;
    private float _maxMoveSpeed;
    private readonly ActionMovesetInputHandler _movesetInputHandler;


    public PlacedAnchorPullAttackState(Player player, Anchor anchor, float maxMoveSpeed, ActionMovesetInputHandler movesetInputHandler)
    {
        _playerController = player.PlayerController;
        _anchor = anchor;
        _maxMoveSpeed = maxMoveSpeed;
        _movesetInputHandler = movesetInputHandler;
    }


    protected override void DoEnter()
    {
        _playerController.MaxSpeed = _maxMoveSpeed;
    }

    public override void Exit()
    {

    }

    public override bool Update()
    {

        if (_movesetInputHandler.IsPullAttack_HoldPressed())
        {

        }
        else if (_movesetInputHandler.IsPullAttack_Released())
        {

        }

        return false;
    }


    
}
