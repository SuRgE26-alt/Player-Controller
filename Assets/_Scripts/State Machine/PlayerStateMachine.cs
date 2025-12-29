using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    //declare reference variables
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _playerAnimator;

    //variables to store player input values 
    Vector2 _currentMovementInput;

    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    Vector3 _appliedMovement;

    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpPressed = false;
    bool _isJumping = false;
    bool _isJumpAnimating;

    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;

    float _onGroundGravity = -0.5f;
    float _gravity = -9.0f;
    float _rotationFactorPerFrame = 15f;
    float _runMultiplier = 3.0f;
    float _initialJumpVelocity;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = 0.75f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        Debug.Log(_isJumpPressed);
    }
}
