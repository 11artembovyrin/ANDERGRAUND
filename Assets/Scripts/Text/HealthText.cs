using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    private Transform spriteTransform;
    private Vector2 offset;
    private GameObject parent;
    private Health health;
    private TextMeshPro text;
    void Awake()
    {

        offset = transform.localPosition;

        SpriteRenderer spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        spriteTransform = spriteRenderer.transform;

        health = transform.parent.GetComponent<Health>();
        text = GetComponent<TextMeshPro>();

    }

    private void LateUpdate()
    {
        transform.position = (Vector2)spriteTransform.position + offset;
        text.text = (health.currentHealth).ToString();
        
    }
}
