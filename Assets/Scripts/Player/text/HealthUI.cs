using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Text text;
    public Health health;
    void Start()
    {
        text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        text.text = (health.currentHealth).ToString();
    }
}
