using UnityEngine;
using UnityEngine.Rendering;
public class EyeBaseController : MonoBehaviour
{


    [Header("Movement")]
    public float maxMoveSpeed = 10f;
    public float acceleration = 5f;

    [Header("AI Settings")]
    public float searchRadius = 50f;
    public float offsetY = 1f;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;

    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public PlayerController playerController;

    public StateMachine stateMachine;

    public SearchingState searchingState { get; private set; }
    public RunningState runningState { get; private set; }
    public ShootingState shootingState { get; private set; }
    public DashingState dashingState { get; private set; }

    private CircleCollider2D col;
    private Rigidbody2D rb;


    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 

        { 
            playerTransform = player.transform; 
            playerController = player.GetComponent<PlayerController>();
        }


        searchingState = new SearchingState(this);
        runningState = new RunningState(this);
        shootingState = new ShootingState(this);
        dashingState = new DashingState(this);

        stateMachine = new StateMachine();
        stateMachine.SwitchState(searchingState);

    }

    private void Update()
    {
        stateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate();

        MoveEye(velocity * Time.fixedDeltaTime);

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, transform.localScale.x / 2, Vector2.zero, 0, playerLayer);
        if (hit)
        {
            Health health = hit.collider.gameObject.GetComponent<Health>();
            health.TakeHit(1);
        }
    }

    void MoveEye(Vector2 delta)
    {
        

        float bodyRadius = transform.localScale.x / 2;
        if (delta.magnitude > 0.001f)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, bodyRadius, delta.normalized, delta.magnitude, groundLayer);
            if (hit)
            {
                velocity = Vector2.Reflect(velocity, hit.normal);
                return;
            }
        }

        transform.position += (Vector3)delta;
    }
}
