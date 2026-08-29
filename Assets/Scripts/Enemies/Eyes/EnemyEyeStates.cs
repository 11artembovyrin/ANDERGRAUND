using UnityEngine;

public class EyeState : IState
{
    protected EyeBaseController enemy;

    public EyeState(EyeBaseController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Exit() { }
}


public class SearchingState : EyeState
{
    public SearchingState(EyeBaseController enemy) : base(enemy) { }

    public override void Enter() { }
    public override void Update() { }
    public override void FixedUpdate() 
    {
        enemy.velocity = Vector2.MoveTowards(enemy.velocity, Vector2.zero, enemy.acceleration * Time.fixedDeltaTime);

        Vector2 eyePos = enemy.transform.position;
        Vector2 playerPos = enemy.playerTransform.position;

        if (Vector2.Distance(eyePos, playerPos) <= enemy.searchRadius)
        {
            enemy.stateMachine.SwitchState(enemy.runningState);
        }

    }
    public override void Exit() { }
}


public class RunningState : EyeState
{
    public RunningState(EyeBaseController enemy) : base(enemy) { }

    public override void Enter() { }
    public override void Update() { }
    public override void FixedUpdate() 
    {
        Vector2 eyePos = enemy.transform.position;
        Vector2 playerPos = enemy.playerTransform.position;
        playerPos.y += enemy.offsetY;

        if(Vector2.Distance(eyePos, playerPos) >= enemy.searchRadius * 1.2f)
        {
            enemy.stateMachine.SwitchState(enemy.searchingState);
        }


        if (enemy is EyeLaserController eyeLaser)
        {
            if (Vector2.Distance(eyePos, playerPos) <= eyeLaser.shootRadius && eyeLaser.fireTimer == eyeLaser.fireRate)
            {
                enemy.stateMachine.SwitchState(enemy.shootingState);
            }
        }

        if (enemy is EyeDashController eyeDasher)
        {
            if (Vector2.Distance(eyePos, playerPos) <= eyeDasher.dashDistance && eyeDasher.dashCooldownTimer == eyeDasher.dashCooldown)
            {
                enemy.stateMachine.SwitchState(enemy.dashingState);
            }
        }




        Vector2 targetVelosity = (playerPos - eyePos).normalized * enemy.maxMoveSpeed;

        enemy.velocity = Vector2.MoveTowards(enemy.velocity, targetVelosity, enemy.acceleration * Time.fixedDeltaTime);
    }
    public override void Exit() { }
}



public class ShootingState : EyeState
{
    public ShootingState(EyeBaseController enemy) : base(enemy) { }

    public override void Enter() { }
    public override void Update() { }
    private float timer = 0f;
    public override void FixedUpdate() 
    { 
        if (enemy is EyeLaserController eyeLaser)
        {
            enemy.velocity = Vector2.MoveTowards(enemy.velocity, Vector2.zero, enemy.acceleration * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;

            if (timer >= eyeLaser.chargeTime)
            {
                GameObject bulletObject = Object.Instantiate(eyeLaser.laserPrefab, enemy.transform.position, Quaternion.identity);
                BulletScript bullet = bulletObject.GetComponent<BulletScript>();
                Vector2 directionToTarget = (enemy.playerTransform.position - bulletObject.transform.position).normalized;
                float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
                
                if (eyeLaser.isElite)
                {
                    GameObject bulletObjectElite1 = Object.Instantiate(eyeLaser.laserPrefab, enemy.transform.position, Quaternion.identity);
                    BulletScript bulletElite1 = bulletObjectElite1.GetComponent<BulletScript>();
                    float newAngle1 = (angle + 15) * Mathf.Deg2Rad;
                    bulletElite1.direction = new Vector2(Mathf.Cos(newAngle1), Mathf.Sin(newAngle1));

                    GameObject bulletObjectElite2 = Object.Instantiate(eyeLaser.laserPrefab, enemy.transform.position, Quaternion.identity);
                    BulletScript bulletElite2 = bulletObjectElite2.GetComponent<BulletScript>();
                    float newAngle2 = (angle - 15) * Mathf.Deg2Rad;
                    bulletElite2.direction = new Vector2(Mathf.Cos(newAngle2), Mathf.Sin(newAngle2));

                }
                


                bullet.direction = directionToTarget;


                eyeLaser.fireTimer = 0;
                timer = 0;
                enemy.stateMachine.SwitchState(enemy.runningState);
            }
        }
        
    }
    public override void Exit() { }
}


public class DashingState : EyeState
{
    public DashingState(EyeBaseController enemy) : base(enemy) { }

    private float dashCount = 1;
    private float chargeTimer;
    private float dashTimer;
    public override void Enter() 
    {
        if (enemy is EyeDashController eyeDasher && eyeDasher.isElite)
        {
            dashCount = 3;
        }
        chargeTimer = 0f;
        dashTimer = 0f;
    }

    

    public override void FixedUpdate()
    {
        if (enemy is EyeDashController eyeDasher)
        {
            if (chargeTimer < eyeDasher.dashChargeTime)
            {
                enemy.velocity = Vector2.MoveTowards(enemy.velocity, Vector2.zero, enemy.acceleration * Time.fixedDeltaTime * 5);
                chargeTimer += Time.fixedDeltaTime;
                if (chargeTimer >= eyeDasher.dashChargeTime)
                {
                    Vector2 targetPos = Vector2.zero;
                    if (eyeDasher.isElite)
                    {
                        float distanceToTarget = (enemy.playerTransform.position - enemy.transform.position).magnitude;
                        float timeToTarget = (distanceToTarget / eyeDasher.dashSpeed) * 1f;
                        targetPos = (Vector2)enemy.playerTransform.position + enemy.playerController.velocity * timeToTarget;
                    }
                    else targetPos = enemy.playerTransform.position;

                    Vector2 directionToTarget = (targetPos - (Vector2)enemy.transform.position).normalized;
                    enemy.velocity = eyeDasher.dashSpeed * directionToTarget;
                }
            }
            else
            {
                dashTimer += Time.fixedDeltaTime;


                if (dashTimer >= eyeDasher.dashDuration)
                {
                    dashCount -= 1;
                    if(dashCount > 0)
                    {
                        // обратно в конец первого условия
                        chargeTimer = eyeDasher.dashChargeTime - Time.fixedDeltaTime;
                        dashTimer = 0f;
                    }
                    else
                    {
                        eyeDasher.dashCooldownTimer = 0;
                        enemy.velocity *= 0.5f;
                        enemy.stateMachine.SwitchState(enemy.runningState);
                    }
                }
            }
            
        }

    }
}