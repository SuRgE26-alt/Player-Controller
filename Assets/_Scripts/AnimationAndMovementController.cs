using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
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

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");

        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;

        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;

        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _currentRunMovement.x = _currentMovementInput.x * _runMultiplier;
        _currentRunMovement.z = _currentMovementInput.y * _runMultiplier;
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

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    void Update()
    {
        HandleRotation();
        HandleAnimations();

        if(_isRunPressed)
        {
            _appliedMovement.x = _currentRunMovement.x;
            _appliedMovement.z = _currentRunMovement.z;
        }
        else
        {
            _appliedMovement.x = _currentMovement.x;
            _appliedMovement.z = _currentMovement.z;
        }

        _characterController.Move(_appliedMovement * Time.deltaTime);

        HandleGravity();
        HandleJumps();
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if(_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleJumps()
    {
        if(!_isJumping && _characterController.isGrounded && _isJumpPressed)
        {
            _playerAnimator.SetBool(_isJumpingHash, true);
            _isJumpAnimating = true;
            _isJumping = true;
            _currentMovement.y = _initialJumpVelocity;
            _appliedMovement.y = _initialJumpVelocity;
        }
        else if (!_isJumpPressed && _isJumping && _characterController.isGrounded)
        {
            _isJumping = false;
        }
    }

    void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed; // you can remove the "|| !isJumpPressed" if you do not want variable jump height 
        float fallMultiplier = 2.0f;

        if(_characterController.isGrounded)
        {
            if(_isJumpAnimating)
            {
                _playerAnimator.SetBool(_isJumpingHash, false);
                _isJumpAnimating = false;
            }
            _currentMovement.y = _onGroundGravity;
            _currentRunMovement.y = _onGroundGravity;
        }
        else if(isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_gravity * fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float previousVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_gravity * Time.deltaTime);
            _appliedMovement.y = (previousVelocity + _currentMovement.y) * 0.5f;
        }
    }

    void HandleAnimations()
    {
        bool isWalking = _playerAnimator.GetBool(_isWalkingHash);
        bool isRunning = _playerAnimator.GetBool(_isRunningHash);

        if(_isMovementPressed && !isWalking)
        {
            _playerAnimator.SetBool(_isWalkingHash, true);
        }
        else if(!_isMovementPressed && isWalking)
        {
            _playerAnimator.SetBool(_isWalkingHash, false);
        }

        if((_isMovementPressed && _isRunPressed) && !isRunning)
        {
            _playerAnimator.SetBool(_isRunningHash, true);
        }
        else if((!_isMovementPressed || !_isRunPressed) && isRunning)
        {
            _playerAnimator.SetBool(_isRunningHash, false);
        }
    }
    
}
