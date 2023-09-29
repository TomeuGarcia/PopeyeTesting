using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAxisMovementInput : IMovementInputHandler
{
    private Transform _cameraTransform;
    private InputSystem.PlayerInputControls _playerInputControls;


    public CameraAxisMovementInput(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;

        _playerInputControls = new InputSystem.PlayerInputControls();
        _playerInputControls.Enable();
    }

    ~CameraAxisMovementInput()
    {
        _playerInputControls.Disable();
    }


    public Vector3 GetMovementInput()
    {
        Vector2 movementInput = _playerInputControls.Land.Move.ReadValue<Vector2>();

        return ToCameraAlignedInput(movementInput);
    }

    public Vector3 GetLookInput()
    {
        Vector3 lookInput = _playerInputControls.Land.Look.ReadValue<Vector2>();

        return ToCameraAlignedInput(lookInput);
    }


    private Vector3 ToCameraAlignedInput(Vector2 input)
    {
        input = Vector2.ClampMagnitude(input, 1.0f);

        Vector3 result = Vector3.zero;
        Vector3 forward = Vector3.Cross(_cameraTransform.right, Vector3.up).normalized;

        result += _cameraTransform.right * input.x;
        result += forward * input.y;

        return result;
    }
}
