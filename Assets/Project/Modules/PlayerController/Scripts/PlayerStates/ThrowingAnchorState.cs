using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingAnchorState : IPlayerState
{
    private PlayerController _playerController;
    private Anchor _anchor;
    private float _maxMoveSpeed;
    private readonly ActionMovesetInputHandler _movesetInputHandler;

    public ThrowingAnchorState(Player player, Anchor anchor, float maxMoveSpeed, ActionMovesetInputHandler movesetInputHandler)
    {
        _playerController = player.PlayerController;
        _maxMoveSpeed = maxMoveSpeed;
        _anchor = anchor;
        _movesetInputHandler = movesetInputHandler;
    }


    protected override void DoEnter()
    {
        _playerController.MaxSpeed = _maxMoveSpeed;
    }

    public override void Exit()
    {

    }

    public override bool Update(float deltaTime)
    {
        if (!_anchor.IsOnAir())
        {
            _nextState = States.WithoutAnchor;
            return true;
        }

        return false;
    }
}
