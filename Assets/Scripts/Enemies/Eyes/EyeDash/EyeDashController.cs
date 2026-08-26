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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        dashCooldownTimer = Mathf.Min(dashCooldown, dashCooldownTimer + Time.fixedDeltaTime);
    }
}
