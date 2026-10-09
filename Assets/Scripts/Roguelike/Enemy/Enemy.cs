using System;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    private Health health;
    private Animator animator;
    [SerializeField] private GameObject spriteObject;
    public event Action<Enemy> Died;

    public bool IsDead => health.IsDead;

    private void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        health.Died += OnHealthDied;

        if (spriteObject != null) spriteObject.SetActive(true);

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
    private void OnDisable()
    {
        health.Died -= OnHealthDied;
        if (spriteObject != null) spriteObject.SetActive(false);
    }

    private void OnHealthDied(Health health)
    {
        Died?.Invoke(this);
    }
}
