using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    #region Variables
    //declare reference variables
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _playerAnimator;

    //variables to store player input values 
    Vector2 _currentMovementInput;
    Vector2 _currentLookInput;

    //Look variables
    

    //Movement variables
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;

    //Run variables
    Vector3 _currentRunMovement;
    bool _isRunPressed;
    float _runMultiplier = 3.0f;

    //Jump variables
    bool _isJumpPressed = false;
    bool _isJumping = false;
    bool _requireNewJumpPress = false;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = 0.75f;
    float _initialJumpVelocity;

    //Animation hashes
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;

    //Gravity variables 
    float _groundedGravity = -0.5f;
    float _gravity = -9.0f;
    float _rotationFactorPerFrame = 15f;

    //state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    #endregion

    #region Getters and Setters
    //getters and setters
    public PlayerBaseState CurrentState {  get { return _currentState; } set { _currentState = value; }}
    public CharacterController CharacterController { get { return _characterController; }}
    public Animator PlayerAnimator { get { return _playerAnimator; }}
    public int IsJumpingHash { get { return _isJumpingHash; }}
    public int IsWalkingHash {  get { return _isWalkingHash; }}
    public int IsRunningHash {  get { return _isRunningHash; }}
    public bool IsMovementPressed { get { return _isMovementPressed; }}
    public bool IsRunPressed {  get { return _isRunPressed; }}
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; }}
    public bool IsJumping {  set { _isJumping = value; }}
    public bool IsJumpPressed { get { return _isJumpPressed; }}
    public float Gravity {  get { return _gravity; }}
    public float GroundedGravity { get { return _groundedGravity; }}
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; }}
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; }}
    public float RunMultiplier { get { return _runMultiplier; }}
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; }}

    #endregion

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();

        //Setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        //Converting animation string to hash 
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");

        //Setting up Movement input
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;

        _playerInput.CharacterControls.Look.started += OnLook;
        _playerInput.CharacterControls.Look.performed += OnLook;
        _playerInput.CharacterControls.Look.canceled += OnLook;

        //Setting up run input
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;

        //Setting up jump input
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

    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnLook(InputAction.CallbackContext context)
    {
        _currentLookInput = context.ReadValue<Vector2>();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
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
}
