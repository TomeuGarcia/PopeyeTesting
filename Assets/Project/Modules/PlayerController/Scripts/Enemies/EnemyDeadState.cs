using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : IEnemyState
{
    private Enemy _enemy;

    public EnemyDeadState(Enemy enemy)
    {
        _enemy = enemy;
    }


    protected override void DoEnter()
    {
        _enemy.StartDeathAnimation();
    }

    public override void Exit()
    {
        
    }
    
    public override void Interrupt()
    {
        
    }

    public override bool Update(float deltaTime)
    {
        return false;
    }


}
