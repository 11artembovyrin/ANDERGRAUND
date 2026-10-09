using TMPro;
using UnityEngine;

public class DamageDealedTextController : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Vector3 velocity;
    private Color textColor;

    [Header("MoveSettings")]
    [SerializeField] private float minHorizontalForce = -2f;
    [SerializeField] private float maxHorizontalForce = 2f;
    [SerializeField] private float minVerticalForce = 4f;
    [SerializeField] private float maxVerticalForce = 6f;
    [SerializeField] private float friction = 10f;

    [Header("TimeSettings")]
    [SerializeField] private float lifetime = 1.0f;
    private float lifeTimer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        textColor = textMesh.color;
        lifeTimer = lifetime;

        float randomX = Random.Range(minHorizontalForce, maxHorizontalForce);
        float randomY = Random.Range(minVerticalForce, maxVerticalForce);
        velocity = new Vector3(randomX, randomY, 0);
    }

    public void Setup(float damageAmount)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damageAmount.ToString("F1");
    }

    private void Update()
    {
        velocity.y -= friction * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, 0);
        transform.position += velocity * Time.deltaTime;

        lifeTimer -= Time.deltaTime;
        float alpha = (lifeTimer * lifeTimer) / lifetime;

        textColor.a = Mathf.Min(1, alpha);
        textMesh.color = textColor;

        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
