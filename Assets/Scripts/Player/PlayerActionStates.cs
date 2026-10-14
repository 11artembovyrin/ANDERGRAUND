using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerActionState : IState
{
    public PlayerController player;

    public PlayerActionState(PlayerController player)
    {
        this.player = player;
    }


    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Exit() { }
}


public class IdleState : PlayerActionState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Update() 
    {
        if (player.movementStateMachine.GetCurrentState() == player.nothingState) return;

        if (Input.GetMouseButtonDown(0) && player.swordTimer == 0)
            player.actionStateMachine.SwitchState(player.swordAttackingState);

        if (Input.GetKeyDown(KeyCode.Q) && player.currentAmmo >= 1)
            player.actionStateMachine.SwitchState(player.shotgunShootingState);
    }
}





public class SwordAttackingState : PlayerActionState
{
    public SwordAttackingState(PlayerController player) : base(player) { }

    // types
    // 1 - small
    // 2 - medium
    // 3 - big

    private float timer;
    private float currentVelocity;
    private int type;

    private GameObject currentSwordInstance;
    private float damage;

    public override void Enter()
    {


        currentVelocity = player.velocity.magnitude;
        if (currentVelocity <= player.minVelocityForMediumSwing)
        {
            timer = player.smallSwingTime;
            player.swordTimer = player.smallSwingCooldown;
            damage = player.smallSwingDamage;
            type = 1;
        }
        else if (currentVelocity <= player.minVelocityForBigSwing)
        {
            timer = player.mediumSwingTime;
            player.swordTimer = player.mediumSwingCooldown;
            damage = player.mediumSwingDamage;
            type = 2;
        }
        else
        {
            timer = player.bigSwingTime;
            player.swordTimer = player.bigSwingCooldown;
            damage = player.bigSwingDamage;
            type = 3;
        }


        quaternion rotation = Utils.GetRotationToMouse(player.transform.position);
        currentSwordInstance = GameObject.Instantiate(player.swordPrefab, player.transform.position, rotation, player.transform);
        SwordSwingController swordController = currentSwordInstance.GetComponent<SwordSwingController>();

        swordController.InitiateSwingByType(type);
        swordController.EnemyHitted += DealSwingDamage;
    }
        

    public override void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            
            player.actionStateMachine.SwitchState(player.idleState);
        }
    }




    public override void Exit()
    {
        GameObject.Destroy(currentSwordInstance);
    }


    public void DealSwingDamage(GameObject enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        if(enemyHealth != null)
        {
            enemyHealth.TakeHit(damage);
        }
        player.currentAmmo = Mathf.Min(player.currentAmmo + damage * player.shotgunAmmoMultiplier, player.maxAmmo);
    }



}

public class ShotgunShootingState : PlayerActionState
{
    public ShotgunShootingState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.currentAmmo -= 1;

        if (player.movementStateMachine.GetCurrentState() == player.jumpingState)
            player.movementStateMachine.SwitchState(player.fallingState);

        quaternion rotation = Utils.GetRotationToMouse(player.transform.position);
        Vector2 direction = Utils.GetDirectionToMouse(player.transform.position);

        if (player.isGrounded == false)
        {
            Vector2 shotgunVelocity = player.recoilForce * direction * -1;

            if (shotgunVelocity.x * player.velocity.x <= 0) player.velocity.x = shotgunVelocity.x;
            else player.velocity.x += shotgunVelocity.x;

            if (shotgunVelocity.y * player.velocity.y <= 0) player.velocity.y = shotgunVelocity.y;
            else player.velocity.y += shotgunVelocity.y;

            player.canWallJump = true;
        }

        for (int i = 0; i < player.bulletNumber; i++)
        {
            GameObject bulletObject = GameObject.Instantiate(player.bulletPrefab, player.transform.position, rotation);
            BulletScript bulletScript = bulletObject.GetComponent<BulletScript>();

            float randomDegree = UnityEngine.Random.Range(-player.angleOffset, player.angleOffset);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = (angle + randomDegree) * Mathf.Deg2Rad;
            bulletScript.direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        
        player.actionStateMachine.SwitchState(player.idleState);
    }
}











//public class SwordAttackingState : PlayerActionState
//{
//    public SwordAttackingState(PlayerController player) : base(player) { }

//    private GameObject currentSwortInstance;

//    private float angle;
//    private Vector2 direction;
//    private bool hitted;
//    private float scaleMultiplierFromVelocity;
//    private CapsuleCollider2D col;
//    public override void Enter()
//    {
//        player.swordTimer = player.swordCooldown;

//        Quaternion rotation = Utils.GetRotationToMouse(player.transform.position);
//        angle = rotation.eulerAngles.z;
//        direction = Utils.GetDirectionToMouse(player.transform.position);

//        currentSwortInstance = Object.Instantiate(player.swordPrefab, player.transform.position, rotation);
//        SpriteRenderer spriteRenderer = player.playerSprite.GetComponent<SpriteRenderer>();
//        SwordSwingController swordSwing = currentSwortInstance.GetComponent<SwordSwingController>();
//        currentSwortInstance.transform.SetParent(player.transform);

//        scaleMultiplierFromVelocity = 1 + (player.velocity.magnitude * player.swordVelocityMultiplierSize);
//        swordSwing.swordSprite.transform.localScale *= scaleMultiplierFromVelocity;

//        col = currentSwortInstance.GetComponent<CapsuleCollider2D>();
//        col.offset = new Vector2(swordSwing.swordSprite.transform.localPosition.x, col.offset.y);


//        swordSwing.playerSpriteTransform = spriteRenderer.transform;
//        hitted = false;
//    }


//    public override void FixedUpdate()
//    {
//        if (currentSwortInstance == null) {
//            player.actionStateMachine.SwitchState(player.idleState);
//            return;
//        };

//        RaycastHit2D[] hit = Physics2D.CapsuleCastAll(col.bounds.center, col.size * scaleMultiplierFromVelocity, col.direction, angle, Vector2.zero, 0, player.enemyLayer);
//        float damageDealed = player.swordBaseDamage + player.velocity.magnitude * player.swordVelocityMultiplierDamage;
//        damageDealed = Mathf.Round(damageDealed * 10) * 0.1f;



//        for (int i = 0; i < hit.Length; i++)
//        {
//            RaycastHit2D enemy = hit[i];

//            Vector2 checkWallDirection = (enemy.point - (Vector2)player.transform.position).normalized;
//            float checkWallDistance = (enemy.point - (Vector2)player.transform.position).magnitude;
//            RaycastHit2D checkWall = Physics2D.Raycast(player.transform.position, checkWallDirection, checkWallDistance, player.groundLayer);
//            if (checkWall) continue;

//            Health health = enemy.collider.GetComponent<Health>();
//            health.TakeHit(damageDealed);

//            if (!hitted)
//            {
//                player.velocity += player.swordHitForce * direction * -1f;
//                player.canWallJump = true;
//                player.currentAmmo = Mathf.Min(player.maxAmmo, player.currentAmmo + damageDealed * player.ammoMultiplier);
//                hitted = true;
//            }

//        }
//    }

//}