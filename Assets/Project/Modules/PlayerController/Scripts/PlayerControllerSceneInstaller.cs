using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSceneInstaller : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _worldAlignedCameraTransform;
    [SerializeField] private Transform _isometricCameraTransform;
    [SerializeField] private bool _setPlayerCameraInputs = true;


    private void Awake()
    {
        _worldAlignedCameraTransform.gameObject.SetActive(!_setPlayerCameraInputs);
        _isometricCameraTransform.gameObject.SetActive(_setPlayerCameraInputs);

        if (_setPlayerCameraInputs)
        {
            _playerController.MovementInputHandler = new CameraAxisMovementInput(_isometricCameraTransform);
        }
    }

}
