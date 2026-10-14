using UnityEngine;

public class SwordAnimationContrroller : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite smallSwordSpriteBegin;
    [SerializeField] private Sprite smallSwordSpriteEnd;
    
    private SwordSwingController swingController;
    private void Start()
    {
        swingController = GetComponent<SwordSwingController>();

    }

}
