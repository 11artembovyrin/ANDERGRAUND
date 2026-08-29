using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject swordPrefab;
    public GameObject playerSprite;
    public GameObject bulletPrefab;

    [Header("MovementSettings")]
    public float walkAcceleration = 80f;
    public float maxWalkSpeed = 30f;
    public float gravity = 60f;
    public float maxFallSpeed = 50f;
    public float skinWidth = 0.015f;


    [Header("Sword")]
    public float swordHitForce = 35f;
    public float swordCooldown = 0.5f;
    public float swordBaseDamage = 0.5f;
    public float swordVelocityMultiplierDamage = 0.01f;
    public float swordVelocityMultiplierSize = 0.01f;
    [HideInInspector] public float swordTimer = 0f;

    [Header("Shotgun")]
    public int bulletNumber = 3;
    public float angleOffset = 10f;
    public float recoilForce = 30f;
    public float ammoMultiplier = 1f;
    public float maxAmmo = 3f;
    [HideInInspector] public float currentAmmo = 0f;

    [Header("Jumping")]
    public float jumpForce = 40f;
    public float wallJumpForceY = 40f;
    public float wallJumpForceX = 30f;

    [Header("Hook")]
    public float hookForce = 100f;
    public float hookAcceleration = 20f;
    public float maxHookSpeed = 100f;

    [Header("Dash")]
    public float dashForce = 50f;
    public float dashDeacceletareForce = 20f;
    public float dashReloadTime = 2f;
    private float dashReloadTimer = 0;
    private bool dashDeacceleratingEnabled = false;

    [Header("Collision")]
    public Vector2 boxSize;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;

    [Header("Settings")]
    [SerializeField] private bool teleportOnSpawn = false;


    [HideInInspector] public Vector2 velocity = Vector2.zero;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isTouchingWall;
    [HideInInspector] public float wallDirection;
    [HideInInspector] public bool canWallJump = false;
    [HideInInspector] public float savedVelocityX;
    [HideInInspector] public Vector2 interpolatedPosition;
    [HideInInspector] public float playerDirection = 1;

    private BoxCollider2D col;
    private float halfWidth, halfHeight;


    // STATE MACHINE
    [HideInInspector] public StateMachine movementStateMachine = new StateMachine();
    [HideInInspector] public StateMachine actionStateMachine = new StateMachine();

    // INPUT
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public bool spañePressed;
    [HideInInspector] public bool shiftPressed;
    

    // OTHER
    [HideInInspector] public float acceleratingMultiplier = 1f;
    [HideInInspector] public float gravityMultiplier = 1f;
    [HideInInspector] public Vector2 futurePosition;


    // SCRIPTS
    [HideInInspector] public HookThrow hook;
    [HideInInspector] public Health health;


    // STATES
    // Movement states
    public JumpState jumpState {  get; private set; }
    public WallJumpState wallJumpState { get; private set; }
    public JumpingState jumpingState { get; private set; }
    public FallingState fallingState { get; private set; }
    public GroundState groundState { get; private set; }
    public HookingState hookingState { get; private set; }
    public HookingToEnemyState hookingToEnemyState { get; private set; }
    public NothingState nothingState { get; private set; }
    

    // Action states
    public IdleState idleState { get; private set; }
    public SwordAttackingState swordAttackingState { get; private set; }
    public ShotgunShootingState shotgunShootingState { get; private set; }


    void Start()
    {
        if (teleportOnSpawn)
        {
            transform.position = SpawnSettings.spawnPosition;
        }


        hook = GetComponent<HookThrow>();

        // Movement states
        jumpState = new JumpState(this);
        wallJumpState = new WallJumpState(this);
        jumpingState = new JumpingState(this);
        fallingState = new FallingState(this);
        groundState = new GroundState(this);
        hookingState = new HookingState(this);
        hookingToEnemyState = new HookingToEnemyState(this);
        nothingState = new NothingState(this);

        // Action states
        idleState = new IdleState(this);
        swordAttackingState = new SwordAttackingState(this);
        shotgunShootingState = new ShotgunShootingState(this);



        health = GetComponent<Health>();
        hook = GetComponent<HookThrow>();

        col = GetComponent<BoxCollider2D>();
        halfWidth = boxSize.x / 2f - skinWidth; 
        halfHeight = boxSize.y / 2f - skinWidth;

        movementStateMachine = new StateMachine();
        movementStateMachine.SwitchState(groundState);

        actionStateMachine = new StateMachine();
        actionStateMachine.SwitchState(idleState);
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        spañePressed = Input.GetKey(KeyCode.Space);
        shiftPressed = Input.GetKey(KeyCode.LeftShift);
        

        movementStateMachine.Update();
        actionStateMachine.Update();

    }

    void FixedUpdate()
    {
        if (movementStateMachine.GetCurrentState() == nothingState) return;


        movementStateMachine.FixedUpdate();
        actionStateMachine.FixedUpdate();

        ApplyGravity();

        HorizontalMovement();

        HandleDash();

        MoveCharacter(velocity * Time.fixedDeltaTime);
        


        MakeCooldown();

    
    }

    private void LateUpdate()
    {
        interpolatedPosition = playerSprite.transform.position;
    }

    void ApplyGravity()
    {
        if (velocity.y > -maxFallSpeed)
            velocity.y -= gravity * gravityMultiplier * Time.fixedDeltaTime;
    }

    void MoveCharacter(Vector2 delta)
    {
        isGrounded = false;


        Vector2 pos = transform.position;

        if (Mathf.Abs(delta.x) > 0.0001f)
        {
            Vector2 originX = pos + Vector2.right * Mathf.Sign(delta.x) * skinWidth * 0.5f;
            RaycastHit2D hitX = Physics2D.BoxCast(
                originX,
                boxSize - new Vector2(2 * skinWidth, 0.1f),
                0,
                Vector2.right * Mathf.Sign(delta.x),
                Mathf.Abs(delta.x) + skinWidth,
                groundLayer
            );

            if (hitX)
            {
                float playerBottom = pos.y - halfHeight;
                bool isWall = hitX.point.y > playerBottom + 0.05f;

                if (isWall)
                {
                    savedVelocityX = Mathf.Max(Mathf.Abs(velocity.x), savedVelocityX);

                    if (!isTouchingWall) timeSaveVelocityX = maxTimeSaveVelocityX;


                    velocity.x = 0;
                    pos.x = hitX.point.x - ((halfWidth + skinWidth) * Mathf.Sign(delta.x));
                    isTouchingWall = true;
                    wallDirection = Mathf.Sign(delta.x);
                }
                else
                {
                    pos.x += delta.x;
                }
            }
            else
            {
                pos.x += delta.x;
            }
        }

        if (isTouchingWall)
        {
            Vector2 circleOrigin = new Vector2(
                pos.x + wallDirection * (halfWidth + skinWidth),
                pos.y
            );

            RaycastHit2D wallHitX = Physics2D.CircleCast(
                circleOrigin,
                0.3f, // Ðàäèóñ êðóãà
                Vector2.right * wallDirection,
                0.2f,
                groundLayer
            );

            isTouchingWall = wallHitX.collider != null;
        }


        // ÄÂÈÆÅÍÈÅ ÏÎ Y - ÈÑÏÐÀÂËÅÍÎ
        if (Mathf.Abs(delta.y) > 0.0001f)
        {
            Vector2 boxSizeY = boxSize - new Vector2(0.1f, 2 * skinWidth);

            RaycastHit2D hitY = Physics2D.BoxCast(
                pos,
                boxSizeY,
                0,
                Vector2.up * Mathf.Sign(delta.y),
                Mathf.Abs(delta.y) + skinWidth,
                groundLayer
            );

            if (hitY)
            {

                bool isFloor = (delta.y < 0 && hitY.normal.y > 0.9f);
                bool isCeiling = (delta.y > 0 && hitY.normal.y < -0.9f);

                if (isFloor || isCeiling)
                {
                    velocity.y = 0;
                    pos.y = hitY.point.y - (halfHeight * Mathf.Sign(delta.y));

                    if (isFloor)
                    {
                        isGrounded = true;
                    }
                }
                else
                {
                    pos.y += delta.y;
                }
            }
            else
            {
                pos.y += delta.y;
            }
        }

        transform.position = pos;
    }

    void HorizontalMovement()
    {
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            if (Mathf.Abs(velocity.x) < maxWalkSpeed ||
                Mathf.Sign(velocity.x) != Mathf.Sign(horizontalInput))
            {
                velocity.x += horizontalInput * walkAcceleration * Time.fixedDeltaTime * acceleratingMultiplier;
            }
        }
        else
        {
            float deceleration = walkAcceleration * Time.fixedDeltaTime * acceleratingMultiplier * 0.5f;
            if (Mathf.Abs(velocity.x) <= deceleration)
            {
                velocity.x = 0;
            }
            else
            {
                velocity.x -= Mathf.Sign(velocity.x) * deceleration;
            }
        }

        if (horizontalInput != 0) playerDirection = horizontalInput;

        // dash deaccelerating

        //float velocityToDeaccelerate = 0;
        //float deacceleratingDirecton = 0;
        //if (dashDeacceleratingEnabled)
        //{
        //    velocityToDeaccelerate = dashForce;
        //    deacceleratingDirecton = -playerDirection;

        //    dashDeacceleratingEnabled = false;
        //}

        //if (velocityToDeaccelerate != 0)
        //{
        //    velocityToDeaccelerate -= dashDeacceletareForce * Time.fixedDeltaTime;
        //    velocity.x += dashDeacceletareForce * deacceleratingDirecton * Time.fixedDeltaTime;

        //    if (Mathf.Sign(velocity.x) == deacceleratingDirecton) velocityToDeaccelerate = 0;
        //}
    }



    private float dashDirection = 0, velocityToDeaccelerate = 0;
    void HandleDash()
    {
        if (dashDeacceleratingEnabled)
        {
            velocityToDeaccelerate -= dashDeacceletareForce * Time.fixedDeltaTime;
            velocity.x += dashDeacceletareForce * -dashDirection * Time.fixedDeltaTime;
            if (velocityToDeaccelerate < 0 || Mathf.Sign(velocity.x) != dashDirection)
            {
                dashDeacceleratingEnabled = false;
            }
        }



        if (dashReloadTimer != 0 || !shiftPressed) return;

        if (Mathf.Sign(velocity.x) == playerDirection)
        {
            velocity.x += dashForce * playerDirection;
        }
        else velocity.x = dashForce * playerDirection * 1.5f;

        velocityToDeaccelerate = dashForce;
        dashDirection = playerDirection;
        dashDeacceleratingEnabled = true;

        dashReloadTimer = dashReloadTime;
    }



    private float maxTimeSaveVelocityX = 0.5f, timeSaveVelocityX = 0.5f;
    void MakeCooldown()
    {
        timeSaveVelocityX = Mathf.Max(0, timeSaveVelocityX - Time.fixedDeltaTime);
        if (timeSaveVelocityX == 0) savedVelocityX = 0;

        swordTimer = Mathf.Max(0, swordTimer - Time.fixedDeltaTime);

        dashReloadTimer = Mathf.Max(0, dashReloadTimer - Time.fixedDeltaTime);
    }


}
