using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_PlayerState : IPlayerState
{
    private Player _player;
    private Anchor _anchor;

    public Spawn_PlayerState(Player player, Anchor anchor)
    {
        _player = player;
        _anchor = anchor;
    }


    protected override void DoEnter()
    {
        _player.Respawn();
        _anchor.RespawnReset();
    }

    public override void Exit()
    {
        
    }

    public override bool Update(float deltaTime)
    {
        _nextState = States.WithAnchor;
        return true;
    }


}
