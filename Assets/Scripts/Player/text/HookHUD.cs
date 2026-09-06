using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookHUD : MonoBehaviour
{
    [SerializeField] private Image hookFill;
    [SerializeField] private TextMeshProUGUI hookText;
    [SerializeField] private HookThrow hookScript;

    private float hookAmount => hookScript.hookAmount;
    private float hookMaxAmount => hookScript.hookMaxAmount;


    private float hookAmountHUD = 0;
    private void FixedUpdate()
    {
        if (hookAmount == hookAmountHUD) return;

        hookFill.fillAmount = (hookAmount / hookMaxAmount);

        if ((int)hookAmount != hookAmountHUD)
        {
            if(hookAmount > hookAmountHUD)
            {
                hookText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1, 1);
            }

            hookAmountHUD = (int)hookAmount;
            hookText.text = hookAmountHUD.ToString();
            
        }
    }
}
