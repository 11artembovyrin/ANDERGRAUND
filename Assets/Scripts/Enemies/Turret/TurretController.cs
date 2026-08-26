using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public GameObject sprite;
    public GameObject shield;
    public GameObject top;
    public float reloadTime = 2f;
    public float chargeTime = 3f;
    public float aimRadius = 50f;
    public float hitForce = 80f;
    public LayerMask targetLayer;
    public LayerMask groundLayer;
    public Color lineDefaultColor = new Color(1, 1, 0, 0.7f);
    public Color linePingColor = new Color(0, 1, 1, 0.7f);
    public Color lineShootColor = new Color(0, 0, 0, 1);

    [HideInInspector] public GameObject playerObject;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public SpriteRenderer playerSprite;
    [HideInInspector] public Health playerHealth;
    [HideInInspector] public LineRenderer line;
    [HideInInspector] public Health health;

    [HideInInspector] public bool isTakingHit = false;


    public StateMachine stateMachine;

    public TurretWaitingState waitingState { get; private set; }
    public TurretAimingState aimingState { get; private set; }
    public TurretShootingState shootingState { get; private set;  }

    void Start()
    {
        shield.transform.position = sprite.transform.position;

        line = Utils.CreateSimpleLine(lineDefaultColor, 0.25f);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();
        playerHealth = playerObject.GetComponent<Health>();
        health = GetComponent<Health>();


        waitingState = new TurretWaitingState(this);
        aimingState = new TurretAimingState(this);
        shootingState = new TurretShootingState(this);

        stateMachine = new StateMachine();
        stateMachine.SwitchState(waitingState);
    }

    private void LateUpdate()
    {
        stateMachine.LateUpdate();

        //if (!isAiming) return;

        //direction = (player.interpolatedPosition - (Vector2)top.transform.position).normalized;
        //float distance = (player.interpolatedPosition - (Vector2)top.transform.position).magnitude;
        //RaycastHit2D hit = Physics2D.Raycast(top.transform.position, direction, distance, targetLayer | groundLayer);

        //Vector2 laserFinalPos;
        //if (hit.point == Vector2.zero || hit.collider.gameObject.CompareTag("Player"))
        //{
        //    laserFinalPos = player.interpolatedPosition;
        //    isTakingHit = true;
        //}
        //else
        //{
        //    laserFinalPos = hit.point;
        //    isTakingHit = false;
        //}

        //line.SetPosition(0, top.transform.position);
        //line.SetPosition(1, laserFinalPos);
    }


    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();

        //if (!isAiming) reloadingTimer += Time.fixedDeltaTime;
        //float distance = (player.transform.position - top.transform.position).magnitude;
        //if (reloadingTimer > reloadTime && distance < aimRadius)
        //{
        //    isAiming = true;
        //    chargingTimer = chargeTime;
        //    shootingTimer = 0;
        //    reloadingTimer = 0;

        //    shield.transform.localPosition = Vector2.zero;
        //}

        //if (isAiming)
        //{
        //    health.damageMultiplier = 0;
        //    chargingTimer -= Time.fixedDeltaTime;
        //    shootingTimer += 4 * Time.fixedDeltaTime;

        //    if (shootingTimer > chargingTimer)
        //    {
        //        shootingTimer = 0;
        //        StartCoroutine(PingLaser());
        //    }

        //    if (chargingTimer < 0)
        //    {
        //        StartCoroutine(ShootLaser());

        //        if (isTakingHit)
        //        {
        //            playerHealth.TakeHit(1);
        //            player.velocity += hitForce * direction;
        //        }

        //        shield.transform.position = Vector2.zero;
        //        isAiming = false;
        //    }
        //}
        //else
        //{
        //    health.damageMultiplier = 1;
        //}
    }


    IEnumerator PingLaser()
    {
        line.startColor = linePingColor;
        line.endColor = linePingColor;

        yield return new WaitForSecondsRealtime(0.1f);

        line.startColor = lineDefaultColor;
        line.endColor = lineDefaultColor;

    }
    public void DoPingLaser()
    {
        StartCoroutine(PingLaser());
    }



    IEnumerator ShootLaser()
    {
        line.startColor = lineShootColor; line.endColor = lineShootColor;
        line.startWidth = 3; line.endWidth = 3;

        yield return new WaitForSecondsRealtime(0.03f);
        line.startWidth = 2; line.endWidth = 2;
        yield return new WaitForSecondsRealtime(0.03f);
        line.startWidth = 1f; line.endWidth = 1f;
        yield return new WaitForSecondsRealtime(0.03f);

        line.startColor = lineDefaultColor; line.endColor = lineDefaultColor;
        line.startWidth = 0.25f; line.endWidth = 0.25f;
        line.SetPosition(0, Vector3.zero); line.SetPosition(1, Vector3.zero);
    }
    public void DoShootLaser()
    {
        StartCoroutine(ShootLaser());
    }

}
