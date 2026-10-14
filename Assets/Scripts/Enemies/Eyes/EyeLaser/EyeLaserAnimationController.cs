using UnityEngine;

public class EyeLaserAnimationController : MonoBehaviour
{
    private EyeLaserController controller;
    private Animator animator;



    private void Start()
    {
        controller = GetComponent<EyeLaserController>();
        animator = GetComponent<Animator>();

        float chargeClipLength = Utils.GetClipLength(animator, "RedEyeCharge");
        float targetDuration = controller.chargeTime;
        animator.SetFloat("chargeSpeed", chargeClipLength / targetDuration);
    }

    private void Update()
    {
        if(controller.stateMachine.GetCurrentState() == controller.shootingState)
        {
            animator.SetBool("isCharging", true);
        }
        else
        {
            animator.SetBool("isCharging", false);
        }
    }

}
