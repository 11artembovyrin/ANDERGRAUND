using UnityEngine;

public class DestroyingSword : MonoBehaviour
{
    public float timer = 0.2f;
    public GameObject sword;
    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer < 0)
        {
            Destroy(sword);
            print(timer);
        }
    }
}
