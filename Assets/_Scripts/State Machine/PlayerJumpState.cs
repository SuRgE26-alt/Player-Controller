using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) 
    {
        IsRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchState()
    {
        if(Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        HandleJumps();
    }

    public override void ExitState()
    {
        Ctx.PlayerAnimator.SetBool(Ctx.IsJumpingHash, false);
        
        if(Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        HandleGravity();
    }

    void HandleJumps()
    {
        Ctx.PlayerAnimator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.IsJumping = true;
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocity;
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocity;

    }

    void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed; //you can remove the "|| !isJumpPressed" if you do not want variable jump height 
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            float previousVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.Gravity * Time.deltaTime);
            Ctx.AppliedMovementY = (previousVelocity + Ctx.CurrentMovementY) * 0.5f;
        }
    }
}
