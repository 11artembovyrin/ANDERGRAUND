using UnityEngine;

public class TurretState : IState
{
    protected TurretController turret;

    public TurretState(TurretController turret)
    {
        this.turret = turret;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Exit() { }
}


public class TurretWaitingState : TurretState
{
    public TurretWaitingState(TurretController turret) : base(turret) { }

    private float reloadingTimer;
    public override void Enter()
    {
        reloadingTimer = turret.reloadTime;
        turret.shield.transform.position = Vector2.zero;
    }

    public override void FixedUpdate()
    {
        if (reloadingTimer >= 0) reloadingTimer -= Time.fixedDeltaTime;
        float distance = (turret.player.interpolatedPosition - (Vector2)turret.top.transform.position).magnitude;
        
        
        if (reloadingTimer < 0 && distance < turret.aimRadius)
        {
            turret.stateMachine.SwitchState(turret.aimingState);
        }
    }
}


public class TurretAimingState : TurretState
{
    public TurretAimingState(TurretController turret) : base(turret) { }

    private float chargingTimer;
    private float pingTimer = 0;
    public override void Enter()
    {
        turret.health.damageMultiplier = 0;
        turret.shield.transform.localPosition = Vector2.zero;
        chargingTimer = turret.chargeTime;
    }

    public override void FixedUpdate()
    {
        chargingTimer -= Time.fixedDeltaTime;
        pingTimer += 2 * Time.fixedDeltaTime;

        if (pingTimer > chargingTimer)
        {
            turret.DoPingLaser();
            pingTimer = 0;
        }

        if (chargingTimer < 0)
        {
            turret.stateMachine.SwitchState(turret.shootingState);
        }
    }


    public override void LateUpdate()
    {
        Vector2 direction = (turret.player.interpolatedPosition - (Vector2)turret.top.transform.position).normalized;
        float distance = (turret.player.interpolatedPosition - (Vector2)turret.top.transform.position).magnitude;
        RaycastHit2D hit = Physics2D.Raycast(turret.top.transform.position, direction, distance, turret.targetLayer | turret.groundLayer);

        Vector2 laserFinalPos;
        if (hit.point == Vector2.zero || hit.collider.gameObject.CompareTag("Player"))
        {
            laserFinalPos = turret.player.interpolatedPosition;
            turret.isTakingHit = true;
        }
        else
        {
            laserFinalPos = hit.point;
            turret.isTakingHit = false;
        }

        turret.line.SetPosition(0, turret.top.transform.position);
        turret.line.SetPosition(1, laserFinalPos);
    }

    public override void Exit()
    {
        turret.health.damageMultiplier = 1;
        turret.shield.transform.position = Vector2.zero;
    }


}


public class TurretShootingState : TurretState
{
    public TurretShootingState(TurretController turret) : base(turret) { }

    private Vector2 direction;
    private float distance;
    private float shootTimer;
    public override void Enter()
    {
        shootTimer = 0.1f;

        direction = (turret.player.transform.position - turret.top.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(turret.top.transform.position, direction, 1000, turret.player.groundLayer);
        Vector2 lineFinalPosition;
        if (hit) lineFinalPosition = hit.point;
        else lineFinalPosition = direction * 1000;

        turret.line.SetPosition(1, lineFinalPosition);

        distance = hit.distance;
    }


    
    public override void FixedUpdate()
    {
        shootTimer -= Time.fixedDeltaTime;
        if (shootTimer < 0)
        {
            RaycastHit2D hit = Physics2D.BoxCast(turret.top.transform.position, new Vector2(3, 3), 0, direction, distance, turret.player.playerLayer);
            if (hit)
            {
                turret.playerHealth.TakeHit(1);
                turret.player.velocity += turret.hitForce * direction;
            }
            turret.DoShootLaser();
            turret.stateMachine.SwitchState(turret.waitingState);
        }
    }
}