using System.Threading;
using UnityEngine;

public class SwordSwing : MonoBehaviour 
{
    public Transform playerSpriteTransform;
    public GameObject swordSprite;
    public float timer = 0.1f;
    private void LateUpdate()
    {
        swordSprite.transform.localPosition = new Vector3(swordSprite.transform.localScale.y, swordSprite.transform.localPosition.y, 0);
        transform.position = playerSpriteTransform.position;
        timer -= Time.deltaTime;
        if (timer < 0) Destroy(gameObject);
    }
}
