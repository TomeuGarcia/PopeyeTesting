using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : IEnemyStateMachine
{
    private Enemy _enemy;

    [Header("DISTANCES")]
    [SerializeField, Range(0.0f, 20.0f)] private float _chaseStartDistance = 8.0f;
    [SerializeField, Range(0.0f, 20.0f)] private float _loseInterestDistance = 7.0f;
    [SerializeField, Range(0.0f, 20.0f)] private float _attackStartDistance = 2.0f;
    [SerializeField, Range(0.0f, 20.0f)] private float _dashDistance = 4.0f;

    [Header("DURATIONS")]
    [SerializeField, Range(0.0f, 10.0f)] private float _minIdleDuration = 0.5f;
    [SerializeField, Range(0.0f, 10.0f)] private float _dashPrepareDuration = 1.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float _dashExecutionDuration = 0.5f;
    [SerializeField, Range(0.0f, 10.0f)] private float _dashRecoverDuration = 2.0f;

    private Dictionary<IEnemyState.States, IEnemyState> _states;
    private IEnemyState _currentState;



    public override void AwakeInit(Enemy enemy)
    {
        _enemy = enemy;
        Init();
    }

    private void OnDestroy()
    {
        _currentState.Exit();
    }

    private void Init()
    {
        EnemyIdleState idleState = new EnemyIdleState(_enemy, _chaseStartDistance, _minIdleDuration);
        EnemyChasingState chasingState = new EnemyChasingState(_enemy, _loseInterestDistance, _attackStartDistance);
        EnemyDashingState dashingState = new EnemyDashingState(_enemy, _dashPrepareDuration, _dashExecutionDuration, _dashRecoverDuration, _dashDistance, 
            _enemy.MaxMoveSpeed);
        EnemyDeadState deadState = new EnemyDeadState(_enemy);


        _states = new Dictionary<IEnemyState.States, IEnemyState>()
        {
            { IEnemyState.States.Idle, idleState },
            { IEnemyState.States.Dead, deadState },
            { IEnemyState.States.Chasing, chasingState },
            { IEnemyState.States.Dashing, dashingState }
        };

        _currentState = _states[IEnemyState.States.Idle];
        _currentState.Enter();
    }


    private void Update()
    {
        if (_currentState.Update(Time.deltaTime))
        {
            _currentState.Exit();
            _currentState = _states[_currentState.NextState];
            _currentState.Enter();
        }
    }


    public override void ResetStateMachine()
    {        
        OverwriteCurrentState(IEnemyState.States.Idle);
    }

    public override void OverwriteCurrentState(IEnemyState.States newState)
    {
        _currentState.Interrupt();
        _currentState.Exit();
        _currentState = _states[newState];
        _currentState.Enter();
    }

}
