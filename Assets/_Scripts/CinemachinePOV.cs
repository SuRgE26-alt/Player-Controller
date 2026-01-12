//using Unity.Cinemachine;
//using UnityEngine;

//public class CinemachinePOV : CinemachineExtension
//{
//    [SerializeField] private float horizontalMouseSpeed = 10f;
//    [SerializeField] private float verticalMouseSpeed = 10f;
//    [SerializeField] private float clampAngle = 80f;

//    PlayerStateMachine _playerStateMachine;

//    Vector3 startingRotation;

//    protected override void Awake()
//    {
//        base.Awake();

//        startingRotation = transform.localRotation.eulerAngles;
//    }

//    void Start()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//    }

//    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
//    {

//        if (stage != CinemachineCore.Stage.Aim)
//            return;

//        if (vcam.Follow == null)
//            return;

//        if (_playerStateMachine == null)
//            _playerStateMachine = vcam.Follow.GetComponent<PlayerStateMachine>();

//        if (_playerStateMachine == null)
//            return;

//        Vector2 deltaInput = _playerStateMachine.LookInput;

//        startingRotation.x += deltaInput.x * verticalMouseSpeed * deltaTime;
//        startingRotation.y += deltaInput.y * horizontalMouseSpeed * deltaTime;
//        startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);

//        state.RawOrientation = Quaternion.Euler(
//            startingRotation.y,
//            startingRotation.x,
//            0f
//        );
//    }
//}
