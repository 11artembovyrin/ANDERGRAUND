using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    private Text text;
    public PlayerController player;
    void Start()
    {
        text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        text.text = (player.currentAmmo).ToString();
    }
}
