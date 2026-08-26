using UnityEngine;

public class BulletScript : MonoBehaviour 
{
    [Header("Settings")]
    public bool hitWithNoCooldown = false;
    public bool isHitScan = true;

    public float damage = 1;
    public float speed;
    public float timer;

    public LayerMask targetLayer;
    public LayerMask groundLayer;

    [HideInInspector] public Vector2 startPosition;
    [HideInInspector] public Vector2 direction;

    private float distance = 0;
    private Vector2 currentPosition;

    public CircleCollider2D circleCollider;


    private void Start()
    {
        direction = direction.normalized;
    }


    private void FixedUpdate()
    {
        distance = speed * Time.fixedDeltaTime;
        transform.position += (Vector3)(speed * direction * Time.fixedDeltaTime);

        RaycastHit2D hit;
        if (isHitScan)
            hit = Physics2D.Raycast(transform.position, direction, distance, targetLayer | groundLayer);
        else hit = Physics2D.CircleCast(transform.position, circleCollider.radius, direction, distance, targetLayer | groundLayer);

        if (hit)
        {
            int hitLayerMask = 1 << hit.collider.gameObject.layer;

            if ((hitLayerMask & groundLayer) != 0)
            {
                Collapse();
            }

            else if ((hitLayerMask & targetLayer) != 0)
            {
                Health health = hit.collider.gameObject.GetComponent<Health>();
                if (!hitWithNoCooldown) health.TakeHit(damage);
                else health.TakeHitWithNoCooldown(damage);


                Collapse();
            }
        }

        timer -= Time.fixedDeltaTime;
        if (timer < 0) Collapse();


    }

    void Collapse()
    {
        Destroy(gameObject);
    }
}
