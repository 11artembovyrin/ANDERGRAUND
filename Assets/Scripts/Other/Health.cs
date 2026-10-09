using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 3;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float damageMultiplier = 1f;

    public float damageCooldown = 1;
    [SerializeField] private bool destroyOnDeath = false;
    [HideInInspector] public float cooldown;

    private SpriteRenderer sprite;
    private Color realColor;
    private Material originalMaterial;
    private Material flashMaterial;


    public event Action<Health> Died;
    public event Action<float> DamageAplied;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            realColor = sprite.color;
            originalMaterial = sprite.material;
            flashMaterial = Resources.Load<Material>("FlashMaterial");
        }
    }


    private void OnEnable()
    {
        currentHealth = maxHealth;
        cooldown = 0;
        if (sprite != null)
        {
            sprite.color = realColor;
            sprite.material = originalMaterial;
        }
    }

    private void FixedUpdate()
    {
        cooldown = Mathf.Max(0, cooldown - Time.fixedDeltaTime);
    }

    public void TakeHit(float damage)
    {
        if (cooldown > 0 || IsDead) return;
        cooldown = damageCooldown;
        ApplyDamage(damage);
    }

    public void TakeHitWithNoCooldown(float damage)
    {
        if (IsDead) return;
        ApplyDamage(damage);
    }

    private void ApplyDamage(float damage)
    {
        float damageDealed = Mathf.Round(damage * damageMultiplier * 10) * 0.1f;
        currentHealth -= damageDealed;
        DamageAplied?.Invoke(damageDealed);

        if (sprite != null) StartCoroutine(ping());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Died?.Invoke(this);
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ping()
    {
        sprite.material = flashMaterial;
        yield return new WaitForSecondsRealtime(0.1f);
        sprite.material = originalMaterial;
    }
}