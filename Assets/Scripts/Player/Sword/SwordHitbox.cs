using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    private SwordSwingController controller;

    void Start()
    {
        controller = GetComponentInParent<SwordSwingController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (controller != null)
        {
            controller.ProcessHit(other);
        }
    }
}