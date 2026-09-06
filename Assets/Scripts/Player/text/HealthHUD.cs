using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthHUD : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Health playerHealth;


    private float maxHealth => playerHealth.maxHealth;
    private float currentHealth => playerHealth.currentHealth;
    private float currentHealthHUD = 0;


    private Tween fillTween;
    private void FixedUpdate()
    {
        if (currentHealthHUD == currentHealth) return;

        healthFill.DOFillAmount(currentHealth / maxHealth, 0.1f);
        healthText.text = $"{currentHealth}  /  {maxHealth}";

        currentHealthHUD = currentHealth;
    }

}
