using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookThrow : MonoBehaviour
{
    [SerializeField] private float hookSpeed = 20f;
    [SerializeField] private float hookMaxDistance = 100f;
    [SerializeField] private LayerMask hookableLayer;
    [SerializeField] private LayerMask notHookableLayer;
    [SerializeField] private GameObject hookPrefab;

    [SerializeField] private float hookMaxAmount = 3f;
    [SerializeField] private float hookRegen = 0.5f;

    [HideInInspector] public float hookAmount;

    public GameObject currentHook;
    private Rigidbody2D hookRb;
    private Vector3 hookDirection;
    private Vector3 hookCurrentPosition;
    private float hookCurrentDistance;
    private Vector3 hookStartPosition;
    private bool isHookActive;
    private bool isHookReturning;
    [HideInInspector] public bool isHookAttached;

    [HideInInspector] public Vector2 hookFinalPosition;


    public PlayerController player;
    //private GameObject lineObj;
    private LineRenderer line;


    private void Start()
    {
        hookAmount = hookMaxAmount;

        line = Utils.CreateSimpleLine(new Color(1, 1, 1, 0.5f), 0.25f);

        //lineObj = new GameObject("Line");
        //line = lineObj.AddComponent<LineRenderer>();
        //line.material = new Material(Shader.Find("Sprites/Default"));
        //line.startColor = new Color(1, 1, 1, 0.5f);
        //line.endColor = new Color(1, 1, 1, 0.5f);
        //line.startWidth = 0.25f;
        //line.endWidth = 0.25f;
        //line.positionCount = 2;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isHookActive && hookAmount >= 1f)
        {
            FireHook();
        }

        if (Input.GetMouseButtonUp(1) && isHookActive)
        {
            StartHookReturning();
        }
    }


    private void FixedUpdate()
    {

        if (isHookActive && !isHookAttached && !isHookReturning)
        {
            UpdateHookPosition();
        }

        if (isHookReturning)
        {
            ReturningHook();
        }

        if (isHookAttached)
        {
            AttachingHook();
        }

        hookAmount = Mathf.Min(hookMaxAmount, hookAmount + hookRegen * Time.fixedDeltaTime);
    }


    private void LateUpdate()
    {
        if (currentHook != null)
        {
            line.SetPosition(0, player.playerSprite.transform.position);
            line.SetPosition(1, currentHook.transform.position);
        }
        else
        {
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
        }
    }


    public void StartHookReturning()
    {
        hookCurrentDistance = 0;
        isHookReturning = true;
        isHookAttached = false;
    }


    void FireHook()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        hookDirection = (mousePos - transform.position).normalized;
        currentHook = Instantiate(hookPrefab, transform.position, Quaternion.identity);
        hookRb = currentHook.GetComponent<Rigidbody2D>();

        isHookActive = true;
        isHookAttached = false;

        hookStartPosition = transform.position;
        hookCurrentPosition = transform.position;

    }

    void UpdateHookPosition()
    {
        hookCurrentPosition += hookDirection * hookSpeed * Time.fixedDeltaTime;
        hookCurrentDistance += hookSpeed * Time.deltaTime;

        RaycastHit2D hitWall = Physics2D.Raycast(hookStartPosition, hookDirection, hookCurrentDistance, hookableLayer);
        if (hitWall)
        {
            int hitLayerMask = 1 << hitWall.collider.gameObject.layer;
            hookAmount -= 1;
            currentHook.transform.position = hitWall.point;
            //hookRb.MovePosition(hitWall.point);
            isHookAttached = true;
            hookFinalPosition = currentHook.transform.position;
            currentHook.transform.SetParent(hitWall.collider.gameObject.transform);

            if ((hitLayerMask & player.groundLayer) != 0)
            {
                player.movementStateMachine.SwitchState(player.hookingState);
            }
            else if ((hitLayerMask & player.enemyLayer) != 0)
            {
                player.movementStateMachine.SwitchState(player.hookingState);
            }
            
        }
        //else currentHook.transform.position = hookCurrentPosition;
        else hookRb.MovePosition(hookCurrentPosition);


        if (!hitWall)
        {
            RaycastHit2D hitSpike = Physics2D.Raycast(hookStartPosition, hookDirection, hookCurrentDistance, notHookableLayer);
            if (hitSpike)
            {
                hookAmount -= 1;
                StartHookReturning();
            }
        }
        



        if (Vector3.Distance(hookCurrentPosition, transform.position) > hookMaxDistance )
            StartHookReturning();
    }


    void ReturningHook()
    {
        if ( currentHook == null )
        {
            isHookActive = false;
            isHookReturning = false;
            return;
        }

        hookCurrentPosition = currentHook.transform.position;
        Vector3 directionToPlayer = (transform.position - hookCurrentPosition).normalized;
        float distanceToPlayer = Vector3.Distance(hookCurrentPosition, transform.position);

        hookCurrentPosition += directionToPlayer * (2 * hookSpeed) * Time.fixedDeltaTime;
        currentHook.transform.position = hookCurrentPosition;

        if (distanceToPlayer < 2 * hookSpeed * Time.fixedDeltaTime)
        {
            Destroy(currentHook);
            currentHook = null;
            isHookReturning = false;
            isHookActive = false;
        }
    }

    void AttachingHook()
    {
        if (!isHookAttached)
        {
            StartHookReturning();
        }
    }


}

