using UnityEngine;

public class EnemyHPBarController : MonoBehaviour
{
    [SerializeField] private Color maxHPColor = Color.green;
    [SerializeField] private Color minHPColor = Color.red;

    private SpriteRenderer sprite;
    private Health health;
    private float fullHPBarWidth;
    private float currentHealth = 0;
    private float maxHealth;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponentInParent<Health>();
        fullHPBarWidth = sprite.size.x;
        maxHealth = health.maxHealth;
    }

    
    void Update()
    {
        if (currentHealth != health.currentHealth)
        {
            currentHealth = health.currentHealth;
            float healthNormalized = currentHealth / maxHealth;
            sprite.size = new Vector2(Mathf.Max(0.1f, fullHPBarWidth * healthNormalized), sprite.size.y);
            sprite.color = Color.Lerp(minHPColor, maxHPColor, healthNormalized);
        }
    }
}
