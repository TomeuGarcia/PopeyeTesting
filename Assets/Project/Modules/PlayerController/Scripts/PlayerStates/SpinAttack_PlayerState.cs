using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpinAttack_PlayerState : IPlayerState
{
    private Player _player;
    private Anchor _anchor;
    private ActionMovesetInputHandler _movesetInputHandler;

    private float _withoutAnchorMoveSpeed;
    private float _deltaTime;

    private LayerMask _obstaclesLayerMask;

    private bool _finishedPerformingSpinAttack;
    private bool _finishedSpinAttackDuration;


    public SpinAttack_PlayerState(Player player, Anchor anchor, ActionMovesetInputHandler movesetInputHandler, float withoutAnchorMoveSpeed,
        LayerMask obstaclesLayerMask)
    {
        _player = player;
        _anchor = anchor;
        _movesetInputHandler = movesetInputHandler;
        _withoutAnchorMoveSpeed = withoutAnchorMoveSpeed;
        _obstaclesLayerMask = obstaclesLayerMask;
    }


    protected override void DoEnter()
    {
        _player.PlayerController.CanRotate = false;
        _player.PlayerController.LookTowardsPosition(_anchor.Position);
        _player.PlayerController.MaxSpeed = 1.0f;

        //_anchor.ParentToOwner();
        _anchor.SetStill();

        if (AnchorCollidedWithObstacle())
        {
            _finishedPerformingSpinAttack = true;
        }
        else
        {
            _anchor.UnparentFromOwner();

            _finishedPerformingSpinAttack = false;
            SpinAttackDuration().Forget();
            StartSpinning().Forget();
        }
    }

    public override void Exit()
    {
        _player.PlayerController.CanRotate = true;
        _player.PlayerController.MaxSpeed = _withoutAnchorMoveSpeed;

        //_anchor.UnparentFromOwner();
    }

    public override bool Update(float deltaTime)
    { 
        _deltaTime = deltaTime;

        if (_finishedPerformingSpinAttack)
        {
            _nextState = States.WithoutAnchor;
            return true;
        }

        return false;
    }


    private async UniTaskVoid SpinAttackDuration()
    {
        _finishedSpinAttackDuration = false;

        Timer durationTimer = new Timer(2.0f);

        while (!_finishedPerformingSpinAttack && !durationTimer.HasFinished())
        {
            durationTimer.Update(_deltaTime);
            await UniTask.Delay(MathUtilities.SecondsToMilliseconds(_deltaTime));
        }


        if (!_finishedPerformingSpinAttack)
        {
            _finishedSpinAttackDuration = true;
        }        
    }



    private async UniTaskVoid StartSpinning()
    {
        _anchor.ChangeState(Anchor.AnchorStates.OnAir);

        float currentDistance = _anchor.CurrentDistanceFromOwner;
        float maxDistance = _anchor.MaxDistanceFromOwner - 1.0f;
        float minDistance = 0.5f;

        currentDistance = Mathf.Max(currentDistance, minDistance);
        float t = Mathf.Min((currentDistance + minDistance) / maxDistance, 1.0f);

        float durationPerLoop = 0.6f;
        int numberOfLoops = 4;


        Vector3 floorNormal = _player.PlayerController.ContactNormal;


        Vector3 rightDirection = Vector3.ProjectOnPlane(Vector3.right, floorNormal).normalized;
        Vector3 forwardDirection = Vector3.ProjectOnPlane(Vector3.forward, floorNormal).normalized;

        Vector3 lookDirection = Vector3.ProjectOnPlane((_anchor.Position - _player.Position).normalized, floorNormal).normalized;
        float dot = Vector3.Dot(lookDirection, forwardDirection);

        float angleOffsetFromOrigin = Mathf.Acos(dot);
        if (Vector3.Dot(lookDirection, rightDirection) < 0)
        {
            angleOffsetFromOrigin *= -1;
        }

        float spinStepSpeed = 1.0f / (numberOfLoops * durationPerLoop);

        float totalAngleLoops = Mathf.PI * 2 * numberOfLoops;
        float tAngle = t * totalAngleLoops;

        float spinStart = angleOffsetFromOrigin - tAngle;
        float spinEnd = spinStart + totalAngleLoops;

        
        //while (t < 1.0f)
        while (_movesetInputHandler.IsPullAttack_HoldPressed() && !_finishedSpinAttackDuration && !AnchorCollidedWithObstacle())
        {
            float spinAngle = spinStart + ((spinEnd - spinStart)* t);
            float spinDistance = Mathf.Lerp(minDistance, maxDistance, t);


            Vector3 anchorPosition = Vector3.zero;
            anchorPosition += _player.Position;
            anchorPosition += rightDirection * Mathf.Sin(spinAngle) * spinDistance;
            anchorPosition += forwardDirection * Mathf.Cos(spinAngle) * spinDistance;

            _anchor.Transform.position = anchorPosition;
            _player.PlayerController.LookTowardsPosition(_anchor.Position);

            _anchor.SetMeshRotation(Quaternion.LookRotation(_player.PlayerController.LookDirection, floorNormal));

            float step = _deltaTime * spinStepSpeed;
            t += step;

            /*
            _spinTangent = Vector3.zero;
            _spinTangent += rightDirection * Mathf.Cos(spinAngle);
            _spinTangent += forwardDirection * -Mathf.Sin(spinAngle);
            */

            await UniTask.Delay(MathUtilities.SecondsToMilliseconds(_deltaTime));
        }

        
        if (_finishedSpinAttackDuration)
        {
            await SpinSlowDown(t, 0.5f, spinStart, spinEnd, maxDistance, floorNormal, rightDirection, forwardDirection, spinStepSpeed);
            _anchor.SpinAttackFinishTired();
        }
        else
        {
            _anchor.SpinAttackFinish();
        }
        

        await UniTask.WaitUntil(() => !_anchor.IsOnAir());
        _finishedPerformingSpinAttack = true;
    }


    private bool AnchorCollidedWithObstacle()
    {
        if (Physics.CheckSphere(_anchor.Position, 0.15f, _obstaclesLayerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }


        return false;
    }


    private async UniTask SpinSlowDown(float spinT, float duration, float spinStart, float spinEnd, float maxDistance, 
        Vector3 floorNormal, Vector3 rightDirection, Vector3 forwardDirection, float spinStepSpeed)
    {
        Timer slowdownTimer = new Timer(duration);

        while (!slowdownTimer.HasFinished())
        {
            float spinAngle = spinStart + ((spinEnd - spinStart) * spinT);
            float spinDistance = maxDistance;




            Vector3 anchorPosition = Vector3.zero;
            anchorPosition += _player.Position;
            anchorPosition += rightDirection * Mathf.Sin(spinAngle) * spinDistance;
            anchorPosition += forwardDirection * Mathf.Cos(spinAngle) * spinDistance;
            anchorPosition += Vector3.down * (0.5f * slowdownTimer.GetCounterRatio01());

            _anchor.Transform.position = anchorPosition;
            _player.PlayerController.LookTowardsPosition(_anchor.Position);

            _anchor.SetMeshRotation(Quaternion.LookRotation(_player.PlayerController.LookDirection, floorNormal));

            float step = _deltaTime * spinStepSpeed * (1 - slowdownTimer.GetCounterRatio01());
            spinT += step;

            slowdownTimer.Update(_deltaTime);
            await UniTask.Delay(MathUtilities.SecondsToMilliseconds(_deltaTime));
        }

        
    }

}
