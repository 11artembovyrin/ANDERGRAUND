using UnityEngine;

public class EyeDashAnimationController : MonoBehaviour
{
    private EyeDashController controller;
    private Animator animator;


    private void Start()
    {
        controller = GetComponent<EyeDashController>();
        animator = GetComponent<Animator>();


    }

    private void FixedUpdate()
    {
        if (controller.stateMachine.GetCurrentState() == controller.greenChargingState)
        {
            animator.SetBool("isCharging", true);
        }
        else if (controller.stateMachine.GetCurrentState() == controller.greenDashingState)
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isCharging", false);
            animator.SetBool("isAttacking", false);
        }
    }
}
