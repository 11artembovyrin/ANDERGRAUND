using System;
using System.Threading;
using UnityEngine;

public class SwordSwingController : MonoBehaviour 
{

    [SerializeField] private Collider2D smallCollider;
    [SerializeField] private Collider2D mediumCollider;
    [SerializeField] private Collider2D bigCollider;

    [SerializeField] private Animator animator;


    
    
    
    private PlayerController playerController;
    private float smallSwingTime;
    private float mediumSwingTime;
    private float bigSwingTime;


    public event Action<GameObject> EnemyHitted;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        smallSwingTime = playerController.smallSwingTime;
        mediumSwingTime = playerController.mediumSwingTime;
        bigSwingTime = playerController.bigSwingTime;


        Utils.ChangeClipDuration(animator, "SmallSwing", "smallSwingSpeed", smallSwingTime);
        Utils.ChangeClipDuration(animator, "MediumSwing", "mediumSwingSpeed", smallSwingTime);
        Utils.ChangeClipDuration(animator, "BigSwing", "bigSwingSpeed", smallSwingTime);
    }

    
    // 1 - small
    // 2 - medium
    // 3 - big
    public void InitiateSwingByType(int type)
    {
        smallCollider.enabled = false;
        mediumCollider.enabled = false;
        bigCollider.enabled = false;


        animator.SetInteger("swingType", type);
        if(type == 1)
        {
            smallCollider.enabled = true;
        }
        if(type == 2)
        {
            mediumCollider.enabled = true;
        }
        if(type == 3)
        {
            bigCollider.enabled = true;
        }
    }



    public void ProcessHit(Collider2D enemyCollider)
    {
        if (enemyCollider.CompareTag("Enemy"))
        {
            GameObject enemy = enemyCollider.gameObject;
            EnemyHitted?.Invoke(enemy);
        }
    }
}
