using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Threading;

public class EnemyDashingState : IEnemyState
{
    private Enemy _enemy;
    private float _dashPrepareDuration;
    private float _dashExecutionDuration;
    private float _dashRecoverDuration;
    private float _dashDistance;

    private float _defaultMaxMoveSpeed;

    Vector3 _dashStartPosition;
    Vector3 _dashEndPosition;

    bool _finishedDashing;

    private CancellationToken _dashCancellationToken;

    public EnemyDashingState(Enemy enemy, float dashPrepareDuration, float dashExecutionDuration, float dashRecoverDuration, 
        float dashDistance, float defaultMaxMoveSpeed)
    {
        _enemy = enemy;
        _dashPrepareDuration = dashPrepareDuration;
        _dashExecutionDuration = dashExecutionDuration;
        _dashRecoverDuration = dashRecoverDuration;
        _dashDistance = dashDistance;
        _defaultMaxMoveSpeed = defaultMaxMoveSpeed;
    }


    protected override void DoEnter()
    {
        _finishedDashing = false;
        _enemy.SetMaxMoveSpeed(0.0f);

        StartDashSequence();

        _dashCancellationToken = new CancellationToken();
    }

    public override void Exit()
    {
        _enemy.SetMaxMoveSpeed(_defaultMaxMoveSpeed);
        _enemy.SetCanRotate(true);        

        _enemy.DisableDealingContactDamage();
    }

    public override void Interrupt()
    {
        if (!_finishedDashing)
        {
            _enemy.transform.DOKill();
            _dashCancellationToken.ThrowIfCancellationRequested();
            _finishedDashing = true;
        }
    }

    public override bool Update(float deltaTime)
    {
        if (_finishedDashing)
        {
            _nextState = States.Chasing;
            return true;
        }

        return false;
    }


    private void ComputeDashStartPosition()
    {
        _dashStartPosition = _enemy.Position - (_enemy.LookDirection * _dashDistance / 4);
        _dashStartPosition = PositioningHelper.Instance.GetGoalPositionCheckingObstacles(_enemy.Position, _dashStartPosition, out float distanceRatio);
    }
    private void ComputeDashEndPosition()
    {
        _dashEndPosition = _enemy.Position + (_enemy.LookDirection * _dashDistance);
        _dashEndPosition = PositioningHelper.Instance.GetGoalPositionCheckingObstacles(_enemy.Position, _dashEndPosition, out float distanceRatio);
    }

    private async void StartDashSequence()
    {
        ComputeDashStartPosition();
        ComputeDashEndPosition();
        
        _enemy.SetCanRotate(false);
        _enemy.transform.DOMove(_dashStartPosition, _dashPrepareDuration)
            .SetEase(Ease.InOutQuart);
        await Task.Delay((int)(_dashPrepareDuration * 1000), _dashCancellationToken);

        if (_finishedDashing) return;
        _enemy.EnableDealingContactDamage();

        _enemy.transform.DOMove(_dashEndPosition, _dashExecutionDuration)
            .SetEase(Ease.OutQuart);
        await Task.Delay((int)(_dashExecutionDuration * 1000), _dashCancellationToken);

        if (_finishedDashing) return;

        await Task.Delay((int)(_dashRecoverDuration * 1000), _dashCancellationToken);
        
        
        _finishedDashing = true;
    }

}
