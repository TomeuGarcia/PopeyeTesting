using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Anchor _anchor;
    [SerializeField] private float _withAnchorMoveSpeed;
    [SerializeField] private float _withoutAnchorMoveSpeed;
    [SerializeField] private float _aimingMoveSpeed;
    [SerializeField] private float _pullingMoveSpeed;
    [SerializeField] private float _pullAttackMoveSpeed;
    [SerializeField, Range(0.0f, 10.0f)] private float _maxAimDuration = 1.0f;
    [SerializeField] private LayerMask _obstaclesLayerMask;

    private ActionMovesetInputHandler _movesetInputHandler;

    private Dictionary<IPlayerState.States, IPlayerState> _states;
    private IPlayerState _currentState;



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _movesetInputHandler = new ActionMovesetInputHandler();
        _player.AwakeInit(_movesetInputHandler);

        Spawn_PlayerState spawnState = new Spawn_PlayerState(_player, _anchor);
        WithAnchorState withAnchorState = new WithAnchorState(_player, _anchor, _withAnchorMoveSpeed, _movesetInputHandler);
        WithoutAnchorState withoutAnchorState = new WithoutAnchorState(_player, _anchor, _withoutAnchorMoveSpeed, _pullingMoveSpeed, _movesetInputHandler);
        AimingThrowAnchorState aimingThrowAnchorState = new AimingThrowAnchorState(_player, _anchor, _aimingMoveSpeed, _movesetInputHandler, _maxAimDuration);
        ThrowingAnchorState throwingAnchorState = new ThrowingAnchorState(_player, _anchor, _withoutAnchorMoveSpeed, _movesetInputHandler);
        PlacedAnchorPullAttackState placedAnchorPullAttackState = new PlacedAnchorPullAttackState(_player, _anchor, _pullAttackMoveSpeed, _movesetInputHandler);

        SpinAttack_PlayerState spinAttackState = new SpinAttack_PlayerState(_player, _anchor, _movesetInputHandler, _withoutAnchorMoveSpeed, _obstaclesLayerMask);


        _states = new Dictionary<IPlayerState.States, IPlayerState>()
        {
            { IPlayerState.States.Spawn, spawnState },
            { IPlayerState.States.WithAnchor, withAnchorState },
            { IPlayerState.States.WithoutAnchor, withoutAnchorState },
            { IPlayerState.States.AimingThrowAnchor, aimingThrowAnchorState },
            { IPlayerState.States.ThrowingAnchor, throwingAnchorState },
            { IPlayerState.States.PlacedAnchorPullAttack, placedAnchorPullAttackState },
            { IPlayerState.States.SpinAttack, spinAttackState }
        };

        _currentState = _states[IPlayerState.States.WithAnchor];
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


        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStates();
        }

    }

    private void OnDrawGizmos()
    {
        _currentState?.OnDrawGizmos();
    }


    private void ResetStates()
    {
        _currentState.Exit();
        _currentState = _states[IPlayerState.States.Spawn];
        _currentState.Enter();
    }


}
