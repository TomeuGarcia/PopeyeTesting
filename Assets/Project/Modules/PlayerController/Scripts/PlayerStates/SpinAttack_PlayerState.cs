using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

        float duration = 2.0f;
        float timer = 0.0f;

        while (!_finishedPerformingSpinAttack && timer < duration)
        {
            timer += _deltaTime;
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


        Vector3 floorNormal = Vector3.up;
        if (Physics.Raycast(_player.Position, Vector3.down, out RaycastHit hit, 10.0f, _obstaclesLayerMask, QueryTriggerInteraction.Ignore))
        {
            floorNormal = hit.normal;
        }



        Vector3 rightDirection = Vector3.ProjectOnPlane(Vector3.right, floorNormal).normalized;
        Vector3 forwardDirection = Vector3.ProjectOnPlane(Vector3.forward, floorNormal).normalized;

        Vector3 lookDirection = Vector3.ProjectOnPlane((_anchor.Position - _player.Position).normalized, floorNormal).normalized;
        float dot = Vector3.Dot(lookDirection, forwardDirection);

        float angleOffsetFromOrigin = Mathf.Acos(dot);
        if (Vector3.Dot(lookDirection, rightDirection) < 0)
        {
            angleOffsetFromOrigin *= -1;
        }


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

            float step = _deltaTime / (numberOfLoops * durationPerLoop);
            t += step;

            await UniTask.Delay(MathUtilities.SecondsToMilliseconds(_deltaTime));
        }

        // TODO try do slowdown


        _anchor.SpinAttackFinish();        

        await UniTask.WaitUntil(() => !_anchor.IsOnAir());
        _finishedPerformingSpinAttack = true;
    }


    public bool AnchorCollidedWithObstacle()
    {
        if (Physics.CheckSphere(_anchor.Position, 0.15f, _obstaclesLayerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }


        return false;
    }


}
