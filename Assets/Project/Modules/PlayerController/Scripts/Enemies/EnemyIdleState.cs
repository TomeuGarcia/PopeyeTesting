using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private Enemy _enemy;
    private float _chaseStartDistance;


    public EnemyIdleState(Enemy enemy, float chaseStartDistance)
    {
        _enemy = enemy;
        _chaseStartDistance = chaseStartDistance;
    }


    protected override void DoEnter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override bool Update(float deltaTime)
    {
        if (_enemy.IsDead())
        {
            _nextState = States.Dead;
            return true;
        }

        if (Vector3.Distance(_enemy.TargetPosition, _enemy.Position) < _chaseStartDistance)
        {
            _nextState = States.Chasing;
            return true;
        }

        return false;
    }
}
