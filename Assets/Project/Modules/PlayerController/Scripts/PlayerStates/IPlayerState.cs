using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayerState
{
    public enum States
    {
        None,
        Spawn,
        WithAnchor,
        WithoutAnchor,
        AimingThrowAnchor,
        ThrowingAnchor,
        PlacedAnchorPullAttack,
        SpinAttack
    }

    protected States _nextState;
    public States NextState => _nextState;


    public void Enter()
    {
        _nextState = States.None;
        DoEnter();
    }

    protected abstract void DoEnter();
    public abstract void Exit();
    public abstract bool Update(float deltaTime);


    public virtual void OnDrawGizmos()
    {

    }

}
