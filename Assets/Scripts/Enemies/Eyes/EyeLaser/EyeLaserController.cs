using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Rendering;
public class EyeLaserController : EyeBaseController
{


    public bool isElite = false;

    [Header("Referenses")]
    public GameObject laserPrefab;

    [Header("Shooting settings")]
    public float fireRate = 2f;
    public float chargeTime = 0.5f;
    public float recoilForce = 10f;
    [HideInInspector] public float fireTimer;
    public float shootRadius = 20f;


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        fireTimer = Mathf.Min(fireRate, fireTimer + Time.fixedDeltaTime);

        
        if (spriteRenderer != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle - 180f);
        }
    }

}
