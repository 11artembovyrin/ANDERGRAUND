using System.Collections;
using System.Net;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 3;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float damageMultiplier = 1f;

    public float damageCooldown = 1;
    [HideInInspector] public float cooldown;

    private SpriteRenderer sprite;
    private Color realColor;
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        realColor = sprite.color;
    }


    private void Start()
    {
        currentHealth = maxHealth;
        cooldown = 0;
    }

    private void FixedUpdate()
    {
        cooldown = Mathf.Max(0, cooldown - Time.fixedDeltaTime);
        currentHealth = Mathf.Round(currentHealth * 10) * 0.1f;
        if (currentHealth <= 0)
        {
            if (gameObject.CompareTag("Player"))
            {
                sprite.color = Color.white;
                PlayerController player = gameObject.GetComponent<PlayerController>();
                player.movementStateMachine.SwitchState(player.nothingState);
            }
            else Destroy(gameObject);

        }

    }

    public void TakeHit(float damage)
    {
        if (cooldown == 0)
        {
            cooldown = damageCooldown;
            currentHealth -= damage * damageMultiplier;
            StartCoroutine(ping());
        }
    }
    public void TakeHitWithNoCooldown(float damage)
    {
        StartCoroutine(ping());
        currentHealth -= damage * damageMultiplier;
    }


    IEnumerator ping()
    {
        sprite.color = Color.white;
        yield return new WaitForSecondsRealtime(0.2f);
        sprite.color = realColor;
    }
}
