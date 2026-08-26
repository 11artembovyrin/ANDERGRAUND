using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovementState :IState
{
    public PlayerController player;

    public PlayerMovementState(PlayerController player)
    {
        this.player = player;
    }


    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Exit() { }
}



public class JumpState : PlayerMovementState
{
    public JumpState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        if (player.horizontalInput != 0)
            player.canWallJump = true;

        player.velocity.y = player.jumpForce;
        player.movementStateMachine.SwitchState(player.jumpingState);
        player.isGrounded = false;
    }
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() { }

}


public class WallJumpState : PlayerMovementState
{
    public WallJumpState(PlayerController player) : base(player) { }
    public override void Enter()
    {
        player.velocity.x = Mathf.Max(player.wallJumpForceX, Mathf.Abs(player.savedVelocityX) + 5, Mathf.Abs(player.velocity.y)) * -player.wallDirection;
        player.velocity.y = player.wallJumpForceY;

        player.movementStateMachine.SwitchState(player.jumpingState);
    }
    public override void Update() { }
    public override void FixedUpdate() { }
    public override void Exit() 
    {
        player.canWallJump = false;
        player.transform.rotation = Quaternion.identity;
    }

}



public class JumpingState : PlayerMovementState
{
    public JumpingState(PlayerController player) : base(player) { }
    public override void Enter() { }
    public override void Update() { }
    public override void FixedUpdate()
    {
        if (player.canWallJump)
            player.transform.Rotate(0, 0, 1500f * Time.deltaTime);

        if (player.spaÒePressed == false)
        {
            player.velocity.y *= 0.5f;
            player.movementStateMachine.SwitchState(player.fallingState);
        }
        if (player.velocity.y < 0)
        {
            player.movementStateMachine.SwitchState(player.fallingState);
        }
    }
    public override void Exit() { }

}


public class FallingState : PlayerMovementState
{
    public FallingState(PlayerController player) : base(player) { }
    public override void Enter() { }
    public override void Update() { }
    public override void FixedUpdate()
    {
        if (player.canWallJump)
            player.transform.Rotate(0, 0, 1500f * Time.deltaTime);

        if (player.canWallJump && player.isTouchingWall && player.spaÒePressed)
        {
            player.movementStateMachine.SwitchState(player.wallJumpState);
        }

        if (player.isGrounded)
        {
            player.movementStateMachine.SwitchState(player.groundState);
        }
    }
    public override void Exit() 
    { 
        player.canWallJump = false;
        player.transform.rotation = Quaternion.identity;
    }

}


public class GroundState : PlayerMovementState
{
    public GroundState(PlayerController player) : base(player) { }
    public override void Enter() 
    {
        player.acceleratingMultiplier = 2;
    }
    public override void Update() { }
    public override void FixedUpdate()
    {
        if (!player.isGrounded)
        {
            player.movementStateMachine.SwitchState(player.fallingState);
        }

        if (player.spaÒePressed)
        {
            player.movementStateMachine.SwitchState(player.jumpState);
        }

    }
    public override void Exit() 
    {
        player.acceleratingMultiplier = 1;
    }

}



public class HookingState : PlayerMovementState
{
    public HookingState(PlayerController player) : base(player) { }
    

    private Vector2 hookPos;
    private Vector2 forceDirection;
    private float hookVelocityX, hookVelocityY;
    private Vector2 startDirectionToHook, currentDirectionToHook;
    private Vector2 finalForse;
    private Vector2 startVelocity;

    private float hookAccelerationTimer;
    public override void Enter()
    {
        player.acceleratingMultiplier = 0;
        player.gravityMultiplier = 0;

        hookPos = player.hook.hookFinalPosition;
        startDirectionToHook = (hookPos - (Vector2)player.transform.position).normalized;
        startVelocity = player.velocity;

        hookAccelerationTimer = 0.05f;
    }

    public override void FixedUpdate()
    {

        if (hookAccelerationTimer > 0)
        {
            forceDirection = (hookPos - (Vector2)player.transform.position).normalized;
            forceDirection = (forceDirection + new Vector2 { x = player.horizontalInput * 0.2f, y = player.verticalInput * 0.2f }).normalized;
            hookVelocityX = player.hookForce * forceDirection.x;
            hookVelocityY = player.hookForce * forceDirection.y;

            hookVelocityX = Mathf.Max(Mathf.Abs(hookVelocityX), Mathf.Abs(startVelocity.x)) * Mathf.Sign(hookVelocityX);
            hookVelocityY = Mathf.Max(Mathf.Abs(hookVelocityY), Mathf.Abs(startVelocity.y)) * Mathf.Sign(hookVelocityY);

            finalForse = new Vector2 { x = hookVelocityX, y = hookVelocityY };
        }
        hookAccelerationTimer = Mathf.Max(0, hookAccelerationTimer - Time.fixedDeltaTime);


        player.velocity = Vector2.MoveTowards(player.velocity, finalForse, player.hookAcceleration * Time.fixedDeltaTime);


        currentDirectionToHook = (hookPos - (Vector2)player.transform.position).normalized;
        if (Vector2.Angle(startDirectionToHook, currentDirectionToHook) > 75)
            player.hook.StartHookReturning();


        if (player.hook.isHookAttached == false)
        {
            player.canWallJump = true;
            player.movementStateMachine.SwitchState(player.fallingState);
        }
    }
    public override void Exit()
    {
        player.acceleratingMultiplier = 1;
        player.gravityMultiplier = 1;
    }

}



// Õ≈ »—œŒÀ‹«”≈“—ﬂ
public class HookingToEnemyState : PlayerMovementState
{
    public HookingToEnemyState(PlayerController player) : base(player) { }

    public override void FixedUpdate()
    {
        player.health.cooldown = 0.2f;


        Vector2 direction = (player.hook.currentHook.transform.position - player.transform.position).normalized;
        player.velocity = direction * player.hookForce;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, player.hookForce * Time.fixedDeltaTime, player.enemyLayer);
        if (hit)
        {
            Vector2 directionToMouse = Utils.GetDirectionToMouse(player.transform.position);
            player.velocity = player.hookForce * directionToMouse * -1;
            player.canWallJump = true;
            player.hook.StartHookReturning();
            player.movementStateMachine.SwitchState(player.fallingState);
            return;
        }
    }
}



public class NothingState : PlayerMovementState
{
    public NothingState(PlayerController player) : base(player) { }
}





//public class HookingStateOld : State
//{
//    public HookThrow hook;

//    private Vector2 hookPos;
//    public HookingStateOld(PlayerMovement player, HookThrow hook) : base(player) 
//    { 
//        this.hook = hook;
//        hookPos = hook.hookFinalPosition; 
//    }


//    public override void Enter() 
//    {
//        player.acceleratingMultiplier = 0;
//    }
//    public override void FixedUpdate()
//    {

//        Vector2 forceDirection = (hookPos - new Vector2 { x = player.transform.position.x, y = player.transform.position.y }).normalized;
//        forceDirection = (forceDirection + new Vector2 { x = player.horizontalInput * 0.1f, y = player.verticalInput * 0.1f });

//        player.velocity += forceDirection * player.hookForce * Time.fixedDeltaTime;

//        player.velocity.x = Mathf.Clamp(player.velocity.x, -player.maxHookSpeed, player.maxHookSpeed);
//        player.velocity.y = Mathf.Clamp(player.velocity.y, -player.maxHookSpeed, player.maxHookSpeed);


//        if (hook.isHookAttached == false)
//        {
//            player.canWallJump = true;
//            player.stateMachine.SwitchState(new FallingState(player));
//        }
//    }
//    public override void Exit()
//    {
//        player.acceleratingMultiplier = 1;
//    }

//}

