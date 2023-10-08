using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingState : IEnemyState
{
    private Enemy _enemy;
    private float _loseInterestDistance;
    private float _attackStartDistance;

    private float _timeLastDash;
    private float _dashCooldownTime;


    public EnemyChasingState(Enemy enemy, float loseInterestDistance, float attackStartDistance)
    {
        _enemy = enemy;
        _loseInterestDistance = loseInterestDistance;
        _attackStartDistance = attackStartDistance;
        _timeLastDash = 0.0f;
        _dashCooldownTime = 2.0f;
    }


    protected override void DoEnter()
    {
        
    }

    public override void Exit()
    {
        _enemy.SetMaxMoveSpeed(_enemy.MaxMoveSpeed);
    }
    public override void Interrupt()
    {
        
    }

    public override bool Update(float deltaTime)
    {
        float distanceFromTarget = Vector3.Distance(_enemy.TargetPosition, _enemy.Position);

        if (distanceFromTarget < _attackStartDistance && _enemy.IsTargetOnReachableHeight())
        {
            if (DashCooldownFinished())
            {
                _timeLastDash = Time.time;
                _nextState = States.Dashing;
                return true;
            }
            else
            {
                _enemy.SetMaxMoveSpeed(1.0f);
            }
        }

        if (distanceFromTarget > _loseInterestDistance)
        {
            _nextState = States.Idle;
            return true;
        }


        return false;
    }

    private bool DashCooldownFinished()
    {
        return Time.time > _timeLastDash + _dashCooldownTime;
    }
}
