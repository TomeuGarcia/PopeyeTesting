using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMovesetInputHandler
{

    private InputSystem.PlayerInputControls _playerInputControls;

    private UnityEngine.InputSystem.InputAction _aim;
    private UnityEngine.InputSystem.InputAction _throw;
    private UnityEngine.InputSystem.InputAction _grab;
    private UnityEngine.InputSystem.InputAction _meleeAttack;
    private UnityEngine.InputSystem.InputAction _move;
    private UnityEngine.InputSystem.InputAction _pullAttack;



    public ActionMovesetInputHandler()
    {
        _playerInputControls = new InputSystem.PlayerInputControls();
        _playerInputControls.Enable();

        _aim = _playerInputControls.Land.Aim;
        _throw = _playerInputControls.Land.Throw;
        _grab = _playerInputControls.Land.Grab;
        _meleeAttack = _playerInputControls.Land.MeleeAttack;
        _move = _playerInputControls.Land.Move;
        _pullAttack = _playerInputControls.Land.PullAttack;
    }

    ~ActionMovesetInputHandler()
    {
        _playerInputControls.Disable();
    }


    public bool IsAim_Pressed()
    {
        return _aim.WasPressedThisFrame();
    }
    public bool IsAim_HoldPressed()
    {
        return _aim.IsPressed();
    }
    public bool IsAim_Released()
    {
        return _aim.WasReleasedThisFrame();
    }
    

    public bool IsThrow_Pressed()
    {
        return _throw.WasPressedThisFrame();
    }
    public bool IsThrow_HoldPressed()
    {
        return _throw.IsPressed();
    }
    public bool IsThrow_Released()
    {
        return _throw.WasReleasedThisFrame();
    }
    

    public bool IsGrab_Pressed()
    {
        return _grab.WasPressedThisFrame();
    }
    public bool IsGrab_HoldPressed()
    {
        return _grab.IsPressed();
    }
    public bool IsGrab_Released()
    {
        return _grab.WasReleasedThisFrame();
    }


    public bool IsMeleeAttack_Pressed()
    {
        return _meleeAttack.WasPressedThisFrame();
    }


    public bool IsMove_HoldPressed()
    {
        return _move.IsPressed();
    }
    public bool IsMove_Released()
    {
        return _move.WasReleasedThisFrame();
    }


    public bool IsPullAttack_Pressed()
    {
        return _pullAttack.WasPressedThisFrame();
    }
    public bool IsPullAttack_HoldPressed()
    {
        return _pullAttack.IsPressed();
    }
    public bool IsPullAttack_Released()
    {
        return _pullAttack.WasReleasedThisFrame();
    }

}
