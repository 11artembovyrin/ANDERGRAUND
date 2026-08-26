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
    [HideInInspector] public float fireTimer;
    public float shootRadius = 20f;


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        fireTimer = Mathf.Min(fireRate, fireTimer + Time.fixedDeltaTime);
    }

}
