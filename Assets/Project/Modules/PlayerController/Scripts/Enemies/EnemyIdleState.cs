using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private Enemy _enemy;
    private float _chaseStartDistance;

    private float _minIdleDuration;
    private float _idleTimer;

    public EnemyIdleState(Enemy enemy, float chaseStartDistance, float minIdleDuration)
    {
        _enemy = enemy;
        _chaseStartDistance = chaseStartDistance;
        _minIdleDuration = minIdleDuration;
    }


    protected override void DoEnter()
    {
        _idleTimer = 0.0f;
        _enemy.SetMaxMoveSpeed(0.0f);
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
        _idleTimer += deltaTime;
        if (_idleTimer < _minIdleDuration)
        {            
            return false;
        }

        if (Vector3.Distance(_enemy.TargetPosition, _enemy.Position) < _chaseStartDistance)
        {
            _nextState = States.Chasing;
            return true;
        }

        return false;
    }
}
