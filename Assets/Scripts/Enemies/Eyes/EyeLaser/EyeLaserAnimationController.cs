using UnityEngine;

public class EyeLaserAnimationController : MonoBehaviour
{
    private EyeLaserController controller;
    private Animator animator;


    private void Start()
    {
        controller = GetComponent<EyeLaserController>();
        animator = GetComponent<Animator>();


    }

    private void FixedUpdate()
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
