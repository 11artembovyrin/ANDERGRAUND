using UnityEngine;

public class EyeDashController : EyeBaseController
{
    [Header("Dash Settings")]
    public float dashSpeed = 30f;
    public float dashDuration = 1f;
    public float dashCooldown = 2f;
    [HideInInspector] public float dashCooldownTimer = 0f;
    public float dashChargeTime = 0.5f;
    public float dashDistance = 20f;
    public bool isElite = false;



    private float srpriteRotateSpeed = 720f;

    private void Update()
    {
        if (spriteRenderer != null)
        {
            Vector2 directionToTurn = Vector2.zero;

            if (stateMachine.GetCurrentState() == greenChargingState)
            {
                directionToTurn = (playerTransform.position - transform.position).normalized;
            }
            else
            {
                directionToTurn = velocity.normalized;
            }

            float targetAngle = Mathf.Atan2(directionToTurn.y, directionToTurn.x) * Mathf.Rad2Deg - 180f;

            float currentAngle = Mathf.MoveTowardsAngle(spriteRenderer.transform.eulerAngles.z, targetAngle, srpriteRotateSpeed * Time.deltaTime);
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }



    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        dashCooldownTimer = Mathf.Min(dashCooldown, dashCooldownTimer + Time.fixedDeltaTime);
    }
}
