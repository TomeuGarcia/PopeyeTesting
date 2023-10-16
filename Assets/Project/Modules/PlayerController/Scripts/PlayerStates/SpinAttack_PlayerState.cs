using Cysharp.Threading.Tasks;
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


    public SpinAttack_PlayerState(Player player, Anchor anchor, ActionMovesetInputHandler movesetInputHandler, float withoutAnchorMoveSpeed)
    {
        _player = player;
        _anchor = anchor;
        _movesetInputHandler = movesetInputHandler;
        _withoutAnchorMoveSpeed = withoutAnchorMoveSpeed;
    }


    protected override void DoEnter()
    {
        _player.PlayerController.CanRotate = false;
        _player.PlayerController.LookTowardsPosition(_anchor.Position);
        _player.PlayerController.MaxSpeed = 1.0f;

        //_anchor.ParentToOwner();
        _anchor.SetStill();

        StartSpinning().Forget();
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

        if (_movesetInputHandler.IsPullAttack_Released())
        {
            _nextState = States.WithoutAnchor;
            return true;
        }

        return false;
    }



    public async UniTaskVoid StartSpinning()
    {
        float currentDistance = _anchor.CurrentDistanceFromOwner;
        float maxDistance = _anchor.MaxDistanceFromOwner;
        float minDistance = 1.0f;

        float t = Mathf.Min(currentDistance / maxDistance, 1.0f);

        float durationPerLoop = 1.0f;
        int numberOfLoops = 3;



        Vector3 lookDirection = Vector3.ProjectOnPlane((_anchor.Position - _player.Position).normalized, Vector3.up).normalized;
        float dot = Vector3.Dot(lookDirection, Vector3.forward);
        float angle = Mathf.Acos(dot);


        if (Vector3.Dot(lookDirection, Vector3.right) < 0)
        {
            angle *= -1;
        }

        float spinStart = angle * numberOfLoops;
        float spinEnd = spinStart + (Mathf.PI * 2 * numberOfLoops);
        


        
        while (t < 1.0f)
        {
            float step = _deltaTime / (numberOfLoops * durationPerLoop);
            t += step;

            float l = Mathf.Lerp(spinStart, spinEnd, t * t);
            float spinDistance = Mathf.Lerp(minDistance, maxDistance, t);

            Vector3 anchorPosition = Vector3.zero;
            anchorPosition += _player.Position;
            anchorPosition.x += Mathf.Sin(l) * spinDistance;
            anchorPosition.z += Mathf.Cos(l) * spinDistance;

            _anchor.Transform.position = anchorPosition;


            

            _player.PlayerController.LookTowardsPosition(_anchor.Position);

            await UniTask.Delay(MathUtilities.SecondsToMilliseconds(_deltaTime));
        }

    }


}
